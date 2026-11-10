using UnityEngine;
using UnityEngine.UIElements;

public class VirtualJoystickUI : MonoBehaviour
{
    private VisualElement joystickContainer;
    private VisualElement baseElement;
    private VisualElement handleElement;

    private bool isDragging = false;
    private Vector2 joystickDir = Vector2.zero;
    private float maxRadius = 45f; // Bán kính tối đa của joystick

    public Vector2 Direction => joystickDir;

    public bool IsPointerOver(Vector2 screenPosition)
    {
        VisualElement hitArea = joystickContainer != null ? joystickContainer : baseElement;
        if (hitArea == null || hitArea.panel == null) return false;

        Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(hitArea.panel, screenPosition);
        bool isOver = hitArea.worldBound.Contains(panelPosition);

        // --- IN RA ĐỂ KIỂM TRA ---
        if (isOver)
        {
            Debug.Log($"<color=green>Đang chạm vào Joystick! ScreenPos: {screenPosition} | PanelPos: {panelPosition} | HitArea Bound: {hitArea.worldBound}</color>");
        }

        return isOver;
    }

    public void Initialize(VisualElement root)
    {
        joystickContainer = root.Q<VisualElement>("joystick-container");
        baseElement = root.Q<VisualElement>("joystick-base");
        handleElement = root.Q<VisualElement>("joystick-handle");

        if (baseElement == null || handleElement == null)
        {
            Debug.LogError("Không tìm thấy joystick-base hoặc joystick-handle trong UXML!");
            return;
        }

        // Hủy đăng ký cũ trước (nếu có) để tránh bị nhân đôi sự kiện khi mở lại GameHUD
        baseElement.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        baseElement.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        baseElement.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        baseElement.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);

        // Đăng ký mới
        baseElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
        baseElement.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        baseElement.RegisterCallback<PointerUpEvent>(OnPointerUp);
        baseElement.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        isDragging = true;
        baseElement.CapturePointer(evt.pointerId);
        CalculateJoystick(evt.localPosition);
        evt.StopPropagation();
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (!isDragging) return;
        CalculateJoystick(evt.localPosition);
        evt.StopPropagation();
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (!isDragging) return;
        baseElement.ReleasePointer(evt.pointerId);
        ResetJoystick();
        evt.StopPropagation();
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
    {
        if (!isDragging) return;
        ResetJoystick();
    }

    private void ResetJoystick()
    {
        isDragging = false;
        handleElement.style.translate = new Translate(0, 0, 0);
        joystickDir = Vector2.zero;
    }

    private void CalculateJoystick(Vector2 localPos)
    {
        Vector2 center = new Vector2(baseElement.resolvedStyle.width / 2f, baseElement.resolvedStyle.height / 2f);
        Vector2 offset = localPos - center;

        float distance = offset.magnitude;
        if (distance > maxRadius)
        {
            offset = offset.normalized * maxRadius;
        }

        handleElement.style.translate = new Translate(offset.x, offset.y, 0);

        if (maxRadius > 0)
        {
            joystickDir = offset / maxRadius;
            joystickDir.y = -joystickDir.y;
        }
    }
}