using UnityEngine;
using EzySlice;
using System.Collections.Generic;
using System.Collections;

public class BlockShatterMechanic : MonoBehaviour
{
    [Header("Cài đặt vùng kiểm tra")]
    public float checkRadius = 1.4f; // Trở lại con số 1.4f chuẩn nhất cho lúc đếm

    [Header("EzySlice Settings")]
    public Material crossSectionMaterial;
    public bool isBreaking = false;

    public void CheckSurroundingAndBreak()
    {
        List<BlockShatterMechanic> connectedBlocks = new List<BlockShatterMechanic>();
        Queue<BlockShatterMechanic> queue = new Queue<BlockShatterMechanic>();

        queue.Enqueue(this);
        connectedBlocks.Add(this);

        while (queue.Count > 0)
        {
            BlockShatterMechanic current = queue.Dequeue();
            Collider[] neighbors = Physics.OverlapSphere(current.transform.position, checkRadius);

            foreach (Collider col in neighbors)
            {
                BlockShatterMechanic neighborScript = col.GetComponent<BlockShatterMechanic>();
                if (neighborScript != null && neighborScript.enabled && !neighborScript.isBreaking && col.CompareTag(this.gameObject.tag))
                {
                    if (!connectedBlocks.Contains(neighborScript))
                    {
                        connectedBlocks.Add(neighborScript);
                        queue.Enqueue(neighborScript);
                    }
                }
            }
        }

        if (connectedBlocks.Count >= 3)
        {
            foreach (var block in connectedBlocks)
            {
                block.TriggerBreak();
            }
        }
        else
        {
            CheckTopStructure();
        }
    }

    public void TriggerBreak()
    {
        if (isBreaking) return;
        isBreaking = true;

        Collider[] allNearby = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (Collider col in allNearby)
        {
            if (col.gameObject == this.gameObject) continue;

            BlockShatterMechanic neighborBlock = col.GetComponent<BlockShatterMechanic>();
            if (neighborBlock != null && !neighborBlock.isBreaking)
            {
                neighborBlock.CheckTopStructure();
            }
        }
        PerformShatter();
    }

    public void CheckTopStructure()
    {
        if (isBreaking) return;
        StartCoroutine(DelayedTopCheck());
    }

    private IEnumerator DelayedTopCheck()
    {
        yield return new WaitForSeconds(0.15f);

        if (isBreaking) yield break;

        bool isConnectedToCeiling = false;
        HashSet<BlockShatterMechanic> visited = new HashSet<BlockShatterMechanic>();
        Queue<BlockShatterMechanic> queue = new Queue<BlockShatterMechanic>();

        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            BlockShatterMechanic current = queue.Dequeue();
            Collider[] neighbors = Physics.OverlapSphere(current.transform.position, checkRadius);

            foreach (Collider col in neighbors)
            {
                if (col.CompareTag("Wall") && col.gameObject.name.Contains("Wall_Ceiling"))
                {
                    isConnectedToCeiling = true;
                    break;
                }

                BlockShatterMechanic neighborScript = col.GetComponent<BlockShatterMechanic>();
                if (neighborScript != null && !neighborScript.isBreaking && !visited.Contains(neighborScript))
                {
                    visited.Add(neighborScript);
                    queue.Enqueue(neighborScript);
                }
            }

            if (isConnectedToCeiling) break;
        }

        if (!isConnectedToCeiling)
        {
            TriggerBreak();
        }
    }

    private void PerformShatter()
    {
        Vector3 slicePos = transform.position;
        Vector3 sliceNormal = Random.onUnitSphere;
        SlicedHull hull = null;

        try
        {
            hull = gameObject.Slice(slicePos, sliceNormal);
        }
        catch (System.Exception) { } // Bỏ qua lỗi thư viện cắt để chống đứng game

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(gameObject, crossSectionMaterial);

            if (this.transform.parent != null)
            {
                upperHull.transform.SetParent(this.transform.parent, true);
                lowerHull.transform.SetParent(this.transform.parent, true);
            }

            upperHull.transform.position = this.transform.position;
            upperHull.transform.rotation = this.transform.rotation;
            lowerHull.transform.position = this.transform.position;
            lowerHull.transform.rotation = this.transform.rotation;

            SetupSlicedPiece(upperHull);
            SetupSlicedPiece(lowerHull);
        }

        Destroy(gameObject);
    }

    private void SetupSlicedPiece(GameObject piece)
    {
        Rigidbody rb = piece.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        MeshCollider col = piece.AddComponent<MeshCollider>();
        col.convex = true;
        col.isTrigger = true;

        piece.transform.localScale = transform.localScale * 0.95f;
        rb.AddExplosionForce(50f, transform.position, 2f);
        Destroy(piece, 3f);
    }
}