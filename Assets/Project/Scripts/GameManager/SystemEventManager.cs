using System;
using UnityEngine;
using Game.Events;
public class SystemEventManager
{
    // ==========================================
    // NHÓM 1: SỰ KIỆN THỰC THỂ (Entity / Combat)
    // ==========================================
    public event Action<UnitSpawnedEvent> OnUnitSpawned = delegate { };
    public event Action<UnitDiedGlobalEvent> OnUnitDied = delegate { };

    public void NotifyUnitSpawned(UnitSpawnedEvent evt) => OnUnitSpawned.Invoke(evt);
    public void NotifyUnitDied(UnitDiedGlobalEvent evt) => OnUnitDied.Invoke(evt);
    public event Action<Transform> OnPlayerSpawnedCameraFollow = delegate { };
    public void NotifyPlayerSpawnedCameraFollow(Transform playerTransform) => OnPlayerSpawnedCameraFollow.Invoke(playerTransform);

    // ==========================================
    // NHÓM 2: SỰ KIỆN NGƯỜI CHƠI (Player / Account)
    // ==========================================
    public event Action<PlayerLevelUpEvent> OnPlayerLevelUp = delegate { };
    public event Action<CurrencyChangedEvent> OnCurrencyChanged = delegate { }; // Vàng, Kim cương thay đổi

    public void NotifyPlayerLevelUp(PlayerLevelUpEvent evt) => OnPlayerLevelUp.Invoke(evt);
    public void NotifyCurrencyChanged(CurrencyChangedEvent evt) => OnCurrencyChanged.Invoke(evt);


    // ==========================================
    // NHÓM 3: SỰ KIỆN HỆ THỐNG & MÔI TRƯỜNG (System / Map)
    // ==========================================
    // public event Action<MapScenarioSO> OnMapLoaded = delegate { };
    public event Action<SessionStateChangedEvent> OnSessionStateChanged = delegate { };
    public event Action<RequestJoinMapEvent> OnRequestJoinMap = delegate { };
    // public void NotifyMapLoaded(MapScenarioSO evt) => OnMapLoaded.Invoke(evt);
    public void NotifySessionStateChanged(SessionStateChangedEvent evt) => OnSessionStateChanged.Invoke(evt);
    public void NotifyRequestJoinMap(RequestJoinMapEvent evt) => OnRequestJoinMap.Invoke(evt);


    // ==========================================
    // NHÓM 4: SỰ KIỆN GIAO DIỆN & TƯƠNG TÁC (UI / Interact)
    // Dùng khi Player bấm vào NPC, rơi đồ, bật Menu
    // ==========================================
    public event Action<OpenDialogUIEvent> OnOpenDialogUI = delegate { };
    public event Action<ShowFloatingTextEvent> OnShowFloatingText = delegate { }; // Hiện chữ máu bay lên

    public void NotifyOpenDialogUI(OpenDialogUIEvent evt) => OnOpenDialogUI.Invoke(evt);
    public void NotifyShowFloatingText(ShowFloatingTextEvent evt) => OnShowFloatingText.Invoke(evt);
    ///Camera change
    public event Action<ChangeCameraModeEvent> OnChangeCameraMode;

    public void NotifyChangeCameraMode(ChangeCameraModeEvent evt)
    {
        OnChangeCameraMode?.Invoke(evt);
    }
    // ==========================================
    // NHÓM 5: SỰ KIỆN ĐIỀU KHIỂN (Input - Đa nền tảng)
    // Dùng chung cho cả Click Chuột (PC) và Chạm (Mobile)
    // ==========================================
    public event Action<PointerDownEvent> OnPointerDown = delegate { };
    public event Action<PointerDragEvent> OnPointerDrag = delegate { };
    public event Action<PointerUpEvent> OnPointerUp = delegate { };

    public void NotifyPointerDown(PointerDownEvent evt) => OnPointerDown.Invoke(evt);
    public void NotifyPointerDrag(PointerDragEvent evt) => OnPointerDrag.Invoke(evt);
    public void NotifyPointerUp(PointerUpEvent evt) => OnPointerUp.Invoke(evt);
    public event Action<PointerZoomEvent> OnPointerZoom = delegate { };
    public void NotifyPointerZoom(PointerZoomEvent evt) => OnPointerZoom.Invoke(evt);
    // ==========================================
    // NHÓM 6: SỰ KIỆN DI CHUYỂN (WASD, Joystick, Click)
    // ==========================================
    public event Action<DirectionalInputEvent> OnDirectionalInput = delegate { };
    public event Action<CommandMoveToEvent> OnCommandMoveTo = delegate { };
    public event Action OnEndAnimation = delegate { };
    public event Action OnJump = delegate { };
    public void NotifyJump() => OnJump.Invoke();
    public void NotifyEndAnimation() => OnEndAnimation.Invoke();

    public void NotifyDirectionalInput(DirectionalInputEvent evt) => OnDirectionalInput.Invoke(evt);
    public void NotifyCommandMoveTo(CommandMoveToEvent evt) => OnCommandMoveTo.Invoke(evt);

    public event Action OnStopMovement = delegate { };
    public void NotifyStopMovement() => OnStopMovement.Invoke();
    public event Action OnCommandCenterCamera = delegate { };
    public void NotifyCenterCamera() => OnCommandCenterCamera.Invoke();
    // Thêm vào NHÓM 6: SỰ KIỆN ĐIỀU KHIỂN
    public event Action<CommandSkillEvent> OnCommandSkill = delegate { };
    public void NotifyCommandSkill(CommandSkillEvent evt) => OnCommandSkill.Invoke(evt);
    public event Action<int> OnNormalAttack = delegate { };
    public void NotifyNormalAttack(int skillId) => OnNormalAttack.Invoke(skillId);



