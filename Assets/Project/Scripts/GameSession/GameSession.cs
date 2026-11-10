using System.Collections.Generic;

// 1. Định nghĩa chi tiết các chế độ chơi
public enum GameMode
{
    PvE_Campaign,  // Đánh quái (1 vs Máy)
    PvP_Solo,      // Đối kháng 1 vs 1
    PvP_Team_3v3   // Đối kháng 3 vs 3
}

public enum TeamFaction 
{
    PlayerTeam,
    EnemyTeam
}

public static class GameSession
{
    // Biến lưu chế độ chơi hiện tại
    public static GameMode CurrentMode = GameMode.PvP_Team_3v3;

    public static List<int> PlayerTeamIds = new List<int>();
    public static List<int> EnemyTeamIds = new List<int>();
    public static int Map;

    // ==========================================
    // 2. BIẾN ĐỘNG (PROPERTY): TỰ ĐỘNG XÁC ĐỊNH SỐ LƯỢNG
    // ==========================================
    public static int MaxPlayersPerTeam
    {
        get
        {
            // Tự động trả về con số giới hạn tùy thuộc vào CurrentMode
            switch (CurrentMode)
            {
                case GameMode.PvE_Campaign: return 1;
                case GameMode.PvP_Solo:     return 1;
                case GameMode.PvP_Team_3v3: return 3;
                default:                    return 1; // Mặc định an toàn
            }
        }
    }

    public static void ResetSelection()
    {
        PlayerTeamIds.Clear();
        EnemyTeamIds.Clear();
    }
}