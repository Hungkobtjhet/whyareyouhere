using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPerpendicularMove : MonoBehaviour
{
    [Header("References")]
    public Transform mainCamera;
    public InputActionReference wasdAction;
    public LayerMask groundLayer; // Gán Layer của các bề mặt có thể đứng được vào đây

    [Header("Movement Settings")]
    public float constantSpeed = 10f;

    [Header("Detection Settings")]
    public float checkForwardDistance = 0.6f; // Khoảng cách dò trước mặt (bán kính khối + một chút)
    public float rayLength = 2.0f;           // Tia phải đủ dài để xuyên qua khối và chạm bề mặt phía sau

    private Rigidbody rb;
    [Header("Custom Gravity")]
    [Tooltip("Hệ số nhân trọng lực (Chỉ hoạt động khi ô Use Gravity của Rigidbody được bật)")]
    public float gravityMultiplier = 20f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        // Đảm bảo không có ma sát gây chậm vận tốc cố định
        rb.linearDamping = 0;
    }

    void OnEnable() { wasdAction.action.Enable(); }
    void OnDisable() { wasdAction.action.Disable(); }
    void FixedUpdate()
    {
        // 1. CHỈ ÁP DỤNG TRỌNG LỰC NHÂN TẠO KHI BẬT USE GRAVITY
        if (rb.useGravity)
        {
            // Unity mặc định đã kéo 1 lần Physics.gravity. 
            // Ta cộng bù thêm phần chênh lệch để tổng lực bằng đúng gravityMultiplier.
            Vector3 extraGravity = Physics.gravity * (gravityMultiplier - 1f);
            rb.AddForce(extraGravity, ForceMode.Acceleration);
        }

        Vector2 input = wasdAction.action.ReadValue<Vector2>();

        // 2. KHI KHÔNG BẤM PHÍM (Hoặc chạm vực)
        if (input.sqrMagnitude < 0.01f)
        {
            // Nếu có trọng lực: chặn bay lên, giữ nguyên quán tính rơi xuống.
            // Nếu KHÔNG trọng lực: dừng hẳn trục Y (lơ lửng).
            float yVel = rb.useGravity ? Mathf.Min(0f, rb.linearVelocity.y) : 0f;

            rb.linearVelocity = new Vector3(0, yVel, 0);
            return;
        }

        Vector3 moveDir = (mainCamera.right * input.x + mainCamera.up * input.y).normalized;

        // 3. KHI DI CHUYỂN
        // 3. KHI DI CHUYỂN
        if (IsGroundAhead(moveDir))
        {
            float targetVelY = rb.linearVelocity.y;

            if (Mathf.Abs(moveDir.y) > 0.01f)
            {
                targetVelY = moveDir.y * constantSpeed;
            }
            else if (!rb.useGravity)
            {
                targetVelY = 0f;
            }

            // Tính toán vận tốc di chuyển cơ bản
            Vector3 finalVelocity = new Vector3(moveDir.x * constantSpeed, targetVelY, moveDir.z * constantSpeed);

            // CÁCH SỬA LỖI TẠI ĐÂY:
            // Nếu tắt Gravity, nhân vật rất dễ bị trôi ra khỏi bề mặt. 
            // Ta cộng thêm một vận tốc nhỏ hướng thẳng vào tường (theo hướng nhìn của camera) để ép nhân vật luôn dính vào bề mặt.
            if (!rb.useGravity)
            {
                finalVelocity += mainCamera.forward * 2f; // Ép nhẹ vào tường với lực = 2
            }

            rb.linearVelocity = finalVelocity;
        }
        else
        {
            // Gặp vực
            float yVel = rb.useGravity ? Mathf.Min(0f, rb.linearVelocity.y) : 0f;
            rb.linearVelocity = new Vector3(0, yVel, 0);
        }
    }

    bool IsGroundAhead(Vector3 direction)
    {
        // Vị trí bắt đầu bắn tia: Từ tâm nhân vật đẩy về phía trước một khoảng (theo hướng dự kiến đi)
        // và cộng thêm Vector3.up * 0.1f để nhấc điểm bắt đầu bắn tia lên cao 0.1 unit (tránh lỗi kẹt tia)
        Vector3 rayOrigin = transform.position + (direction * checkForwardDistance) + (Vector3.up * 0.1f);

        // Vẽ tia debug trong Scene view để bạn dễ căn chỉnh (chỉ thấy khi nhấn Play)
        // Tia bây giờ bắn thẳng theo hướng nhìn của camera (Forward)
        Debug.DrawRay(rayOrigin, mainCamera.forward * rayLength, Color.red);

        // Bắn tia VỀ PHÍA TRƯỚC (theo tầm nhìn camera)
        return Physics.Raycast(rayOrigin, mainCamera.forward, rayLength, groundLayer);
    }
}