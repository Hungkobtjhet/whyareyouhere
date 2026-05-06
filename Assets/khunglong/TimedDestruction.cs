using UnityEngine;

public class TimedDestruction : MonoBehaviour
{
    public float lifeTime = 30f; // Thời gian tồn tại (giây) trước khi biến mất

    void Start()
    {
        // Hàm Destroy có tham số thứ 2 là thời gian chờ
        // Nó sẽ tự đếm ngược và xóa vật thể này sau lifeTime giây
        Destroy(gameObject, lifeTime);
    }
}