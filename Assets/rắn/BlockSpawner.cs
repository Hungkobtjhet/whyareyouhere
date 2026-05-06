using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    // Danh sách tĩnh chứa tất cả các Spawner đang có trong Scene
    public static List<BlockSpawner> allSpawners = new List<BlockSpawner>();

    public GameObject foodPrefab;
    public Vector3 planeSize = new Vector3(10, 10, 10);
    public float checkRadius = 0.5f;
    public LayerMask blockLayer;

    void OnEnable()
    {
        // Tự thêm mình vào danh sách khi được bật lên
        if (!allSpawners.Contains(this)) allSpawners.Add(this);
    }

    void OnDisable()
    {
        // Tự xóa mình khỏi danh sách khi bị tắt đi
        allSpawners.Remove(this);
    }

    // Hàm chọn ngẫu nhiên một Spawner để tạo khối
    public static void SpawnInRandomManager()
    {
        if (allSpawners.Count > 0)
        {
            int randomIndex = Random.Range(0, allSpawners.Count);
            allSpawners[randomIndex].SpawnNewBlock();
        }
    }

    public void SpawnNewBlock()
    {
        Vector3 spawnPos = Vector3.zero;
        bool canSpawn = false;
        int attempts = 0;

        while (!canSpawn && attempts < 100)
        {
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomY = Random.Range(-planeSize.y / 2, planeSize.y / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);
            spawnPos = transform.position + new Vector3(randomX, randomY, randomZ);

            if (!Physics.CheckSphere(spawnPos, checkRadius, blockLayer))
            {
                canSpawn = true;
            }
            attempts++;
        }

        if (canSpawn)
        {
            Instantiate(foodPrefab, spawnPos, Quaternion.identity);
        }
    }
}