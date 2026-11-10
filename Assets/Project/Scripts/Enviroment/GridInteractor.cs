using UnityEngine;

public class GridInteractor : MonoBehaviour
{
    [Header("Preview Settings")]
    [SerializeField] private GameObject groundTilePrefab; // Prefab hiển thị khi hover
    [SerializeField] private Grid grid;

    private GameObject previewInstance; // Đối tượng giữ bản preview hiển thị theo chuột

    private void Start()
    {
        if (groundTilePrefab == null || grid == null) return;

        // Tạo sẵn 1 instance để làm preview và ẩn nó đi lúc đầu
        previewInstance = Instantiate(groundTilePrefab);

        // Tắt các Collider của bản preview đi để nó không cản trở raycast chuột
        foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        previewInstance.SetActive(false);
        SystemGameManager.Instance.EventManager.OnPointerClick += HandlePointerClick;
        SystemGameManager.Instance.EventManager.OnDataLoadCompleted += HandleLoadItemsOnGrid;
        // var gridService = SystemGameManager.Instance.DataLoadManager._itemOnGridService;
        // if (gridService != null && gridService.ItemsOnGrid != null && gridService.ItemsOnGrid.Count > 0)
        // {
        //     // Nếu đã có data sẵn rồi thì tự động gọi hàm spawn luôn
        //     HandleLoadItemsOnGrid();
        // }
    }

    public void HandleLoadItemsOnGrid()
    {
        // Debug.Log("[GridInteractor] Data load completed. Loading items on grid... 22");
        // Load dữ liệu từ dịch vụ IItemOnGrid
        Debug.Log("[GridInteractor] Data load completed. Loading items on grid... 22");
        // if (SystemGameManager.Instance.DataLoadManager._itemOnGridService != null)
        // {
        //     // Debug.Log("[GridInteractor] Data load completed. Loading items on grid... 22");
        //     var itemsOnGrid = SystemGameManager.Instance.DataLoadManager._itemOnGridService.ItemsOnGrid;
        //     // Debug.Log($"[GridInteractor] Loaded {itemsOnGrid.Count} items on grid. 33");
        //     foreach (var kvp in itemsOnGrid)
        //     {

        //         var item = kvp.Value;
        //         Vector3 spawnPosition = new Vector3(item.position[0], item.position[1], item.position[2]);
        //         // Debug.Log($"[GridInteractor] Spawned item at ({item.Key.Item1}, {kvp.Key.Item2}) with position {kvp.Value.position}. 44");
        //         Debug.Log($"[GridInteractor] Spawned item at ({item.cellX}, {item.cellZ}) with position {spawnPosition}. 44");
        //         Instantiate(groundTilePrefab, spawnPosition, Quaternion.identity);
        //     }
        // }
    }
    // private void Update()
    // {
    //     // Liên tục cập nhật vị trí chuột để hiện preview bám theo grid
    //     // UpdateTilePreview();
    // }

    // private void UpdateTilePreview()
    // {
    //     if (previewInstance == null || grid == null) return;

    //     // Bắn Raycast từ vị trí chuột hiện tại
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
    //     {
    //         Vector3 gridOrigin = grid.transform.position;
    //         Vector3 cellSize = grid.cellSize;

    //         // Tính toán tọa độ ô (cell) mà chuột đang chỉ vào
    //         int clickedX = Mathf.FloorToInt((hit.point.x - gridOrigin.x) / cellSize.x);
    //         int clickedZ = Mathf.FloorToInt((hit.point.z - gridOrigin.z) / cellSize.z);

    //         // Tính toán vị trí tâm của ô lưới đó
    //         Vector3 cellCenterWorldPos = new Vector3(
    //             gridOrigin.x + (clickedX * cellSize.x) + (cellSize.x / 2f),
    //             0.05f, // Đặt một chút cao hơn mặt đất để preview không bị z-fighting
    //             gridOrigin.z + (clickedZ * cellSize.z) + (cellSize.z / 2f)
    //         );

    //         // Bật preview lên và dịch chuyển đến tâm ô lưới
    //         previewInstance.SetActive(true);
    //         previewInstance.transform.position = cellCenterWorldPos;
    //     }
    //     else
    //     {
    //         // Nếu chuột không chĩa vào mặt phẳng sàn thì ẩn preview đi
    //         previewInstance.SetActive(false);
    //     }
    // }

    private void HandlePointerClick(PointerClickEvent evt)
    {
        if (evt.Mode != InputMode.Building) return;

        // Khi click chuột trái (hoặc bấm xây dựng), thay vì dùng preview nữa
        // thì ta sinh ra một bản chuẩn thức (hoặc giữ nguyên vị trí của preview và tạo mới một cái khác để giữ chỗ)
        Ray ray = Camera.main.ScreenPointToRay(evt.ScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 gridOrigin = grid.transform.position;
            Vector3 cellSize = grid.cellSize;

            int clickedX = Mathf.FloorToInt((hit.point.x - gridOrigin.x) / cellSize.x);
            int clickedZ = Mathf.FloorToInt((hit.point.z - gridOrigin.z) / cellSize.z);

            Vector3 spawnPosition = new Vector3(
                gridOrigin.x + (clickedX * cellSize.x) + (cellSize.x / 2f),
                0.05f, // Đặt một chút cao hơn mặt đất để preview không bị z-fighting
                gridOrigin.z + (clickedZ * cellSize.z) + (cellSize.z / 2f)
            );

            // Spawn thật một tile cố định xuống ô đó
            Instantiate(groundTilePrefab, spawnPosition, Quaternion.identity);
            // SystemGameManager.Instance.DataLoadManager._itemOnGridService.AddItemOnGridAsync(new ItemOnGridModel
            // {
            //     cellX = clickedX,
            //     cellZ = clickedZ,

            //     // Convert Unity Vector3/Quaternion to standard float arrays
            //     position = new float[] { spawnPosition.x, spawnPosition.y, spawnPosition.z },
            //     rotation = new float[] { Quaternion.identity.x, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w },
            //     scale = new float[] { 1f, 1f, 1f }
            // }).ContinueWith(task =>
            // {
            //     if (task.IsFaulted)
            //     {
            //         Debug.LogError("Failed to add item on grid: " + task.Exception);
            //     }
            //     else
            //     {
            //         Debug.Log($"Item added on grid at ({clickedX}, {clickedZ})");
            //     }
            // });
        }
    }

    private void OnDisable()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
        {
            SystemGameManager.Instance.EventManager.OnPointerClick -= HandlePointerClick;
            SystemGameManager.Instance.EventManager.OnDataLoadCompleted -= HandleLoadItemsOnGrid;
        }
    }
}