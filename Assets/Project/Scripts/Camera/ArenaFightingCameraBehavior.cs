using UnityEngine;

public class ArenaFightingCameraBehavior : ICameraBehavior
{
    private readonly Camera mainCam;
    private Transform playerTarget;
    private Transform enemyTarget;

    [Header("Camera Positioning")]
    private float heightOffset = 1.5f;      
    private float baseDistance = 3.5f;      
    
    [Header("Dynamic Zoom")]
    private float distanceMultiplier = 0.8f; 
    private float minCamDistance = 4f;       
    private float maxCamDistance = 15f;      

    [Header("Smoothness")]
    private const float MOVE_SPEED = 12f;
    private const float ROTATION_SPEED = 12f;

    public ArenaFightingCameraBehavior(Camera cam, Transform player)
    {
        this.mainCam = cam;
        this.playerTarget = player;
    }

    public void SetEnemyTarget(Transform enemy)
    {
        this.enemyTarget = enemy;
    }

    public void OnEnter() { }

    public void HandleTick(float deltaTime)
    {
        if (playerTarget == null || enemyTarget == null) return;

        Vector3 playerPos = playerTarget.position;
        Vector3 enemyPos = enemyTarget.position;
        
        Vector3 dirToEnemy = enemyPos - playerPos;
        dirToEnemy.y = 0; 
        
        float distanceBetweenFighters = dirToEnemy.magnitude;
        Vector3 lookDirection = dirToEnemy.normalized;

        // Trọng tâm nghiêng về phía Player (35%) để Player ở gần khung hình hơn
        Vector3 centerPoint = Vector3.Lerp(playerPos, enemyPos, 0.35f);

        // TÍNH KHOẢNG CÁCH ZOOM ĐỘNG
        float idealDistance = baseDistance + (distanceBetweenFighters * distanceMultiplier);
        idealDistance = Mathf.Clamp(idealDistance, minCamDistance, maxCamDistance);

        // VỊ TRÍ LÝ TƯỞNG CỦA CAMERA
        Vector3 idealCamPos = centerPoint - (lookDirection * idealDistance) + (Vector3.up * heightOffset);

        // BẮN TIA KIỂM TRA KẸT TƯỜNG (CAMERA COLLISION)
        Vector3 rayStart = playerPos + (Vector3.up * heightOffset);
        Vector3 rayDirection = idealCamPos - rayStart;
        float rayLength = rayDirection.magnitude;

        Vector3 finalCamPos = idealCamPos;
        int wallLayerMask = Physics.DefaultRaycastLayers; 

        if (Physics.Raycast(rayStart, rayDirection.normalized, out RaycastHit hit, rayLength, wallLayerMask))
        {
            // Tránh đâm xuyên qua tường
            finalCamPos = hit.point + (hit.normal * 0.2f);
        }

        // DI CHUYỂN
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, finalCamPos, MOVE_SPEED * deltaTime);

        // XOAY CAMERA
        Vector3 lookTarget = centerPoint + (Vector3.up * heightOffset);
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - mainCam.transform.position);
        mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, targetRotation, ROTATION_SPEED * deltaTime);
    }

    public void HandleDrag(Vector2 deltaPosition, float dragSpeed)
    {
        // Trong chế độ Lock-on, cấm vuốt xoay Camera
    }

    public void HandleZoom(float zoomDelta)
    {
        // Có thể áp dụng zoom thủ công vào baseDistance nếu muốn
    }

    public void SetPlayerTarget(Transform newTarget)
    {
        this.playerTarget = newTarget;
    }

    public void OnExit() { }
}