    public event Action<PointerClickEvent> OnPointerClick = delegate { };
    public void NotifyPointerClick(PointerClickEvent evt) => OnPointerClick.Invoke(evt);
    // ==========================================
    // NHÓM sự kiện cho bot AI
    // ==========================================
    public event Action<DirectionalInputEvent> OnDirectionalInputForBot = delegate { };
    public void NotifyDirectionalInputForBot(DirectionalInputEvent evt) => OnDirectionalInputForBot.Invoke(evt);
    public event Action<int> OnNormalAttackForBot = delegate { };
    public void NotifyNormalAttackForBot(int id) => OnNormalAttackForBot.Invoke(id);
    /// <summary>
    /// Sự kiện thông báo khi dữ liệu game đã được load xong, ví dụ: Map, NPC, Item, v.v.
    /// </summary>
    public event Action OnDataLoadCompleted = delegate { };
    public void NotifyDataLoadCompleted() => OnDataLoadCompleted.Invoke();

    ///Nhom 8: su kien change scence
    public event Action<string> OnChangeScene = delegate { };
    public void NotifyChangeScene(string sceneName) => OnChangeScene.Invoke(sceneName);

    public event Action OnCloseUIMapSelect = delegate { };
    public void NotifyCloseUIMapSelect() => OnCloseUIMapSelect.Invoke();


    //Da loaded
    public event Action OnMasterDataLoaded = delegate { };
    public void NotifyMasterDataLoaded() => OnMasterDataLoaded.Invoke();
    //Load UI cho trận đấu

    public event Action OnMatchStart = delegate { };
    public void NotifyMatchStart() => OnMatchStart.Invoke();


    //Targeted
    public event Action<UnitController> OnEnemyTargeted = delegate { };

    public void NotifyEnemyTargeted(UnitController enemy)
    {
        OnEnemyTargeted.Invoke(enemy);
    }
    public event Action<int> OnChangedTotalKilled = delegate { };
    public void NotifyKillCountChanged(int total) => OnChangedTotalKilled.Invoke(total);

    ///Time in match
    public event Action<float> OnTimeMatch = delegate { };
    public void NotifyTimeMatch(float time) => OnTimeMatch.Invoke(time);

    ///End Game
    public event Action<EndGameWinOrLost> OnEndMatchWinOrLost = delegate { };
    public void NotifyEndMatch(EndGameWinOrLost evt) => OnEndMatchWinOrLost.Invoke(evt);


    ///Resource trong game gold, dynamond, exp...
    public event Action<UserProfileCache> OnUserProfileUpdated = delegate { };
    public void NotifyUserProfileUpdated(UserProfileCache userData) => OnUserProfileUpdated.Invoke(userData);

    public event Action<string> OnLoginFailed = delegate { };
    public void NotifyLoginFailed(string reason) => OnLoginFailed.Invoke(reason);

    public event Action<PlayerDataDto> OnLoginSuccess = delegate { };
    public void NotifyLoginSuccess(PlayerDataDto playerData) => OnLoginSuccess.Invoke(playerData);

    public event Action OnRequireLogin = delegate { };
    public void NotifyRequireLogin() => OnRequireLogin.Invoke();
}


// =========================================================================
// KHAI BÁO CẤU TRÚC DỮ LIỆU (STRUCTS) CHO CÁC SỰ KIỆN Ở TRÊN
// Bỏ tất cả các Struct này nằm dưới cùng của file SystemEventManager.cs
// =========================================================================
public struct PointerClickEvent
{
    public Vector2 ScreenPosition;
    public InputMode Mode; // Truyền theo mode để các script khác biết đường xử lý
}
public struct UnitSpawnedEvent
{
    public int UnitId;

    public bool IsLocalPlayer;
}

public struct UnitDiedGlobalEvent
{
    public int DeadUnitId;
    public int KillerUnitId;
    public Vector3 DeathPosition;
}

public struct PlayerLevelUpEvent
{
    public int PlayerId;
    public int NewLevel;
}

public struct CurrencyChangedEvent
{
    public string CurrencyType; // Ví dụ: "Gold", "Gems"
    public int NewAmount;
    public int AmountChanged; // Ví dụ: +50 hoặc -10
}

public struct MapLoadedEvent
{
    public string MapName;
    // public MapEnvironment Environment; // Mở comment này khi bạn code file MapEnvironment
}

public struct SessionStateChangedEvent
{
    public int NewState; // Hoặc dùng Enum SessionState
}

public struct OpenDialogUIEvent
{
    public string NpcName;
    public string DialogText;
}

public struct ShowFloatingTextEvent
{
    public Vector3 WorldPosition;
    public string Text;
    public Color TextColor;
}
public struct RequestJoinMapEvent
{
}
public struct DirectionalInputEvent
{
    public Vector2 Direction;
}

public struct ChangeCameraModeEvent
{
    public CameraMode TargetMode; // Đảm bảo bạn đã có Enum CameraMode
}


public struct EndGameWinOrLost
{
    public int Star;
    public string WinOrLost;
    public int Gold;
    public int Exp;
    public string Time;
}

public struct Resource
{
    public long Gold;
    public int Ruby;
    public int Exp;
}