using UnityEngine;

public class BlockSpawner1 : MonoBehaviour
{
    [Header("Danh sách các khối gạch")]
    public GameObject[] tetrominoPrefabs;

    void Start()
    {
        SpawnRandomBlock();
    }

    public void SpawnRandomBlock()
    {
        if (tetrominoPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, tetrominoPrefabs.Length);
        Instantiate(tetrominoPrefabs[randomIndex], transform.position, Quaternion.identity);
    }
}