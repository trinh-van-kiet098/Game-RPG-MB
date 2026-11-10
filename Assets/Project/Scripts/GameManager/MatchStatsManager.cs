using System;
using UnityEngine; // Cần cái này để dùng Debug.Log

public class MatchStatsManager : IDisposable
{
    public void Dispose()
    {
        // Gỡ sự kiện thông qua Singleton, nhớ check null phòng trường hợp tắt game đột ngột
        if (SystemGameManager.Instance != null && SystemGameManager.Instance.EventManager != null)
        {
            // SystemGameManager.Instance.EventManager.OnUnitDied -= HandleUnitDied;
            Debug.Log("[MatchStatsManager] 🧹 Đã hủy lắng nghe sự kiện, giải phóng RAM.");
        }
    }
}