// using UnityEngine;

// public enum BotState
// {
//     Idle,
//     Chase,
//     Attack,
//     Defend
// }

// public class BrainBot : IBrainBot
// {
//     private UnitController unitController;
//     private SystemGameManager systemGameManager;
//     private Transform playerTarget; // Cần biết Player đang ở đâu

//     private BotState currentState = BotState.Idle;

//     // Thông số AI
//     private float attackDistance = 2.5f; // Tầm đánh
//     private float chaseDistance = 15f;   // Tầm nhìn thấy Player
//     private float reactionTime = 0.2f;   // Tốc độ phản xạ (0.2s là rất nhanh)
//     private float lastThinkTime;

//     public void Configure(UnitController unit, SystemGameManager system)
//     {
//         this.systemGameManager = system;
//         this.unitController = unit;

//         // Tìm Player (Tùy cấu trúc game của bạn, đây là ví dụ)
//         GameObject player = GameObject.FindGameObjectWithTag("Player");
//         if (player != null) playerTarget = player.transform;
//     }

//     public void TickUpdate(float deltaTime)
//     {
//         if (playerTarget == null || !unitController.IsAlive) return;

//         // Cứ mỗi 0.2 giây (reactionTime), Bot sẽ "Suy nghĩ" lại 1 lần để tránh nặng máy và làm chuyển động mượt hơn
//         if (Time.time - lastThinkTime >= reactionTime)
//         {
//             ThinkAndDecide();
//             lastThinkTime = Time.time;
//         }

//         // Thực thi hành động dựa trên State hiện tại
//         ExecuteCurrentState();
//     }

//     private void ThinkAndDecide()
//     {
//         float distanceToPlayer = Vector3.Distance(unitController.transform.position, playerTarget.position);

//         // 1. Logic Phủ (Defend): Nếu Player đang trong tầm đánh VÀ Player đang xuất chiêu -> Tỷ lệ cao Bot sẽ Thủ
//         UnitController playerUnit = playerTarget.GetComponent<UnitController>();
//         bool isPlayerAttacking = false; // Lấy từ playerUnit (vd: playerUnit.Combat.IsAttacking)

//         if (distanceToPlayer <= attackDistance && isPlayerAttacking)
//         {
//             // Tỷ lệ 70% thủ thành công nếu địch đánh
//             if (Random.value < 0.7f) 
//             {
//                 currentState = BotState.Defend;
//                 return;
//             }
//         }

//         // 2. Logic Tấn Công / Áp Sát
//         if (distanceToPlayer <= attackDistance)
//         {
//             currentState = BotState.Attack;
//         }
//         else if (distanceToPlayer <= chaseDistance)
//         {
//             currentState = BotState.Chase;
//         }
//         else
//         {
//             currentState = BotState.Idle;
//         }
//     }

//     private void ExecuteCurrentState()
//     {
//         switch (currentState)
//         {
//             case BotState.Chase:
//                 // Tìm hướng lao về phía Player
//                 Vector3 dir = (playerTarget.position - unitController.transform.position).normalized;
//                 systemGameManager.EventManager.NotifyDirectionalInputForBot(new DirectionalInputEvent
//                 {
//                     Direction = new Vector2(dir.x, dir.z)
//                 });
//                 break;

//             case BotState.Attack:
//                 // Dừng lại để đánh
//                 systemGameManager.EventManager.NotifyDirectionalInputForBot(new DirectionalInputEvent { Direction = Vector2.zero });

//                 // Gọi Event đánh thường (Nếu có hệ thống Combo, cứ gọi liên tục, hàm Combat sẽ tự nối combo)
//                 // this.unitController.UnitEvent.NotifyOnAttackNormal(1);
//                 systemGameManager.EventManager.NotifyNormalAttackForBot(1);

//                 break;

//             case BotState.Defend:
//                 // Dừng lại và bật khiên
//                 systemGameManager.EventManager.NotifyDirectionalInputForBot(new DirectionalInputEvent { Direction = Vector2.zero });
//                 // unitController.UnitAnimator.PlayAnimation("Block");
//                 // Giảm sát thương nhận vào
//                 break;

//             case BotState.Idle:
//                 systemGameManager.EventManager.NotifyDirectionalInputForBot(new DirectionalInputEvent { Direction = Vector2.zero });
//                 break;
//         }
//     }
// }
using UnityEngine;

public enum BotState
{
    None, // Thêm State này để ép Bot phải chạy lệnh khởi tạo khi vừa vào game
    Idle,
    Chase,
    Attack,
    Defend
}

public class BrainBot : IBrainBot
{
    private UnitController unitController;
    private SystemGameManager systemGameManager;
    private Transform playerTarget; // Cần biết Player đang ở đâu

