using UnityEngine;

public class RandomEmissionColor : MonoBehaviour
{
    void Start()
    {
        // 1. Lấy component Renderer đang gắn trên GameObject
        Renderer rend = GetComponent<Renderer>();

        if (rend != null)
        {
            // 2. Tạo màu ngẫu nhiên với 3 giá trị R, G, B (từ 0.0 đến 1.0)
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            // Tùy chọn: Nếu muốn màu phát sáng mạnh hơn (HDR), nhân với một số cường độ (ví dụ: 2.5f)
            // randomColor *= 2.5f;

            // 3. Bật tính năng Emission trên Material (Bắt buộc với Standard Shader của Unity)
            rend.material.EnableKeyword("_EMISSION");

            // 4. Gán màu ngẫu nhiên vừa tạo vào thuộc tính Emission
            rend.material.SetColor("_EmissionColor", randomColor);
        }

        // 5. Xóa script này khỏi GameObject ngay sau khi đã đổi màu xong
        Destroy(this);
    }
}