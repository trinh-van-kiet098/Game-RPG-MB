using System;
using System.Dynamic;
using UnityEngine;

public class UnitEventController
{
    // Kênh thông báo: Bị tác động vật lý / Mất máu
    public event Action<float, int> OnTakeDamage = delegate { };

    // Kênh thông báo: Bắt đầu di chuyển (truyền vào tốc độ để Animator xài)
    public event Action<float> OnMovementStarted = delegate { };

    // Kênh thông báo: Chết
    public event Action OnDied = delegate { };

    // Kênh thông báo: Có ai đó vừa click chuột vào tôi (để mở Shop, Chat...)
    public event Action<UnitController> OnInteracted = delegate { };
    public event Action<int> OnAttackNormal = delegate { };
    public event Action OnJump = delegate { };
    public event Action OnStopMovement = delegate { };
    public event Action<Vector3> OnMovement = delegate { };
    public void NotifyMovement(Vector3 vector3) => OnMovement.Invoke(vector3);
    public event Action OnDizzyStop = delegate { };
    public void NotyDizzyStop() => OnDizzyStop.Invoke();
    // --- CÁC NÚT BẤM ĐỂ PHÁT TÍN HIỆU ---
    public void NotifyJump() => OnJump.Invoke();
    public void NotifyTakeDamage(float damageAmount, int id) => OnTakeDamage.Invoke(damageAmount, id);

    public void NotifyMovementStarted(float speed) => OnMovementStarted.Invoke(speed);

    public void NotifyDied() => OnDied.Invoke();

    public void NotifyInteracted(UnitController interactor) => OnInteracted.Invoke(interactor);
    public void NotifyOnAttackNormal(int skillID) => OnAttackNormal.Invoke(skillID);
    public void NotifyStopMovement() => OnStopMovement.Invoke();
}