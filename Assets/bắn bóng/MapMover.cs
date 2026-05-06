using UnityEngine;
using System.Collections;

public class MapMover : MonoBehaviour
{
    public float moveStep = 1.0f;
    public float smoothSpeed = 2.0f;

    private bool isMoving = false;

    public void MoveMapStepDownSmooth()
    {
        if (!isMoving)
        {
            StartCoroutine(SmoothMoveCoroutine());
        }
    }

    private IEnumerator SmoothMoveCoroutine()
    {
        isMoving = true;
        Vector3 targetPos = transform.position - new Vector3(0, moveStep, 0);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, smoothSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}