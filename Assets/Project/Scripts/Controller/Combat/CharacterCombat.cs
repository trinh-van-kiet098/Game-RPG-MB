using UnityEngine;
using System.Collections.Generic;

public class CharacterCombat : ICombat
{
    private UnitController unitController;
    private int id;

    // Đổi thành 1 Transform duy nhất thay vì mảng Transform[]
    private Transform attackPoint;
    private float attackRange;
    private LayerMask enemyLayer;

    // --- HỆ THỐNG COMBO ---
    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private float comboWindow = 1.2f; // Có 1.2 giây để bấm nút nối combo

    // Lưu những kẻ địch đã chém trúng trong 1 nhịp chém
    private HashSet<Collider> alreadyHitEnemies = new HashSet<Collider>();

    // Constructor cập nhật lại nhận 1 Transform
    public CharacterCombat(Transform attackPoint, float attackRange, LayerMask enemyLayer)
    {
        this.attackPoint = attackPoint;
        this.attackRange = attackRange;
        this.enemyLayer = enemyLayer;
    }

    public void Configure(UnitController controller)
    {
        this.unitController = controller;
        id = controller.ID;

        // Lắng nghe sự kiện đánh từ UnitEvent
        this.unitController.UnitEvent.OnAttackNormal += TriggerNormalAttack;
    }

    private void TriggerNormalAttack(int skillID)
    {
        NormalAttack();
    }

    // Hàm này chạy ngay khoảnh khắc bạn BẤM NÚT ĐÁNH
    public void NormalAttack(UnitController target = null)
    {
        // QUAN TRỌNG NHẤT LÀ ĐÂY: Phải xóa trí nhớ của cú chém trước, nếu không chém nhát thứ 2 sẽ bị xịt!
        // alreadyHitEnemies.Clear();

        // Logic combo
        if (Time.time - lastAttackTime > comboWindow)
        {
            comboStep = 0;
        }

        comboStep++;
        if (comboStep > 3) comboStep = 1;

        lastAttackTime = Time.time;

        // Debug.Log($"Bắt đầu vung vũ khí: Đòn đánh số {comboStep}");
    }

    // HÀM NÀY ĐƯỢC GỌI TỪ ANIMATION EVENT (Đúng lúc lưỡi kiếm chém tới)
    public void OnAnimationHitboxActive()
    {
        if (attackPoint == null) return;
        alreadyHitEnemies.Clear();
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            if (!alreadyHitEnemies.Contains(enemy))
            {
                alreadyHitEnemies.Add(enemy);

                // 1. TÚM CỔ KẺ ĐỊCH THÔNG QUA COLLIDER
                if (enemy.TryGetComponent<UnitController>(out UnitController targetUnit))
                {
                    // 2. GỌI TRỰC TIẾP VÀO UNITEVENT CỦA KẺ ĐỊCH (Nội bộ con quái tự xử lý)
                    float damageAmount = 1000f; // Sát thương giả định

                    // Phát tín hiệu mất máu / bị choáng cho con quái
                    targetUnit.UnitEvent.NotifyTakeDamage(damageAmount, id);
                    SystemGameManager.Instance.EventManager.NotifyEnemyTargeted(targetUnit);

                    // // NẾU CÓ HIỆU ỨNG ĐẨY LÙI (Knockback), LÀM Ở ĐÂY LÀ CHUẨN NHẤT, tùy trường hợp gọi trực tiếp hoặc gọi thông qua sự kiện event 
                    Rigidbody targetRb = enemy.GetComponent<Rigidbody>();
                    targetRb.AddForce(targetRb.transform.forward * -5f, ForceMode.Impulse);
                }
            }
        }
    }

    public void CastSkill(int skillId, UnitController target = null)
    {
        if (skillId == 3) NormalAttack();
    }
}