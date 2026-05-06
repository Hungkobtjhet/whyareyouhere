using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Giới hạn lưới (Tọa độ World Space)")]
    public int leftLimit = -5;
    public int rightLimit = 4; // Chiều rộng 10 ô (từ -5 đến 4)
    public int bottomLimit = 0; // Mức sàn thấp nhất
    public int topLimit = 20;

    public int Width { get { return rightLimit - leftLimit + 1; } }
    public int Height { get { return topLimit - bottomLimit + 1; } }

    // Mảng 2 chiều để lưu trữ các ô gạch nhỏ đã rơi xuống
    public Transform[,] grid;

    void Awake()
    {
        Instance = this;
        grid = new Transform[Width, Height];
    }

    // Chuyển đổi tọa độ Unity sang Index của mảng
    public Vector2Int WorldToGrid(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x) - leftLimit;
        int y = Mathf.RoundToInt(pos.y) - bottomLimit;
        return new Vector2Int(x, y);
    }

    // Kiểm tra xem vị trí có hợp lệ không (chưa chạm viền/đáy và chưa có khối nào chiếm chỗ)
    public bool IsValidPosition(Vector3 pos)
    {
        Vector2Int gridPos = WorldToGrid(pos);

        // 1. Kiểm tra lọt ra ngoài viền trái, phải hoặc dưới đáy
        if (gridPos.x < 0 || gridPos.x >= Width || gridPos.y < 0) return false;

        // 2. Kiểm tra đụng khối gạch khác (chỉ check nếu nằm trong giới hạn chiều cao)
        if (gridPos.y < Height && grid[gridPos.x, gridPos.y] != null) return false;

        return true;
    }

    // Ghi một ô gạch vào mảng
    public void AddToGrid(Transform blockPart)
    {
        Vector2Int pos = WorldToGrid(blockPart.position);
        if (pos.y < Height && pos.y >= 0 && pos.x >= 0 && pos.x < Width)
        {
            grid[pos.x, pos.y] = blockPart;
        }
    }

    // --- LOGIC XÓA HÀNG ---
    public void CheckForLines()
    {
        for (int y = 0; y < Height; y++)
        {
            if (HasFullLine(y))
            {
                DeleteLine(y);
                DecreaseRowsAbove(y + 1);
                y--; // Lùi lại 1 bước để kiểm tra lại chính hàng này (vì hàng trên vừa rớt xuống)
            }
        }
    }

    bool HasFullLine(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if (grid[x, y] == null) return false; // Có ô trống -> Chưa đầy
        }
        return true;
    }

    void DeleteLine(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject); // Xóa game object thực tế trên Scene
                grid[x, y] = null; // Xóa dữ liệu trong mảng
            }
        }
    }

    void DecreaseRowsAbove(int yStart)
    {
        for (int y = yStart; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (grid[x, y] != null)
                {
                    // Dịch dữ liệu xuống 1 hàng trong mảng
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    // Kéo ô gạch trên Scene rơi xuống 1 unit
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }
}