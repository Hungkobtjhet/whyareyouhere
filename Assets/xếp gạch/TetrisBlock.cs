using UnityEngine;
using UnityEngine.InputSystem;

public class TetrisBlock : MonoBehaviour
{
    public float fallTime = 0.8f;
    public Vector3 rotationAxis = Vector3.forward;

    private float previousTime;
    private bool isLocked = false;

    void Update()
    {
        if (isLocked || Keyboard.current == null) return;

        // Di chuyển Trái / Phải
        if (Keyboard.current.aKey.wasPressedThisFrame) Move(Vector3.left);
        else if (Keyboard.current.dKey.wasPressedThisFrame) Move(Vector3.right);

        // Xoay
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) Rotate(90f);
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame) Rotate(-90f);

        // Rơi tự động hoặc rơi nhanh (nhấn giữ S hoặc Mũi tên xuống)
        bool isSpeedingUp = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
        float currentFallTime = isSpeedingUp ? fallTime / 10f : fallTime;

        if (Time.time - previousTime > currentFallTime)
        {
            transform.position += Vector3.down;

            // Nếu vị trí sau khi rơi là không hợp lệ (chạm đáy hoặc khối khác)
            if (!IsValidGridPosition())
            {
                transform.position -= Vector3.down; // Lùi lại vị trí cũ
                LockAndSpawnNext();
            }
            previousTime = Time.time;
        }
    }

    void Move(Vector3 direction)
    {
        transform.position += direction;
        if (!IsValidGridPosition()) transform.position -= direction; // Đụng tường ảo thì lùi lại
    }

    void Rotate(float angle)
    {
        transform.Rotate(rotationAxis, angle, Space.World);
        if (!IsValidGridPosition()) transform.Rotate(rotationAxis, -angle, Space.World); // Kẹt thì không cho xoay
    }

    bool IsValidGridPosition()
    {
        // Kiểm tra xem GameObject này có khối con bên trong không (chuẩn Tetris thường có 4 khối con)
        if (transform.childCount == 0)
        {
            return GridManager.Instance.IsValidPosition(transform.position);
        }

        foreach (Transform child in transform)
        {
            if (!GridManager.Instance.IsValidPosition(child.position))
                return false;
        }
        return true;
    }

    void LockAndSpawnNext()
    {
        isLocked = true;

        // 1. Đăng ký các ô vuông vào lưới ảo
        if (transform.childCount == 0)
        {
            GridManager.Instance.AddToGrid(transform);
        }
        else
        {
            // Trích xuất các khối con vào mảng để duyệt an toàn
            Transform[] childArray = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++) childArray[i] = transform.GetChild(i);

            foreach (Transform child in childArray)
            {
                GridManager.Instance.AddToGrid(child);
            }
        }

        // 2. Kích hoạt quét lưới xem có hàng nào đầy không để dọn dẹp
        GridManager.Instance.CheckForLines();

        // 3. Tắt script này để nó ngưng hoạt động
        this.enabled = false;

        // 4. Gọi Spawner đẻ ra khối mới (đảm bảo bạn đang có 1 object mang script BlockSpawner trong Scene)
        FindFirstObjectByType<BlockSpawner1>()?.SpawnRandomBlock();
    }
}