    private BotState currentState = BotState.None;

    // --- Thông số AI ---
    private float attackDistance = 2.5f; // Tầm đánh
    private float chaseDistance = 15f;   // Tầm nhìn thấy Player
    private float reactionTime = 0.2f;   // Tốc độ phản xạ (0.2s não load 1 lần)
    private float lastThinkTime;

    // --- Khóa chống spam (QUAN TRỌNG) ---
    private float attackCooldown = 3f; // Giãn cách giữa 2 lần chém (1.5 giây)
    private float lastAttackTime;

    public void Configure(UnitController unit, SystemGameManager system)
    {
        this.systemGameManager = system;
        this.unitController = unit;

        // Tìm Player (Tùy cấu trúc game của bạn, đây là ví dụ)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;

        // Ép não bộ khởi động vào trạng thái Idle
        ChangeState(BotState.Idle);
    }

    public void TickUpdate(float deltaTime)
    {
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform; // Đã tìm thấy!
            }
            else
            {
                return; // Nếu vẫn chưa thấy, thoát frame này, frame sau tìm tiếp
            }
        }

        // 2. Chết rồi thì miễn suy nghĩ
        if (!unitController.IsAlive) return;

        // Cứ mỗi 0.2 giây (reactionTime), Bot sẽ "Suy nghĩ" lại 1 lần
        if (Time.time - lastThinkTime >= reactionTime)
        {
            ThinkAndDecide();
            lastThinkTime = Time.time;
        }

        // Thực thi hành động dựa trên State hiện tại
        ExecuteCurrentState();
    }

    private void ThinkAndDecide()
    {
        // Debug.Log("Bot so " + unitController.ID + " Dang suy nghĩ");
        float distanceToPlayer = Vector3.Distance(unitController.transform.position, playerTarget.position);

        // 1. Logic Thủ (Defend)
        UnitController playerUnit = playerTarget.GetComponent<UnitController>();
        bool isPlayerAttacking = false; // Gắn logic check player đang đánh ở đây sau

        if (distanceToPlayer <= attackDistance && isPlayerAttacking)
        {
            if (Random.value < 0.7f)
            {
                ChangeState(BotState.Defend);
                return;
            }
        }

        // 2. Logic Tấn Công / Áp Sát / Đứng im
        if (distanceToPlayer <= attackDistance)
        {
            ChangeState(BotState.Attack);
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            ChangeState(BotState.Chase);
        }
        else
        {
            ChangeState(BotState.Idle);
        }
    }

    // =========================================================
    // NGƯỜI GÁC CỔNG: CHỈ CHO PHÉP CHẠY 1 LẦN DUY NHẤT KHI ĐỔI TRẠNG THÁI
    // =========================================================
    private void ChangeState(BotState newState)
    {
        // Nếu quyết định mới giống hệt cái cũ -> Không làm gì cả, chống spam!
        if (currentState == newState) return;

        currentState = newState;

        // Chỉ gửi tín hiệu Stop đúng 1 lần khi chuyển sang các trạng thái đứng im
        switch (currentState)
        {
            case BotState.Idle:
            case BotState.Attack:
            case BotState.Defend:
                // GỌI TRỰC TIẾP VÀO CƠ THỂ BOT, KHÔNG DÙNG SYSTEM GAME MANAGER NỮA
                unitController.Stop();
                break;
            case BotState.Chase:
                // Lúc bắt đầu chạy thì không cần làm gì, ExecuteCurrentState sẽ lo việc truyền hướng
                break;
        }
    }

    // =========================================================
    // HÀM THỰC THI LIÊN TỤC (60 FRAME/GIÂY)
    // =========================================================
    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case BotState.Chase:
                // Liên tục tính toán hướng 3D lao về phía Player
                Vector3 dir = playerTarget.position - unitController.transform.position;
                dir.y = 0; // Tránh việc Bot cắm mặt xuống đất hay ngước lên trời

                unitController.MoveFromInput(dir.normalized);
                break;

            case BotState.Attack:
                // Không gửi lệnh dừng ở đây nữa vì đã gửi bên ChangeState rồi

                // Khóa hồi chiêu: Chỉ đánh khi đã nghỉ đủ 1.5s
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    // PHÁT TÍN HIỆU ĐÁNH QUA KÊNH UNITEVENT (NỘI BỘ), KHÔNG DÙNG GLOBAL EVENT
                    unitController.UnitEvent.NotifyOnAttackNormal(1);

                    lastAttackTime = Time.time;
                }
                break;

            case BotState.Defend:
                // Gọi Event bật khiên ở đây
                break;

            case BotState.Idle:
                // Đứng im không cần gửi lệnh gì thêm vì đã gọi Stop() lúc đổi State rồi
                break;
        }
    }
}