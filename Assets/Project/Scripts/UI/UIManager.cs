using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum MenuId
{
    MainMenu,
    Settings,
    Shop,
    Inventory,
    GameHUD,
    CharacterSelection,
}

public class UIManager : ConfiguredMonoBehaviour
{
    private SystemGameManager system;

    [System.Serializable]
    public struct MenuData
    {
        public MenuId id;
        public VisualTreeAsset uxmlAsset;
    }

    [Header("Danh sách tất cả các Menu UXML")]
    [SerializeField] private List<MenuData> menuDatabase = new List<MenuData>();

    [Header("Màn hình hiển thị UI chung")]
    [SerializeField] private UIDocument mainUIDocument;

    private Dictionary<MenuId, VisualElement> activeMenus = new Dictionary<MenuId, VisualElement>();
    private VirtualJoystickUI joystickController;

    [Header("Script Preview 3D")]
    [SerializeField] private CharacterPreviewUI characterPreviewUI;
    [Tooltip("Điểm xuất hiện nhân vật; để trống thì tạo ở tọa độ (0, 0, 0).")]
    [SerializeField] private Transform playerSpawnPoint;

    private bool isSpawningPlayer;
    private string selectedWeaponId = "sword";
    private int selectedMaterialId = 1;
    private Color selectedMaterialColor = new Color32(52, 152, 219, 255);

    private void Awake()
    {
        joystickController = GetComponent<VirtualJoystickUI>();
    }

    public bool IsPointerOverJoystick(Vector2 screenPosition)
    {
        return joystickController != null && joystickController.IsPointerOver(screenPosition);
    }

    public override void Configure(SystemGameManager system)
    {
        this.system = system;
        OpenMenu(MenuId.MainMenu);
    }

    public void OpenMenu(MenuId id)
    {
        if (activeMenus.ContainsKey(id))
        {
            Debug.LogWarning($"Menu {id} đang mở rồi, không cần mở lại!");
            return;
        }

        var targetMenu = menuDatabase.Find(x => x.id == id);
        if (targetMenu.uxmlAsset == null)
        {
            Debug.LogError($"Không tìm thấy Menu với ID: {id} trong Database!");
            return;
        }

        if (mainUIDocument == null)
        {
            Debug.LogError("Chưa gán Main UIDocument vào UIManager trong Inspector!");
            return;
        }

        var root = mainUIDocument.rootVisualElement;

        VisualElement menuInstance = targetMenu.uxmlAsset.CloneTree();
        menuInstance.style.width = Length.Percent(100);
        menuInstance.style.height = Length.Percent(100);
        menuInstance.style.position = Position.Absolute;
        
        root.Add(menuInstance);
        activeMenus[id] = menuInstance;

        // ĐẶT XỬ LÝ NÀY XUỐNG DƯỚI SAU KHI menuInstance ĐÃ ĐƯỢC ADD VÀO ROOT
        if (id == MenuId.GameHUD && joystickController != null)
        {
            joystickController.Initialize(menuInstance);
        }

        if (id == MenuId.CharacterSelection)
        {
            SetCameraSwipeEnabled(false);
        }

        Debug.Log($"Đã mở thành công Menu: {id}");

        SetupMenuEvents(id, menuInstance);
    }

    public void CloseMenu(MenuId id)
    {
        if (activeMenus.TryGetValue(id, out VisualElement menuInstance))
        {
            menuInstance.RemoveFromHierarchy();
            activeMenus.Remove(id);
            if (id == MenuId.CharacterSelection)
            {
                SetCameraSwipeEnabled(true);
            }
            Debug.Log($"Đã xóa thành công Menu cũ ra khỏi màn hình: {id}");
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy Menu {id} trong activeMenus để đóng!");
        }
    }

    private void SetCameraSwipeEnabled(bool enabled)
    {
        if (system == null || system.InputManager == null)
        {
            Debug.LogError("Không thể cập nhật vuốt camera: chưa gán InputManager trong SystemGameManager!");
            return;
        }

        system.InputManager.SetCameraSwipeEnabled(enabled);
    }

