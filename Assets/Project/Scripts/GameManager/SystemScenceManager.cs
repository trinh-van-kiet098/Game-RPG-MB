
using UnityEngine;
using UnityEngine.SceneManagement;
public class SystemSceneManager : ConfiguredMonoBehaviour
{

    public override void Configure(SystemGameManager manager)
    {
        base.Configure(manager);

    }
    void OnDestroy()
    {
        if (systemGameManager?.EventManager != null)
        {
        }
    }
}