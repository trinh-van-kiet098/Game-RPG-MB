using UnityEngine;

public class UnitAnimator
{
    private UnitController unitController;
    private Animator animator;
    private string currentAnimState = "";

    public UnitAnimator(UnitController controller, Animator animatorComponent)
    {
        this.unitController = controller;
        this.animator = animatorComponent;

        if (this.unitController != null && this.unitController.UnitEvent != null)
        {
            this.unitController.UnitEvent.OnMovementStarted += HandleMovementStarted;
            this.unitController.UnitEvent.OnDied += HandleDied;
            this.unitController.UnitEvent.OnTakeDamage += HandleTakeDamage;
            this.unitController.UnitEvent.OnAttackNormal += HandleAttackNormal;
            this.unitController.UnitEvent.OnStopMovement += HandleStopMovement;
            this.unitController.UnitEvent.OnJump += HandleJump;
            // this.unitController.UnitEvent.OnDizzyStop+=;


        }
    }

    // ==========================================
    // HÀM KIỂM TRA XEM CÓ ĐANG BẬN (NHẢY/ĐÁNH) KHÔNG
    // ==========================================
    private bool IsPlayingHighPriorityAction()
    {
        if (animator == null) return false;

        // Lấy thông tin Animation đang thực sự chạy trên Unity
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // NẾU ĐANG NHẢY
        if (currentAnimState == "Jump")
        {
            // normalizedTime là % chạy của Animation (1.0 là chạy xong 100%)
            // Nếu Unity xác nhận đang chạy Jump và chạy chưa xong (< 90%)
            if (stateInfo.IsName("Jump") && stateInfo.normalizedTime < 0.9f) return true;

            // Nếu chạy xong rồi thì nhả cờ ra
            if (stateInfo.IsName("Jump") && stateInfo.normalizedTime >= 0.9f) currentAnimState = "";
        }

        // NẾU ĐANG TẤN CÔNG (Làm tương tự để di chuyển không đứt đoạn combo chém)
        if (currentAnimState.Contains("Attack"))
        {
            if (stateInfo.IsName(currentAnimState) && stateInfo.normalizedTime < 0.9f) return true;
            if (stateInfo.IsName(currentAnimState) && stateInfo.normalizedTime >= 0.9f) currentAnimState = "";
        }

        // Đang bị thương cũng không cho chạy
        if (currentAnimState == "Hurt")
        {
            if (stateInfo.IsName("Hurt") && stateInfo.normalizedTime < 0.9f) return true;
            if (stateInfo.IsName("Hurt") && stateInfo.normalizedTime >= 0.9f) currentAnimState = "";
        }

        return false; // Trả về false nghĩa là RẢNH -> cho phép chạy/đứng im
    }

    // ==========================================

    private void HandleMovementStarted(float speed)
    {
        if (animator == null) return;

        // KHÓA: Nếu đang bận đánh/nhảy thì bỏ qua lệnh di chuyển này
        if (IsPlayingHighPriorityAction()) return;

        if (speed > 0.1f)
        {
            if (currentAnimState != "Run")
            {
                currentAnimState = "Run";
                animator.Play("Run", 0, 0f);
            }
        }
    }

    private void HandleStopMovement()
    {
        // Khi hết choáng, UnitController gọi Stop() -> Cho phép xóa cờ Dizzy để về Ide
        if (currentAnimState == "Dizzy") currentAnimState = "";
        Debug.Log($"Unit {unitController.ID} stopped moving. Current animation state: {currentAnimState}");
        if (IsPlayingHighPriorityAction()) return;

        if (currentAnimState == "Idle") return;
        currentAnimState = "Idle";
        animator.Play("Idle", 0, 0f);
    }

    private void HandleJump()
    {
        // Nhảy là ưu tiên cao nhất, ép Play luôn
        currentAnimState = "Jump";
        animator.Play("Jump", 0, 0f);
    }

    private void HandleAttackNormal(int id)
    {
        // Đánh là ưu tiên cao, ép Play
        string attackAnimName = $"Attack_{id}";
        // Debug.Log(attackAnimName);

        currentAnimState = attackAnimName; // Lưu tên đòn chém hiện tại
        animator.Play(attackAnimName, 0, 0f);
    }

    private void HandleTakeDamage(float damageAmount, int id)
    {
        // BÍ QUYẾT: Nếu đang Dizzy rồi thì KHÔNG Play lại từ đầu (0f) nữa, chỉ trừ máu ngầm thôi
        if (currentAnimState == "Dizzy") return;

        currentAnimState = "Dizzy";
        animator.Play(currentAnimState, 0, 0f);
    }

    private void HandleDied()
    {
        currentAnimState = "Dead";
        if (animator != null) animator.SetBool("IsDead", true);
    }

    public void Release()
    {
        if (unitController != null && unitController.UnitEvent != null)
        {
            unitController.UnitEvent.OnMovementStarted -= HandleMovementStarted;
            unitController.UnitEvent.OnDied -= HandleDied;
            unitController.UnitEvent.OnTakeDamage -= HandleTakeDamage;
            unitController.UnitEvent.OnStopMovement -= HandleStopMovement;
            unitController.UnitEvent.OnJump -= HandleJump;
            unitController.UnitEvent.OnAttackNormal -= HandleAttackNormal;
        }
    }
}