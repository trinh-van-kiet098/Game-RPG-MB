public interface ICombat
{
    void Configure(UnitController controller);
    void NormalAttack(UnitController target = null); // Đánh thường
    void CastSkill(int skillId, UnitController target = null); // Đánh kỹ năng
    void OnAnimationHitboxActive();
}