using UnityEngine;

public class DistanceSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float spawnInterval = 30f;
    public float spawnOffsetForward = 50f; // Khoảng cách vật thể xuất hiện trước mặt

    private float lastSpawnX;

    void Start()
    {
        // Lấy vị trí X hiện tại làm mốc ban đầu
        lastSpawnX = transform.position.x;
    }

    void Update()
    {
        // Kiểm tra nếu đi được quãng đường bằng spawnInterval trên trục X
        if (transform.position.x >= lastSpawnX + spawnInterval)
        {
            SpawnObject();
            lastSpawnX = transform.position.x;
        }
    }

    void SpawnObject()
    {
        // SỬA TẠI ĐÂY: Giữ nguyên Y và Z của Spawner, chỉ thay đổi X
        Vector3 spawnPos = new Vector3(
            transform.position.x + spawnOffsetForward,
            transform.position.y,
            transform.position.z
        );

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}