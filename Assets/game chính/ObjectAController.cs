using System.Collections;
using UnityEngine;

public class ObjectAController : MonoBehaviour
{
    [Header("Cài đặt Vật thể")]
    public GameObject objectB;       // Vật thể B cần chạm
    public GameObject objectCPrefab; // Prefab của vật thể C (để spawn)
    public Transform spawnPointC;    // Vị trí sẽ spawn vật thể C
    public GameObject objectD;       // Vật thể D cần xóa

    [Header("Cài đặt Thời gian")]
    public float timeToTouchB = 5f;  // Thời gian chờ A chạm B (giây)
    public float timeToDeleteD = 3f; // Thời gian chờ sau khi spawn C để xóa D (giây)

    private bool hasTouchedB = false;

    void Start()
    {
        // Bắt đầu chuỗi đếm thời gian
        StartCoroutine(CheckAndSpawnRoutine());
    }

    // Hàm kiểm tra va chạm
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == objectB)
        {
            hasTouchedB = true;
            Debug.Log("Vật thể A đã chạm Vật thể B kịp lúc!");

            // LƯU Ý: Nếu bạn muốn A tự xóa NGAY LẬP TỨC khi chạm B thành công 
            // (không cần đợi hàm Coroutine chạy xong), hãy bỏ dấu // ở dòng bên dưới:
            // Destroy(gameObject); 
        }
    }

    // Coroutine xử lý toàn bộ logic
    private IEnumerator CheckAndSpawnRoutine()
    {
        float timer = 0f;

        // Vòng lặp đếm thời gian
        while (timer < timeToTouchB && !hasTouchedB)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Kịch bản thất bại: Không chạm B kịp lúc
        if (!hasTouchedB)
        {
            Debug.Log("Quá thời gian! Đang spawn Vật thể C...");
            Instantiate(objectCPrefab, spawnPointC.position, spawnPointC.rotation);

            // Chờ trước khi xóa D
            yield return new WaitForSeconds(timeToDeleteD);

            if (objectD != null)
            {
                Debug.Log("Đang xóa Vật thể D...");
                Destroy(objectD);
            }
        }

        // --- PHẦN THÊM MỚI ---
        // Xóa Vật thể A (vật thể đang chứa script này) sau khi hoàn tất mọi kịch bản
        Debug.Log("Chuỗi sự kiện đã hoàn tất. Đang tự xóa chính mình...");
        Destroy(gameObject);

        // Mẹo: Nếu bạn chỉ muốn xóa script này để vật thể A mất đi tính năng đếm giờ, 
        // nhưng VẪN GIỮ LẠI khối hình của vật thể A trên Scene, hãy dùng lệnh: 
        // Destroy(this);
    }
}