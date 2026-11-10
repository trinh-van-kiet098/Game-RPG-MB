using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GridObject : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Định danh vật thể")]
    public string objectID;       // Dùng để load Addressables hoặc save game (VD: "Building_House_01")
    public string objectName;     // Tên hiển thị
    public Vector2Int gridPosition; // Tọa độ hiện tại trên ma trận N*M

    [Header("Trạng thái kéo thả")]
    protected bool isDragging = false;
    protected Vector3 originalPosition;

    // --- 1. XỬ LÝ CLICK & DOUBLE CLICK ---
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // Nếu đang kéo thả thì bỏ qua sự kiện click
        if (isDragging) return;

        if (eventData.clickCount == 2)
        {
            OnDoubleClick();
        }
        else if (eventData.clickCount == 1)
        {
            OnClick();
        }
    }

    // Các lớp con (như Nhà, Cây, Khoáng sản) sẽ viết logic riêng cho các hàm này
    protected abstract void OnClick();
    protected abstract void OnDoubleClick();


    // --- 2. XỬ LÝ KÉO THẢ (DRAG & DROP) ---
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalPosition = transform.position;
        
        // Tạm thời bật Collider thành trigger hoặc tắt đi để không cản trở raycast khi kéo vật thể bay qua các vật khác
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log($"Bắt đầu kéo vật thể: {objectName}");
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // Chuyển đổi tọa độ màn hình của chuột thành tọa độ thế giới (World Position)
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Cập nhật vị trí vật thể theo con trỏ chuột (có thể giữ nguyên độ cao Y)
            transform.position = new Vector3(hit.point.x, 0.05f, hit.point.z);
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // Bật lại Collider sau khi thả
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Debug.Log($"Kết thúc kéo vật thể: {objectName}");
        
        // Tại đây, bạn sẽ viết logic kiểm tra xem vị trí thả mới có hợp lệ không (có nằm trong lưới N*M và ô trống không).
        // Nếu hợp lệ: Cập nhật lại GridPosition mới và snap vào tâm ô lưới.
        // Nếu không hợp lệ: Trả vật thể về vị trí cũ (transform.position = originalPosition).
    }
}