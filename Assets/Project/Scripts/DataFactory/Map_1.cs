using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Map_1 : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private string playerPrefabAddress = "Assets/Project/Prefab/PicoChan/Prefabs/Picochan.prefab";
    [SerializeField] private Transform spawnPoint; // Kéo 1 object rỗng vào đây để định vị chỗ sinh ra (tùy chọn)

    private async void Start()
    {
        // 1. Hiển thị giao diện chiến đấu (Nút Joystick, Skills...)
        if (SystemGameManager.Instance != null && SystemGameManager.Instance.UIManager != null)
        {
        }

        // 2. Tải và sinh nhân vật Test
        await SpawnTestPlayerAsync();
    }

    private async Task SpawnTestPlayerAsync()
    {
        // Bắt đầu tải Prefab từ RAM/Ổ cứng thông qua Addressables
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(playerPrefabAddress);
        await handle.Task; // Chờ tải xong

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Xác định tọa độ (nếu chưa kéo điểm Spawn thì cho rơi ở gốc tọa độ 0,0,0)
            Vector3 startPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

            // Sinh nhân vật ra môi trường 3D
            GameObject playerGo = Instantiate(handle.Result, startPos, Quaternion.identity);

            // Gắn dữ liệu để nó biết nó là người chơi thật (Local Player)
            UnitController unitCtrl = playerGo.GetComponent<UnitController>();
            if (unitCtrl != null)
            {
                unitCtrl.Configure(SystemGameManager.Instance);
                // Giả lập ID = 100, Loại = Player, Máy này điều khiển = true
                unitCtrl.InitializeUnit(100, UnitType.Player, true); 
            }

            // Gọi CameraManager khóa mục tiêu vào nhân vật này
            if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
            {
                SystemGameManager.Instance.EventManager.NotifyPlayerSpawnedCameraFollow(playerGo.transform);
            }

            // Debug.Log("<color=green>[Map_1] Load nhân vật Test thành công!</color>");
        }
        else
        {
            Debug.LogError($"[Map_1] Không thể tải Prefab từ Addressables. Kiểm tra lại Key: {playerPrefabAddress}");
        }
    }
}