
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Game.Events;
public enum InputMode
{
    Gameplay,   // Di chuyển, đánh nhau, rê map bình thường
    Building,   // Chế độ xây nhà (Click để đặt vật thể)
    Chatting,   // Đang gõ Chat (Khóa WASD, JKL, Space, chỉ cho rê chuột)
    Locked      // Khóa toàn bộ (Ví dụ: Lúc đang chuyển scene, xem cắt cảnh)
}


public class InputManager : ConfiguredMonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float dragThreshold = 10f; // Ngưỡng vuốt camera
    [SerializeField] private VirtualJoystickUI virtualJoystick;
    private bool isPointerDown = false;
    private bool cameraSwipeEnabled = true;
    private Vector2 lastPointerPosition;

    public override void Configure(SystemGameManager system)
    {
        base.Configure(system);

        if (virtualJoystick == null && system.UIManager != null)
        {
            virtualJoystick = system.UIManager.GetComponent<VirtualJoystickUI>();
        }

        if (virtualJoystick == null)
        {
            Debug.LogWarning("InputManager chưa tìm thấy VirtualJoystickUI. Hãy gán component vào Inspector hoặc đặt nó cùng GameObject với UIManager.");
        }
    }

    public void SetCameraSwipeEnabled(bool enabled)
    {
        cameraSwipeEnabled = enabled;
        if (!enabled)
        {
            isPointerDown = false;
        }
    }

    private void Update()
    {
        if (systemGameManager == null || systemGameManager.EventManager == null) return;

        HandleMovementInput();
        HandleCameraSwipe();
    }

    private void HandleMovementInput()
    {
        Vector2 moveDirection = Vector2.zero;

        if (virtualJoystick != null && virtualJoystick.Direction.sqrMagnitude > 0.0001f)
        {
            moveDirection = virtualJoystick.Direction;
        }
        else if (Gamepad.current != null)
        {
            moveDirection = Gamepad.current.leftStick.ReadValue();
        }
        // Giữ lại WASD để bạn dễ test trực tiếp trên Unity Editor (không cần build)
        else if (Keyboard.current != null)
        {
            float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
            float y = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
            moveDirection = new Vector2(x, y).normalized;
        }
        systemGameManager.EventManager.NotifyDirectionalInput(new DirectionalInputEvent { Direction = moveDirection });
    }

    // 2. XỬ LÝ VUỐT CAMERA (SWIPE)
    private void HandleCameraSwipe()
    {
        if (!cameraSwipeEnabled) return;
        if (Pointer.current == null) return;

        Vector2 currentPosition = Pointer.current.position.ReadValue();

        // =========================================================================
        // SỬA QUAN TRỌNG: 
        // 1. Nếu ngón tay chạm ở NỬA BÊN TRÁI màn hình (< 50% chiều rộng) -> Dành cho Joystick, KHÔNG CHO XOAY CAM.
        // 2. Hoặc nếu VirtualJoystick đang được gán và người dùng đang chạm vào vùng joystick -> Cấm xoay cam.
        // =========================================================================
        float screenWidth = Screen.width;
        bool isLeftSide = currentPosition.x < screenWidth * 0.5f;

        bool isOverJoystick = false;
        if (systemGameManager != null && systemGameManager.UIManager != null)
        {
            isOverJoystick = systemGameManager.UIManager.IsPointerOverJoystick(currentPosition);
        }

        if (isLeftSide || isOverJoystick)
        {
            isPointerDown = false;
            return; // Dừng ngay, không xử lý vuốt camera!
        }
        // =========================================================================

        bool inputBegan = Pointer.current.press.wasPressedThisFrame;
        bool inputHeld = Pointer.current.press.isPressed;
        bool inputEnded = Pointer.current.press.wasReleasedThisFrame;

        // Lấy ID ngón tay
        int pointerId = Pointer.current is Touchscreen ? ((Touchscreen)Pointer.current).touches[0].touchId.ReadValue() : -1;

        // Kiểm tra uGUI cũ (nếu có)
        if (inputBegan && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId))
        {
            isPointerDown = false;
            return;
        }

        // Bắt đầu vuốt (chỉ xảy ra ở nửa bên phải màn hình)
        if (inputBegan)
        {
            isPointerDown = true;
            lastPointerPosition = currentPosition;
        }
        // Đang vuốt
        else if (isPointerDown && inputHeld)
        {
            Vector2 delta = currentPosition - lastPointerPosition;
            if (delta.sqrMagnitude > dragThreshold * dragThreshold)
            {
                systemGameManager.EventManager.NotifyPointerDrag(new PointerDragEvent { ScreenPosition = currentPosition, DeltaPosition = delta });
                lastPointerPosition = currentPosition;
            }
        }
        // Kết thúc vuốt
        else if (isPointerDown && inputEnded)
        {
            isPointerDown = false;
        }
    }

    // ==========================================
    // 3. XỬ LÝ 4 NÚT SKILL VÀ ĐÁNH THƯỜNG TRÊN UI
    // ==========================================

    // Gắn hàm này vào sự kiện OnClick() của Nút Đánh Thường (Nút C cũ)
    public void OnNormalAttackPressed()
    {
        if (systemGameManager == null) return;
        systemGameManager.EventManager.NotifyNormalAttack(1);
    }

    // Gắn hàm này vào sự kiện OnClick() của các Nút Skill (Truyền số 0, 1, 2 vào Inspector)
    public void OnSkillPressed(int skillIndex)
    {
        if (systemGameManager == null) return;
        systemGameManager.EventManager.NotifyCommandSkill(new CommandSkillEvent { SkillIndex = skillIndex });
    }

    // Gắn hàm này vào sự kiện OnClick() của Nút Nhảy (Nút Space cũ)
    public void OnJumpPressed()
    {
        if (systemGameManager == null) return;
        systemGameManager.EventManager.NotifyJump();
    }
}