using UnityEngine;
using UnityEngine.Rendering;

public class CharacterStats
{
    public EnemyConfig enemyConfig;
    private UnitController unitController;
    public float currentHP;
    public float currentMana;
    public float currentDameBase;
    public float currentSpeed;
    // public float currentArmor;

    // Danh sách chiêu thức của nhân vật này
    public CharacterStats()
    {
        enemyConfig = null;
    }
    public CharacterStats(UnitController unit, int id)
    {
        unitController = unit;

        // KIỂM TRA NULL ĐỂ CHỐNG CRASH
        if (enemyConfig != null)
        {
            currentHP = enemyConfig.maxHp;
            currentDameBase = enemyConfig.baseDamage;
        }
        else
        {
            // Báo lỗi đỏ lên Console để bạn biết ID nào đang bị thiếu data

            // Set thông số mặc định để game vẫn chạy được tiếp
            currentHP = 100f;
            currentDameBase = 10f;
        }

        if (unitController != null)
        {
            unitController.UnitEvent.OnTakeDamage += HandleTakeDame;
        }
    }
    public void HandleTakeDame(float dame, int id)
    {
        // currentHP -= dame;
    }
}