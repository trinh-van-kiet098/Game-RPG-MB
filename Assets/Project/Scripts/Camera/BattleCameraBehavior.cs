using UnityEngine;

public class BattleCameraBehavior : ICameraBehavior
{
    private readonly Camera mainCam;
    private Transform playerTarget;

    [Header("Orbit Settings")]
    private float targetRadius = 6f; 
    private float currentRadius = 6f;
    private const float MIN_RADIUS = 2f;
    private const float MAX_RADIUS = 12f;

    [Header("Rotation Settings")]
    private float currentYaw = 0f;    
    private float currentPitch = 15f; 
    private const float MIN_PITCH = -10f; 
    private const float MAX_PITCH = 60f;  
    private const float ROTATION_SPEED = 0.5f; 

    [Header("Follow Settings")]
    private const float LERP_SPEED = 15f;
    private readonly Vector3 heightOffset = new Vector3(0, 1.5f, 0); 

    public BattleCameraBehavior(Camera cam, Transform player, float initialRadius = 6f)
    {
        this.mainCam = cam;
        this.playerTarget = player;
        this.targetRadius = initialRadius;
        this.currentRadius = initialRadius;

        if (player != null)
        {
            currentYaw = player.eulerAngles.y;
        }
    }

    public void OnEnter()
    {
        // Khi vừa chuyển sang, lấy góc xoay hiện tại của Camera để không giật khung hình
        currentYaw = mainCam.transform.eulerAngles.y;
        currentPitch = mainCam.transform.eulerAngles.x;
        // Fix trường hợp Pitch trả về 300~360 độ thay vì số âm
        if (currentPitch > 180f) currentPitch -= 360f;
    }

    public void HandleTick(float deltaTime)
    {
        if (playerTarget == null) return;

        currentRadius = Mathf.Lerp(currentRadius, targetRadius, LERP_SPEED * deltaTime);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 focusPoint = playerTarget.position + heightOffset;
        Vector3 targetDirection = rotation * new Vector3(0, 0, -currentRadius);
        
        Vector3 idealCamPos = focusPoint + targetDirection;

        // CHECK TƯỜNG (CAMERA COLLISION)
        int wallLayerMask = Physics.DefaultRaycastLayers; // Tốt nhất nên đổi thành Layer của Tường/Mặt đất
        Vector3 finalCamPos = idealCamPos;
        
        if (Physics.Raycast(focusPoint, (idealCamPos - focusPoint).normalized, out RaycastHit hit, currentRadius, wallLayerMask))
        {
            finalCamPos = hit.point + (hit.normal * 0.1f); // Kéo cam sát tường
        }

        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, finalCamPos, LERP_SPEED * deltaTime);
        mainCam.transform.LookAt(focusPoint);
    }

    public void HandleDrag(Vector2 deltaPosition, float dragSpeed)
    {
        currentYaw += deltaPosition.x * ROTATION_SPEED * dragSpeed;
        currentPitch -= deltaPosition.y * ROTATION_SPEED * dragSpeed; 
        currentPitch = Mathf.Clamp(currentPitch, MIN_PITCH, MAX_PITCH);
    }

    public void HandleZoom(float zoomDelta)
    {
        targetRadius -= zoomDelta * 0.1f;
        targetRadius = Mathf.Clamp(targetRadius, MIN_RADIUS, MAX_RADIUS);
    }

    public void SetPlayerTarget(Transform newTarget)
    {
        this.playerTarget = newTarget;
        if (this.playerTarget != null) currentYaw = this.playerTarget.eulerAngles.y;
    }

    public void OnExit() { }
}