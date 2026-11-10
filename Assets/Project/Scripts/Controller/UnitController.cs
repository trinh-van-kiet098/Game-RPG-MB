using UnityEngine;
using Game.Events;
using System.Collections.Generic;
using Unity.VisualScripting;
public enum UnitType
{
    Player,
    Monster,
    NPC,
    ProxyPlayer
}
public struct InfoCharacter
{
    float dame;
    float heal;
}
public class UnitController : InteractableBase
{
    [Header("Liên kết Dữ Liệu JSON")]
    [Tooltip("ID của nhân vật này trong file CharacterData.json")]
    [SerializeField] private int configCharId;

    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public UnitType Type { get; private set; }
    public bool IsLocalPlayer { get; private set; }
    private int lastAttackerId = -1;
    public bool IsAlive = true;

    public UnitEventController UnitEvent { get; private set; }

    public UnitMotor Motor { get; private set; }
    public UnitAnimator UnitAnimator { get; private set; }
    public CharacterStats Stats { get; private set; }
    public ICombat Combat { get; private set; }
    public CharacterProfile CharacterProfile { get; private set; }



    [SerializeField] private Animator unityAnimator;
    private IBrainBot brainBot;
    [Header("Combat Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 2;
    [SerializeField] private LayerMask enemyLayer;

    public float heal = 100f;
    public bool isDizzy = false;
    private float dizzyTimer = 0f;
    public override void Configure(SystemGameManager system)
    {
        base.Configure(system);

        UnitEvent = new UnitEventController();
        Combat = new CharacterCombat(attackPoint, attackRange, enemyLayer);

        Motor = GetComponent<UnitMotor>();
        Stats = new CharacterStats(this, this.configCharId);

        if (unityAnimator == null) unityAnimator = GetComponent<Animator>();
        UnitAnimator = new UnitAnimator(this, unityAnimator);


        if (Motor != null) Motor.Configure(this);
        UnitEvent.OnTakeDamage += HandleTakeAttack;
        if (!IsLocalPlayer)
        {
            brainBot = new BrainBot();
            brainBot.Configure(this, systemGameManager);
        }
        // if (Stats != null)
        // {
        //     Debug.Log("Max HP: " + Stats.enemyConfig.maxHp);
        // }
    }

    public void InitializeUnit(int uuid, UnitType type, bool isLocalPlayer)
    {
        this.ID = uuid;
        this.Type = type;
        this.IsLocalPlayer = isLocalPlayer;
        ///neu chuyen qua game server thi khi co chi can lang nghe su kien co kenh lap dung id la duoc
        if (this.IsLocalPlayer)
        {
            SystemGameManager.Instance.EventManager.OnDirectionalInput += HandleDirectionalInput;
            // SystemGameManager.Instance.EventManager.OnStopMovement +=Han
            SystemGameManager.Instance.EventManager.NotifyUnitSpawned(new UnitSpawnedEvent
            {
                UnitId = this.ID,
                IsLocalPlayer = this.IsLocalPlayer
            });
            SystemGameManager.Instance.EventManager.OnCommandSkill += HandleSkillInput;
            SystemGameManager.Instance.EventManager.OnNormalAttack += HandleNormalAttack;
            SystemGameManager.Instance.EventManager.OnJump += HandleJump;

        }

        else
        {
            SystemGameManager.Instance.EventManager.OnDirectionalInputForBot += HandleDirectionalInput;
            SystemGameManager.Instance.EventManager.OnNormalAttackForBot += HandleNormalAttack;

        }
    }

    public void SetCharacterProfile(CharacterProfile profile)
    {
        if (profile == null)
        {
            Debug.LogError("Không thể gán hồ sơ nhân vật null cho UnitController.");
            return;
        }

        CharacterProfile = profile;
    }

    public void TickUpdate(float deltaTime)
    {
        // Xử lý đếm ngược thời gian choáng
        if (this.isDizzy)
        {

            dizzyTimer -= deltaTime;
            if (dizzyTimer <= 0)
            {
                ChangeDizzy(false);
            }
            else
            {
                return;
            }
        }

        if (Motor != null) Motor.TickUpdate(deltaTime);

        if (!IsLocalPlayer && brainBot != null) brainBot.TickUpdate(deltaTime);
    }

    public override void Interact(GameObject interactorGO)
    {
        if (!IsAlive) return;

        UnitController interactorUnit = interactorGO != null ? interactorGO.GetComponent<UnitController>() : null;
        UnitEvent.NotifyInteracted(interactorUnit);
    }

    public void MoveFromInput(Vector3 direction)
    {
        Motor.Move(direction);
        UnitEvent.NotifyMovementStarted(direction.magnitude);
    }

    public void ApplyNetworkMovement(Vector3 targetPosition, float speed)
    {
        if (!IsAlive) return;

        Motor.MoveToNetworkPosition(targetPosition, speed);
        UnitEvent.NotifyMovementStarted(speed);
    }

    public void Die()
    {
        if (!IsAlive) return;
        IsAlive = false;

        ChangeDizzy(false);
        if (Motor != null) Motor.StopMovement();
        UnitEvent.NotifyDied();
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        if (SystemGameManager.Instance.EventManager != null)
        {
            SystemGameManager.Instance.EventManager.NotifyUnitDied(new UnitDiedGlobalEvent
            {
                DeadUnitId = this.ID,
                DeathPosition = transform.position,
                KillerUnitId = lastAttackerId
            });
        }
    }
    public void ChangeDizzy(bool ap, float duration = 0f)
    {
        this.isDizzy = ap;
        if (ap)
        {
            this.dizzyTimer = duration;
            DizzyStop();
        }
        else
        {
            this.dizzyTimer = 0f;
            Stop();
        }
    }

    private bool isMoving = false;

    private void HandleDirectionalInput(DirectionalInputEvent evt)
    {
        Vector2 input2D = evt.Direction;

        if (input2D.sqrMagnitude > 0.01f)
        {
            isMoving = true; // Đánh dấu là đang di chuyển (BẠN THIẾU DÒNG NÀY)

            // Lấy hướng Camera hiện tại để bẻ hướng di chuyển cho khớp với góc nhìn
            Vector3 camForward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;
            Vector3 camRight = Camera.main != null ? Camera.main.transform.right : Vector3.right;

            camForward.y = 0;
            camForward.Normalize();

            camRight.y = 0;
            camRight.Normalize();

            // Tính vector di chuyển dựa trên góc nhìn của Camera
            Vector3 moveDir = (camForward * input2D.y) + (camRight * input2D.x);
            // MoveFromInput(moveDir.normalized);
            this.UnitEvent.NotifyMovement(moveDir);
            UnitEvent.NotifyMovementStarted(moveDir.magnitude);
        }
        else
        {
            // CHỈ GỌI STOP NẾU TRƯỚC ĐÓ ĐANG DI CHUYỂN (BẠN THIẾU CÁI IF NÀY)
            if (isMoving)
            {
                isMoving = false; // Đánh dấu là đã dừng lại
                Stop();
            }
        }
    }
    public void Stop()
    {
        UnitEvent.NotifyStopMovement();
    }
    public void DizzyStop()
    {
        UnitEvent.NotyDizzyStop();
    }
    public void HandleSkillInput(CommandSkillEvent evt)
    {

        if (Combat != null)
        {
            Debug.Log($"Unit {ID} casting skill with index: {evt.SkillIndex}");
            Combat.CastSkill(evt.SkillIndex);
        }
    }
    public void HandleNormalAttack(int ID)
    {
        UnitEvent.NotifyOnAttackNormal(ID);

        // Combat.NormalAttack(this);
    }
    public void OnAttackActive()
    {
        Combat.OnAnimationHitboxActive();
    }
    private void HandleJump()
    {
        UnitEvent.NotifyJump();
    }












    private void OnDisable()
    {
        if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
        {
            SystemGameManager.Instance.EventManager.OnDirectionalInput -= HandleDirectionalInput;
            SystemGameManager.Instance.EventManager.OnDirectionalInputForBot -= HandleDirectionalInput;
            SystemGameManager.Instance.EventManager.OnCommandSkill -= HandleSkillInput;
            SystemGameManager.Instance.EventManager.OnJump -= HandleJump;
            SystemGameManager.Instance.EventManager.OnNormalAttack -= HandleNormalAttack;
            SystemGameManager.Instance.EventManager.OnNormalAttackForBot -= HandleNormalAttack;

        }
    }
    private void OnDestroy()
    {
        if (UnitAnimator != null)
        {
            UnitAnimator.Release();
        }

        // 🔴 ĐÚNG NHƯ BÁC ĐOÁN: PHẢI DỌN SẠCH "NÃO" CỦA BOT CŨ
        if (brainBot != null)
        {
            // Nếu file BrainBot.cs của bác có viết hàm Release() hoặc Dispose(), hãy mở comment dòng bên dưới:
            // brainBot.Release(); 

            brainBot = null;
        }
    }

    private void HandleTakeAttack(float dame, int id)
    {
        if (!IsAlive) return;
        lastAttackerId = id;
        if (Stats != null)
        {
            Stats.currentHP -= dame;
            if (Stats.currentHP < 0) Stats.currentHP = 0;
            this.heal = Stats.currentHP;
        }
        else
        {
            this.heal -= dame;
        }
        if (this.heal <= 0)
        {
            Die();
        }
        else
        {
            ChangeDizzy(true, 1.5f);
        }
    }





    private void OnDrawGizmosSelected()
    {
        // Kiểm tra nếu biến chưa được kéo thả trong Inspector thì bỏ qua
        if (attackPoint == null) return;

        // Vẽ một vòng tròn màu đỏ quanh điểm đánh để dễ canh chỉnh tầm đánh
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void EndAnimation()
    {
        SystemGameManager.Instance.EventManager.NotifyEndAnimation();
    }
}