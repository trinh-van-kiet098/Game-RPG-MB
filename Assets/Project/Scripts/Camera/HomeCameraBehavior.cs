using UnityEngine;

public class HomeCameraBehavior : ICameraBehavior
{
    private readonly Camera mainCam;
    private  Transform playerTarget;
    private readonly Vector3 baseOffset;
    private readonly Vector3 eulerRotation;

    private readonly Vector2 minBounds;
    private readonly Vector2 maxBounds;

    // THAY ĐỔI LỚN NHẤT: Lưu điểm đang nhìn dưới đất, không lưu vị trí trên trời nữa
    private Vector3 focusPoint;

    public bool isFollowing { get; private set; } = true;

    private float targetZoomMultiplier = 1f;
    private float currentZoomMultiplier = 1f;
    private const float MIN_ZOOM_MULT = 0.5f;
    private const float MAX_ZOOM_MULT = 2.0f;
    private const float ZOOM_SPEED = 10f;

    public HomeCameraBehavior(Camera cam, Transform playerTarget, Vector3 offset, Vector3 eulerRotation, Vector2 minBounds, Vector2 maxBounds)
    {
        this.mainCam = cam;
        this.playerTarget = playerTarget;
        this.baseOffset = offset;
        this.eulerRotation = eulerRotation;
        this.minBounds = minBounds;
        this.maxBounds = maxBounds;

        if (playerTarget != null)
        {
            this.focusPoint = playerTarget.position;
            ClampFocusPoint();
        }
    }

    private Vector3 GetActualOffset()
    {
        return baseOffset * currentZoomMultiplier;
    }

    public void OnEnter()
    {
        mainCam.transform.rotation = Quaternion.Euler(eulerRotation);

        if (isFollowing && playerTarget != null)
        {
            focusPoint = playerTarget.position;
            ClampFocusPoint();
            // Đặt cam thẳng lên đầu nhân vật ngay khi vào game (chống hiệu ứng bay từ xa tới)
            mainCam.transform.position = focusPoint + GetActualOffset();
        }
        else if (!isFollowing)
        {
            // Tính ngược lại điểm dưới đất dựa trên vị trí hiện tại của Camera
            focusPoint = mainCam.transform.position - GetActualOffset();
            ClampFocusPoint();
        }
    }

    public void HandleZoom(float zoomDelta)
    {
        targetZoomMultiplier -= zoomDelta * 0.1f;
        targetZoomMultiplier = Mathf.Clamp(targetZoomMultiplier, MIN_ZOOM_MULT, MAX_ZOOM_MULT);
        Debug.Log($"[Camera] Hệ số Zoom: {targetZoomMultiplier}");
    }

    public void HandleDrag(Vector2 deltaPosition, float dragSpeed)
    {
        if (isFollowing) isFollowing = false;

        float zoomFactor = currentZoomMultiplier;
        Vector3 right = mainCam.transform.right;
        Vector3 up = mainCam.transform.up;
        right.y = 0; up.y = 0;
        right.Normalize(); up.Normalize();

        Vector3 moveDelta = (-right * deltaPosition.x - up * deltaPosition.y) * dragSpeed * zoomFactor * Time.deltaTime;

        // Vuốt màn hình chính là dời cái điểm Focus Point dưới đất đi chỗ khác
        focusPoint += moveDelta;
        ClampFocusPoint();
    }

    public void HandleTick(float deltaTime)
    {
        // 1. Luôn cập nhật hệ số Zoom
        currentZoomMultiplier = Mathf.Lerp(currentZoomMultiplier, targetZoomMultiplier, ZOOM_SPEED * deltaTime);
        // 2. Cập nhật điểm nhìn dưới đất (nếu đang khóa nhân vật)
        if (isFollowing && playerTarget != null)
        {
            focusPoint = playerTarget.position;
            ClampFocusPoint();
        }

        // 3. TÍNH TOÁN VỊ TRÍ ĐÍCH CỦA CAMERA TRÊN TRỜI = ĐIỂM DƯỚI ĐẤT + OFFSET ZOOM
        Vector3 targetCamPosition = focusPoint + GetActualOffset();

        // 4. Lướt mượt mà tới đó
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetCamPosition, 10f * deltaTime);
    }

    public void LockOnPlayer()
    {
        if (playerTarget == null) return;
        isFollowing = true;
    }
    public void SetPlayerTarget(Transform newTarget)
    {
        this.playerTarget = newTarget;
        Debug.Log($"[CameraManager] Player spawned, setting camera follow target.");
        
        if (this.playerTarget != null)
        {
            this.isFollowing = true; // Bật cờ bám theo lên
            this.focusPoint = this.playerTarget.position; // Lấy tọa độ ngay lập tức
            ClampFocusPoint();
        }
    }

    public void OnExit() { }

    // Hàm Clamp bây giờ đơn giản hơn rất nhiều
    private void ClampFocusPoint()
    {
        focusPoint.x = Mathf.Clamp(focusPoint.x, minBounds.x, maxBounds.x);
        focusPoint.z = Mathf.Clamp(focusPoint.z, minBounds.y, maxBounds.y);
    }
    // private void LateUpdate()
    // {
    //     if (floatingCanvas.gameObject.activeSelf && mainCam != null)
    //     {
    //         // CÁCH CHUẨN NHẤT: Copy y hệt góc xoay (Rotation) của Camera.
    //         // Thanh máu sẽ luôn luôn song song với màn hình người chơi, 
    //         // dù nhìn từ trước, sau, hay trên cao nhìn xuống cũng KHÔNG BAO GIỜ bị lật ngược!
    //         floatingCanvas.transform.rotation = mainCam.transform.rotation;
    //     }
    // }
}