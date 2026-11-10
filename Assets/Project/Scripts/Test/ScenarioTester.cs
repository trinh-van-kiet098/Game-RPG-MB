using UnityEngine;
using UnityEngine.InputSystem;
public class ScenarioTester : MonoBehaviour
{

    void Update()
    {
        // Bấm phím SPACE (Dấu cách) trên bàn phím để giả lập hành động: "Vừa tiêu diệt được 1 con quái!"
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
            {
                // Bắn sự kiện quái chết ra Loa Phường
                // SystemGameManager.Instance.EventManager.NotifyUnitDied(new UnitDiedGlobalEvent
                // {
                //     DeadUnitId = "Goblin_Test",
                //     KillerUnitId = "Player_Main",
                //     DeathPosition = Vector3.zero
                // });

                // Debug.Log("<color=magenta>🧪 [TESTER] Bạn vừa bấm Space -> Giả lập 1 con quái đã bị hạ gục!</color>");
            }
        }
        // if (Input.GetKey(KeyCode.W))
        // {
        //     if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
        //     {
        //         // Bắn sự kiện quái chết ra Loa Phường
        //         SystemGameManager.Instance.EventManager.NotifyMapLoaded(SystemGameManager.Instance.ScenarioRunner.CurrentScenario);

        //         Debug.Log("<color=magenta>🧪 [TESTER] Bạn vừa bấm Space -> Giả lập 1 con quái đã bị hạ gục!</color>");
        //     }
        // }
    }
}