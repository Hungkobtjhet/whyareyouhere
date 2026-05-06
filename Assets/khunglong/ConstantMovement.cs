using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
    public float speed = 10f; // Tốc độ di chuyển

    void Update()
    {
        // Di chuyển theo trục Z (phía trước) mỗi khung hình
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}