    private void SetupMenuEvents(MenuId id, VisualElement rootElement)
    {
        switch (id)
        {
            case MenuId.MainMenu:
                var btnStart = rootElement.Q<Button>("btn-start");
                if (btnStart != null)
                {
                    btnStart.clicked -= OnStartButtonClicked;
                    btnStart.clicked += OnStartButtonClicked;
                }
                var btnNewGame = rootElement.Q<Button>("btn-newgame");
                if (btnNewGame != null)
                {
                    btnNewGame.clicked += () =>
                    {
                        OpenMenu(MenuId.CharacterSelection);
                        CloseMenu(MenuId.MainMenu);
                    };
                }
                break;

            case MenuId.Settings:
                var btnClose = rootElement.Q<Button>("btn-close");
                if (btnClose != null)
                {
                    btnClose.clicked += () => CloseMenu(MenuId.Settings);
                }
                break;

            case MenuId.GameHUD:
                var btnInventory = rootElement.Q<Button>("btn-inventory");
                if (btnInventory != null)
                {
                    btnInventory.clicked += () => OpenMenu(MenuId.Inventory);
                }
                break;

            case MenuId.CharacterSelection:
                if (characterPreviewUI == null)
                {
                    characterPreviewUI = FindFirstObjectByType<CharacterPreviewUI>();
                }

                if (characterPreviewUI != null)
                {
                    characterPreviewUI.InitPreview(rootElement);
                    CharacterProfile profile = system.CharacterDataManager.Profile;
                    selectedWeaponId = profile.equippedWeaponId == "dual" ? "dual" : "sword";
                    selectedMaterialId = profile.equippedMaterialId >= 1 && profile.equippedMaterialId <= 4
                        ? profile.equippedMaterialId
                        : 1;
                    selectedMaterialColor = profile.equippedMaterialId == 0 || profile.equippedMaterialColor.a <= 0f
                        ? ColorForMaterial(selectedMaterialId)
                        : profile.equippedMaterialColor;
                    characterPreviewUI.ChangeCharacterColor(selectedMaterialColor);
                }
                else
                {
                    Debug.LogError("Vẫn chưa tìm thấy component CharacterPreviewUI trong Scene!");
                }

                var btnSword = rootElement.Q<Button>("btn-weapon-sword");
                var btnDual = rootElement.Q<Button>("btn-weapon-dual");
                btnSword?.EnableInClassList("selected", selectedWeaponId == "sword");
                btnDual?.EnableInClassList("selected", selectedWeaponId == "dual");
                UpdateMaterialSelection(rootElement, selectedMaterialId);
                if (btnSword != null)
                {
                    btnSword.clicked += () => SelectWeapon("sword", btnSword, btnDual);
                }
                if (btnDual != null)
                {
                    btnDual.clicked += () => SelectWeapon("dual", btnDual, btnSword);
                }

                var btnConfirm = rootElement.Q<Button>("btn-confirm");
                if (btnConfirm != null)
                {
                    btnConfirm.clicked += OnConfirmCharacterSelection;
                }

                var btnBack = rootElement.Q<Button>("btn-back");
                if (btnBack != null)
                {
                    btnBack.clicked += () => {
                        CloseMenu(MenuId.CharacterSelection);
                        OpenMenu(MenuId.MainMenu);
                    };
                }

                // --- BẮT SỰ KIỆN CÁC NÚT CHỌN MÀU ---
                var btnBlue = rootElement.Q<Button>("btn-color-blue");
                if (btnBlue != null) 
                    btnBlue.clicked += () => SelectMaterial(1, rootElement); // Xanh dương

                var btnRed = rootElement.Q<Button>("btn-color-red");
                if (btnRed != null) 
                    btnRed.clicked += () => SelectMaterial(2, rootElement); // Đỏ

                var btnGreen = rootElement.Q<Button>("btn-color-green");
                if (btnGreen != null) 
                    btnGreen.clicked += () => SelectMaterial(3, rootElement); // Xanh lá

                var btnGold = rootElement.Q<Button>("btn-color-gold");
                if (btnGold != null) 
                    btnGold.clicked += () => SelectMaterial(4, rootElement); // Vàng
                // ------------------------------------
                break;
        }
    }

