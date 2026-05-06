using UnityEngine;
using System.Collections.Generic;

public class SnakeController : MonoBehaviour
{
    [Header("Tail Settings")]
    public float lerpSpeed = 10f;      // Tốc độ mượt của đuôi
    public int distanceGap = 15;      // Khoảng cách giữa các đốt đuôi trong lịch sử vị trí
    public List<Transform> bodyParts = new List<Transform>();

    private List<Vector3> positionsHistory = new List<Vector3>();
    public BlockSpawner spawner;

    // Trong hàm Start của SnakeController, chỉ gọi 1 lần duy nhất
    void Start()
    {
        if (bodyParts.Count == 0) bodyParts.Add(this.transform);

        // Tạo khối đầu tiên ở một vị trí ngẫu nhiên trong các Spawner
        BlockSpawner.SpawnInRandomManager();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            other.tag = "Untagged";
            bodyParts.Add(other.transform);

            // Mỗi khi ăn xong, yêu cầu hệ thống chọn ngẫu nhiên 1 Spawner để tạo khối mới
            BlockSpawner.SpawnInRandomManager();
        }
    }

    void Update()
    {
        // 1. Ghi lại vị trí hiện tại (do script di chuyển kia điều khiển)
        // Lưu ý: Ta lưu vị trí ở Update để đuôi di chuyển mượt mà hơn theo mắt nhìn
        positionsHistory.Insert(0, transform.position);

        // 2. Điều khiển các khối đuôi đi theo
        int index = 1;
        foreach (var body in bodyParts)
        {
            if (body == transform) continue; // Bỏ qua đầu rắn

            // Lấy vị trí cũ từ lịch sử dựa trên chỉ số (index) của đốt đuôi
            int historyIndex = Mathf.Min(index * distanceGap, positionsHistory.Count - 1);
            Vector3 targetPos = positionsHistory[historyIndex];

            // Di chuyển mượt đốt đuôi tới vị trí đó
            body.position = Vector3.Lerp(body.position, targetPos, Time.deltaTime * lerpSpeed);

            // Xoay đốt đuôi nhìn theo hướng di chuyển (tùy chọn)
            body.LookAt(targetPos);

            index++;
        }

        // Tối ưu bộ nhớ: Xóa bớt lịch sử vị trí quá cũ
        if (positionsHistory.Count > bodyParts.Count * distanceGap + 100)
        {
            positionsHistory.RemoveAt(positionsHistory.Count - 1);
        }
    }
}