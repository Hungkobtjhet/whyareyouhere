using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraSixFixedPositions : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera camToControl;
    public InputActionReference arrowKeysAction;

    private CinemachineFollow followModule;
    private Vector2 lastInput;

    // Định nghĩa cứng 6 tọa độ cố định
    private readonly Vector3 PosTop = new Vector3(0, 20, 0);
    private readonly Vector3 PosBottom = new Vector3(0, -20, 0);
    private readonly Vector3 PosFront = new Vector3(0, 0, 20);
    private readonly Vector3 PosRight = new Vector3(20, 0, 0);
    private readonly Vector3 PosBack = new Vector3(0, 0, -20);
    private readonly Vector3 PosLeft = new Vector3(-20, 0, 0);

    void Start()
    {
        if (camToControl != null)
        {
            followModule = camToControl.GetComponent<CinemachineFollow>();
            // Khởi tạo camera luôn bắt đầu ở mặt trên
            followModule.FollowOffset = PosTop;
        }
    }

    void OnEnable() { arrowKeysAction.action.Enable(); }
    void OnDisable() { arrowKeysAction.action.Disable(); }

    void Update()
    {
        if (followModule == null) return;

        Vector2 input = arrowKeysAction.action.ReadValue<Vector2>();

        // Chỉ kích hoạt khi vừa bấm phím để camera nhảy dứt khoát 1 lần
        if (input != Vector2.zero && lastInput == Vector2.zero)
        {
            Vector3 currentPos = followModule.FollowOffset;

            if (input.y > 0) // Mũi tên Lên
            {
                followModule.FollowOffset = PosTop;
            }
            else if (input.y < 0) // Mũi tên Xuống
            {
                followModule.FollowOffset = PosBottom;
            }
            else if (input.x > 0) // Mũi tên Phải -> Xoay vòng qua phải
            {
                if (currentPos == PosFront) followModule.FollowOffset = PosRight;
                else if (currentPos == PosRight) followModule.FollowOffset = PosBack;
                else if (currentPos == PosBack) followModule.FollowOffset = PosLeft;
                else followModule.FollowOffset = PosFront; // Fallback nếu đang ở Top/Bottom
            }
            else if (input.x < 0) // Mũi tên Trái -> Xoay vòng qua trái
            {
                if (currentPos == PosFront) followModule.FollowOffset = PosLeft;
                else if (currentPos == PosLeft) followModule.FollowOffset = PosBack;
                else if (currentPos == PosBack) followModule.FollowOffset = PosRight;
                else followModule.FollowOffset = PosFront; // Fallback nếu đang ở Top/Bottom
            }
        }

        lastInput = input;
    }
}