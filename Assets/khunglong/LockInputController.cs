using UnityEngine;
using UnityEngine.InputSystem;
public class LockInputController : MonoBehaviour
{
    // Cờ 1: Vật B đã từng xuất hiện trên map chưa?
    private bool hasBAppeared = false;

    // Cờ 2: Đã chạm Vật B lần đầu tiên chưa?
    private bool hasTouchedFirstTime = false;

    // Cờ 3: Hiện tại có đang chạm Vật B không?
    private bool isTouchingB = false;

    void Update()
    {
        // Quét tìm tất cả vật thể có Tag là "kiemtra"
        GameObject[] bObjects = GameObject.FindGameObjectsWithTag("kiemtra");
        int bCount = bObjects.Length;

        // ==================================================
        // GIAI ĐOẠN 1: KIỂM TRA SỰ XUẤT HIỆN CỦA VẬT B
        // ==================================================
        if (!hasBAppeared)
        {
            if (bCount > 0)
            {
                hasBAppeared = true;
                Debug.Log("Vật B đã xuất hiện! Chuyển sang chờ chạm lần đầu.");
            }
            else
            {
                // Chưa có Vật B nào -> Cho phép dùng phím W tự do
                FreeMovement();
                return; // Ngắt vòng lặp
            }
        }

        // ==================================================
        // GIAI ĐOẠN 4: VẬT B HẾT SẠCH -> TẮT SCRIPT
        // ==================================================
        if (bCount == 0)
        {
            Debug.Log("Vật B đã biến mất hoàn toàn -> TẮT VĨNH VIỄN SCRIPT!");
            this.enabled = false;
            return;
        }

        // ==================================================
        // GIAI ĐOẠN 2: CHỜ CHẠM LẦN ĐẦU TIÊN
        // ==================================================
        if (!hasTouchedFirstTime)
        {
            // Vật B đã xuất hiện nhưng chưa chạm lần nào -> Vẫn dùng W tự do
            FreeMovement();
            return;
        }

        // ==================================================
        // GIAI ĐOẠN 3: LUẬT CHƠI CHÍNH THỨC HOẠT ĐỘNG
        // ==================================================
        // Kiểm tra phím W có đang được GIỮ bằng New Input System
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
        {
            if (isTouchingB)
            {
                Debug.Log("Luật kích hoạt: Đang chạm Vật B -> MỞ KHÓA phím W!");
                // transform.Translate(Vector3.forward * Time.deltaTime);
            }
            else
            {
                Debug.Log("Luật kích hoạt: Rời Vật B -> KHÓA phím W!");
            }
        }
    }

    // --- HÀM PHỤ TRỢ ---
    private void FreeMovement()
    {
        // Kiểm tra phím W an toàn bằng New Input System
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
        {
            Debug.Log("Trạng thái tự do: Phím W hoạt động bình thường.");
            // Viết code di chuyển tự do vào đây:
            // transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

    // Các hàm kiểm tra vật lý giữ nguyên, không ảnh hưởng bởi hệ thống Input
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("kiemtra"))
        {
            if (!hasTouchedFirstTime)
            {
                hasTouchedFirstTime = true;
                Debug.Log("ĐÃ CHẠM LẦN ĐẦU! Bắt đầu áp dụng luật khóa phím.");
            }

            isTouchingB = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("kiemtra"))
        {
            isTouchingB = false;
        }
    }
}