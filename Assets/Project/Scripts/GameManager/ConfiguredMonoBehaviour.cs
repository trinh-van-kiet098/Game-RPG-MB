using UnityEngine;

public abstract class ConfiguredMonoBehaviour : MonoBehaviour
{
    protected PhysicsScene physicsScene;
    public PhysicsScene PhysicsScene { get => physicsScene; }

    // Các tham chiếu (References) trỏ về Hệ thống chính
    protected SystemGameManager systemGameManager = null;
    protected SystemEventManager systemEventManager = null;

    protected int configureCount = 0;

    // Hàm này được SystemGameManager gọi để nhét dữ liệu vào
    public virtual void Configure(SystemGameManager manager)
    {
        this.systemGameManager = manager;
        this.physicsScene = gameObject.scene.GetPhysicsScene();
        SetGameManagerReferences();
        configureCount++;
    }

    // Tự động gán các Manager con để class nào kế thừa cũng xài được ngay
    public virtual void SetGameManagerReferences()
    {
        if (systemGameManager != null)
        {
            this.systemEventManager = systemGameManager.EventManager;
        }
    }

}