    private void SelectWeapon(string weaponId, Button selectedButton, Button otherButton)
    {
        selectedWeaponId = weaponId;
        selectedButton?.EnableInClassList("selected", true);
        otherButton?.EnableInClassList("selected", false);
    }

    private void SelectMaterial(int materialId, VisualElement rootElement)
    {
        selectedMaterialId = materialId;
        selectedMaterialColor = ColorForMaterial(materialId);
        if (characterPreviewUI != null)
        {
            characterPreviewUI.ChangeCharacterColor(selectedMaterialColor);
        }

        UpdateMaterialSelection(rootElement, materialId);
    }

    private static void UpdateMaterialSelection(VisualElement rootElement, int materialId)
    {
        rootElement.Q<Button>("btn-color-blue")?.EnableInClassList("selected", materialId == 1);
        rootElement.Q<Button>("btn-color-red")?.EnableInClassList("selected", materialId == 2);
        rootElement.Q<Button>("btn-color-green")?.EnableInClassList("selected", materialId == 3);
        rootElement.Q<Button>("btn-color-gold")?.EnableInClassList("selected", materialId == 4);
    }

    private static Color ColorForMaterial(int materialId)
    {
        switch (materialId)
        {
            case 2: return new Color32(231, 76, 60, 255);
            case 3: return new Color32(46, 204, 113, 255);
            case 4: return new Color32(241, 196, 15, 255);
            default: return new Color32(52, 152, 219, 255);
        }
    }

    private void OnConfirmCharacterSelection()
    {
        if (isSpawningPlayer) return;
        if (system == null || system.CharacterDataManager == null)
        {
            Debug.LogError("Không thể tạo nhân vật: CharacterDataManager chưa được khởi tạo.");
            return;
        }
        if (characterPreviewUI == null || string.IsNullOrEmpty(characterPreviewUI.CharacterAddressKey))
        {
            Debug.LogError("Không thể tạo nhân vật: chưa gán Address Key cho CharacterPreviewUI.");
            return;
        }

        isSpawningPlayer = true;
        Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
            characterPreviewUI.CharacterAddressKey,
            spawnPosition,
            Quaternion.identity);
        handle.Completed += OnPlayerInstantiationCompleted;
    }

    private void OnPlayerInstantiationCompleted(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Không thể tạo nhân vật từ Address Key: {characterPreviewUI.CharacterAddressKey}");
            Addressables.Release(handle);
            isSpawningPlayer = false;
            return;
        }

        GameObject player = handle.Result;
        UnitController unitController = player.GetComponent<UnitController>();
        if (unitController == null)
        {
            Debug.LogError("Prefab nhân vật không có UnitController; không thể vào game.");
            Addressables.ReleaseInstance(player);
            isSpawningPlayer = false;
            return;
        }

        CharacterProfile profile = system.CharacterDataManager.Profile;
        profile.characterAddressKey = characterPreviewUI.CharacterAddressKey;
        profile.equippedWeaponId = selectedWeaponId;
        profile.equippedMaterialId = selectedMaterialId;
        profile.equippedMaterialColor = selectedMaterialColor;
        system.CharacterDataManager.SaveProfile();

        ApplyCharacterColor(player, selectedMaterialColor);
        unitController.Configure(system);
        unitController.SetCharacterProfile(profile);
        unitController.InitializeUnit(100, UnitType.Player, true);
        if (system.UnitManager != null)
        {
            system.UnitManager.RegisterUnit(unitController);
        }
        system.EventManager.NotifyPlayerSpawnedCameraFollow(player.transform);

        CloseMenu(MenuId.CharacterSelection);
        OpenMenu(MenuId.GameHUD);
        isSpawningPlayer = false;
    }

    private static void ApplyCharacterColor(GameObject character, Color color)
    {
        foreach (Renderer renderer in character.GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color);
                }
                else if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", color);
                }
            }
        }
    }

    private void OnStartButtonClicked()
    {
        CloseMenu(MenuId.MainMenu);
        OpenMenu(MenuId.GameHUD);
    }
}