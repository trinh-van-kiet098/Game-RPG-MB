using UnityEngine;

public class ScenceMain : ConfiguredMonoBehaviour
{
    [Header("Map Settings (Giới hạn bản đồ)")]
    [SerializeField] private int mapWidth = 50;  // Trục X có 50 ô
    [SerializeField] private int mapHeight = 50; // Trục Y (hoặc Z) có 50 ô
    
    [Header("Grid Offset (Độ lệch của Plane)")]
    [SerializeField] public float cellSize = 1f; // Kích thước 1 ô
    [SerializeField] private Vector3 originPosition; // Tọa độ góc dưới cùng bên trái của mặt đất (Plane)

    public static ScenceMain Instance { get; private set; }
    
    private byte[] gridMapData;

    public override void Configure(SystemGameManager manager)
    {
        base.Configure(manager);
        Instance = this;

        gridMapData = new byte[mapWidth * mapHeight];
        Debug.Log($"[ScenceMain] Đã tạo bản đồ {mapWidth}x{mapHeight}. Gốc tọa độ tại: {originPosition}");
    }

    // ==========================================
    // CÁC HÀM CHUYỂN ĐỔI TỌA ĐỘ (WORLD <-> GRID)
    // ==========================================

    /// <summary>
    /// Biến tọa độ đâm của tia Raycast (Thực) thành Index ô lưới (Ảo)
    /// </summary>
    public void WorldToGridPosition(Vector3 worldPosition, out int gridX, out int gridZ)
    {
        // Trừ đi độ lệch (originPosition) trước khi chia cho cellSize
        gridX = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
        gridZ = Mathf.FloorToInt((worldPosition.z - originPosition.z) / cellSize);
    }

    /// <summary>
    /// Tính tọa độ thực tế (chuẩn giữa ô) để đặt 3D Model vào
    /// </summary>
    public Vector3 GridToWorldPosition(int gridX, int gridZ, int sizeX = 1, int sizeZ = 1, float yPos = 0f)
    {
        // Cộng bù lại độ lệch ban đầu
        float offsetX = (sizeX * cellSize) / 2f;
        float offsetZ = (sizeZ * cellSize) / 2f;

        float worldX = originPosition.x + (gridX * cellSize) + offsetX;
        float worldZ = originPosition.z + (gridZ * cellSize) + offsetZ;

        return new Vector3(worldX, yPos, worldZ);
    }

    // ==========================================
    // CÁC HÀM QUẢN LÝ LƯỚI & KIỂM TRA GIỚI HẠN
    // ==========================================

    private int GetIndex(int x, int y) => x + (y * mapWidth);

    public bool IsWithinBounds(int x, int y) => x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;

    public bool CanBuildAt(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;

                if (!IsWithinBounds(checkX, checkY)) return false;
                if (gridMapData[GetIndex(checkX, checkY)] != 0) return false;
            }
        }
        return true;
    }

    public void OccupyCells(int startX, int startY, int sizeX, int sizeY, byte objectType = 1)
    {
        if (!CanBuildAt(startX, startY, sizeX, sizeY)) return;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                gridMapData[GetIndex(startX + x, startY + y)] = objectType;
            }
        }
    }
}