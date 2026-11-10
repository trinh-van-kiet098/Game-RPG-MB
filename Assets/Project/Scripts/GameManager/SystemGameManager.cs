using UnityEngine;

public class SystemGameManager : MonoBehaviour
{
    public static SystemGameManager Instance { get; private set; }

    [field: SerializeField] public SystemEventManager EventManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
    [field: SerializeField] public UnitManager UnitManager { get; private set; }
    [field: SerializeField] public CameraManager CameraManager { get; private set; }
    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public SystemSceneManager SceneManager { get; private set; }
    [field: SerializeField] public CharacterDataManager CharacterDataManager { get; private set; }



    public SystemSetting SystemSetting;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        InitSystems();
    }

    private void InitSystems()
    {
        if (CharacterDataManager == null)
        {
            CharacterDataManager = GetComponent<CharacterDataManager>();
            if (CharacterDataManager == null)
            {
                CharacterDataManager = gameObject.AddComponent<CharacterDataManager>();
            }
        }
        CharacterDataManager.Initialize();

        if (EventManager == null)
        {
            EventManager = new SystemEventManager();
        }
        if (SystemSetting == null)
        {
            SystemSetting = new SystemSetting();
        }

        if (UIManager != null)
        {
            UIManager.Configure(this);
        }


        if (UnitManager != null)
        {
            UnitManager.Configure(this);
        }
        if (CameraManager != null)
        {
            CameraManager.Configure(this);
        }
        if (InputManager != null)
        {
            InputManager.Configure(this);
        }
       
        if (SceneManager != null)
        {
            SceneManager.Configure(this);
        }


    }

}