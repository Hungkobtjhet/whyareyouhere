using UnityEngine;

public class RandomScaleOnStart : MonoBehaviour
{
    [Header("Mục tiêu để bám vào (Sàn hoặc Trần)")]
    public Transform surfaceTarget;

    [Header("Phạm vi chiều cao")]
    public float minHeight = 1.0f;
    public float maxHeight = 5.0f;

    void Start()
    {
        if (surfaceTarget == null)
        {
            Debug.LogError("Vui lòng kéo Sàn hoặc Trần vào ô Surface Target!");
            return;
        }

        ApplySmartScale();
    }

    void ApplySmartScale()
    {
        // 1. Tạo chiều cao ngẫu nhiên mới
        float newHeight = Random.Range(minHeight, maxHeight);

        // 2. Xác định hướng: Cột đang ở TRÊN hay DƯỚI mặt phẳng mục tiêu?
        // Nếu Y cột > Y mặt phẳng => Cột đang đứng trên sàn (đẩy lên)
        // Nếu Y cột < Y mặt phẳng => Cột đang treo dưới trần (đẩy xuống)
        bool isStandingOnSurface = transform.position.y > surfaceTarget.position.y;

        // 3. Cập nhật Scale trục Y
        Vector3 newScale = transform.localScale;
        newScale.y = newHeight;
        transform.localScale = newScale;

        // 4. Tính toán vị trí mới
        Vector3 newPosition = transform.position;

        if (isStandingOnSurface)
        {
            // Trường hợp LÀM CỘT NHÀ (Bám sàn):
            // Chân cột = Y của mặt phẳng + (độ dày mặt phẳng / 2 nếu có)
            // Ở đây ta lấy đơn giản là bám vào tâm Y của surfaceTarget
            newPosition.y = surfaceTarget.position.y + (newHeight / 2f);
        }
        else
        {
            // Trường hợp LÀM THẠCH NHŨ/ĐÈN TRẦN (Bám trần):
            // Đỉnh cột = Y của mặt phẳng - (chiều cao cột / 2)
            newPosition.y = surfaceTarget.position.y - (newHeight / 2f);
        }

        transform.position = newPosition;
    }
}