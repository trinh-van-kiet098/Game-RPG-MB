using UnityEngine;
// using Unity.Cinemachine; // Tạm tắt nếu bạn không dùng Cinemachine nữa
using Game.Events;

public interface ICameraBehavior
{
    void OnEnter();
    void HandleTick(float deltaTime);
    void HandleDrag(Vector2 deltaPosition, float dragSpeed);
    void HandleZoom(float zoomDelta);
    void OnExit();
    void SetPlayerTarget(Transform playerTransform);
}

public class CameraManager : ConfiguredMonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform playerCharacter;

    [Header("Camera Settings")]
    [SerializeField] private float dragSpeed = 2f;

    private ICameraBehavior currentBehavior;

    private BattleCameraBehavior freeLookLogic;
    private ArenaFightingCameraBehavior lockOnLogic;
    public void SetMainCamera(bool cam)
    {
        mainCam.gameObject.SetActive(cam);
    }
    public override void Configure(SystemGameManager manager)
    {
        base.Configure(manager);

        if (mainCam == null) mainCam = Camera.main;
        if (systemGameManager != null && systemGameManager.EventManager != null)
        {
            systemGameManager.EventManager.OnChangeCameraMode += HandleCameraModeChanged;
            systemGameManager.EventManager.OnPointerDrag += HandleMapDragging;
            systemGameManager.EventManager.OnPointerZoom += HandleZooming;
            systemGameManager.EventManager.OnPlayerSpawnedCameraFollow += HandlePlayerSpawnedCameraFollow;
        }

        freeLookLogic = new BattleCameraBehavior(mainCam, playerCharacter);
        lockOnLogic = new ArenaFightingCameraBehavior(mainCam, playerCharacter);

        SwitchBehavior(freeLookLogic);
    }

    public void LockOnEnemy(Transform enemyTransform)
    {
        lockOnLogic.SetEnemyTarget(enemyTransform);
        SwitchBehavior(lockOnLogic);
    }

    public void UnlockEnemy()
    {
        SwitchBehavior(freeLookLogic);
    }

    private void HandleZooming(PointerZoomEvent evt)
    {
        if (currentBehavior != null) currentBehavior.HandleZoom(evt.ZoomDelta);
    }

    private void HandleMapDragging(PointerDragEvent evt)
    {
        if (currentBehavior != null) currentBehavior.HandleDrag(evt.DeltaPosition, dragSpeed);
    }

    private void HandleCameraModeChanged(ChangeCameraModeEvent evt)
    {
        switch (evt.TargetMode)
        {
            case CameraMode.Home:
                SwitchBehavior(freeLookLogic);
                break;
        }
    }

    private void SwitchBehavior(ICameraBehavior newBehavior)
    {
        if (currentBehavior != null) currentBehavior.OnExit();
        currentBehavior = newBehavior;
        currentBehavior.OnEnter();
    }

    private void LateUpdate()
    {
        if (currentBehavior != null) currentBehavior.HandleTick(Time.deltaTime);
    }

    private void HandlePlayerSpawnedCameraFollow(Transform playerTransform)
    {
        playerCharacter = playerTransform;
        if (freeLookLogic != null) freeLookLogic.SetPlayerTarget(playerTransform);
        if (lockOnLogic != null) lockOnLogic.SetPlayerTarget(playerTransform);
    }

    private void OnDestroy()
    {
        if (systemGameManager != null && systemGameManager.EventManager != null)
        {
            systemGameManager.EventManager.OnChangeCameraMode -= HandleCameraModeChanged;
            systemGameManager.EventManager.OnPointerDrag -= HandleMapDragging;
            systemGameManager.EventManager.OnPointerZoom -= HandleZooming;
            systemGameManager.EventManager.OnPlayerSpawnedCameraFollow -= HandlePlayerSpawnedCameraFollow;
        }
    }
}