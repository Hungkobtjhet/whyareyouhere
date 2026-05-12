using UnityEngine;

public class TeleportOnFall : MonoBehaviour
{
    [Header("Cấu hình độ cao")]
    [Tooltip("Nếu vật thể rơi xuống thấp hơn mốc này sẽ bị dịch chuyển")]
    public float thresholdY = -10f;

    [Header("Cấu hình đích đến")]
    [Tooltip("Tọa độ mà vật thể sẽ biến đến")]
    public Vector3 targetPosition = new Vector3(0, 5, 0);

    [Header("Tùy chọn")]
    [Tooltip("Nếu tích chọn, vật thể sẽ dừng mọi lực quán tính khi dịch chuyển")]
    public bool resetVelocity = true;

    private Rigidbody rb;

    void Start()
    {
        // Lấy thành phần Rigidbody nếu có để xử lý vận tốc
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Kiểm tra xem vị trí hiện tại có thấp hơn ngưỡng cho phép không
        if (transform.position.y < thresholdY)
        {
            Teleport();
        }
    }

    void Teleport()
    {
        // Thay đổi vị trí của vật thể
        transform.position = targetPosition;

        // Nếu vật thể có vật lý (Rigidbody), ta nên triệt tiêu lực rơi cũ
        if (resetVelocity && rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Reset vận tốc di chuyển
            rb.angularVelocity = Vector3.zero; // Reset vận tốc quay
        }

        Debug.Log(gameObject.name + " đã được đưa về tọa độ an toàn!");
    }
}