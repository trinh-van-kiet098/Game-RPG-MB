using System;
using System.Collections.Generic;
[Serializable]
public enum RewardType
{
    Gold,
    Ruby,
    Hero,       // Thay cho Animation/Pet rườm rà lúc trước
    VfxSkill,
    Item        // Vật phẩm linh tinh (vé quay, mảnh tướng...)
}

[Serializable]
public class RewardInfo
{
    public RewardType rewardType;

    // Nếu thưởng Vàng/Kim Cương thì itemId = 0.
    // Nếu thưởng Tướng/VFX thì truyền đúng cái ID từ HeroConfig / VfxSkillConfig vào đây.
    public int itemId;

    public int amount; // Số lượng (Vd: 1000 vàng, hoặc 1 cái VFX)
}
// 1. Cấu hình Tướng (Ông bảo có tướng nên phải thêm vào)
[Serializable]
public class HeroConfig
{
    public int heroId;
    public string heroName;
    public float baseHp;
    public float baseDamage;
    public int priceGold;
    public int priceRuby;
    public string iconPath;
    public string prefabPath; // Asset Addressables
}

// 2. Cấu hình VFX (Ông bảo bán VFX và nó tăng chỉ số)
[Serializable]
public class VfxSkillConfig
{
    public int vfxId;
    public string vfxName;
    public int priceGold;
    public int priceRuby;

    // Chỉ số cộng thêm khi trang bị VFX này
    public float bonusHp;
    public float bonusDamage;
    public float cooldown;

    public string iconPath;
    public string prefabPath;
}

// 3. Cấu hình Quái/Bot
[Serializable]
public class EnemyConfig
{
    public int enemyId;
    public string enemyName;
    public float maxHp;
    public float baseDamage;
    public string prefabPath;
    public string avatarIconPath;
}

// 4. Cấu hình Màn Chơi (Đính kèm Map và danh sách Bot theo yêu cầu)
[Serializable]
public class StageSpawnData
{
    public int enemyId;
    public int spawnCount; // Số lượng con bot này sẽ xuất hiện
}

[Serializable]
public class StageConfig
{
    public int stageId;
    public string stageName;
    public int mapId; // ĐÍNH KÈM: Màn này chơi ở Map nào

    // ĐÍNH KÈM: Màn này xuất hiện những con Bot nào, số lượng bao nhiêu
    public List<StageSpawnData> waves;

    public List<RewardInfo> clearRewards; // Quà qua màn
}

// 5. Cấu hình Map (Bây giờ nó siêu mỏng vì Stage đã lo việc điều phối)
[Serializable]
public class MapConfig
{
    public int mapId;
    public string mapName;
    public string mapSceneName;  // Scene Addressables
    public string pathImageMap;
    public string pathImageBackground; // Background của Map (UI Lobby)
}

// 6. Cấu hình Quà Tuần / Quà Tháng
[Serializable]
public class PeriodicRewardConfig
{
    public int rewardId; // vd: 1 = Tuần 1, 2 = Tháng 1
    public string eventType; // "Weekly" hoặc "Monthly"
    public List<RewardInfo> rewards;
}

// ---> GỘP CHUNG VÀO 1 DATABASE (Parse từ JSON tải từ Server)
[Serializable]
public class MasterDataDatabase
{
    public List<HeroConfig> heroes;
    public List<VfxSkillConfig> vfxSkills;
    public List<EnemyConfig> enemies;
    public List<MapConfig> maps;
    public List<StageConfig> stages;
    public List<PeriodicRewardConfig> periodicRewards;
}

[Serializable]
public class UserProfileCache
{
    public string userId; // ID của người chơi (Server cấp)
    public string userName;
    public int level;
    public float exp;

    // Tiền tệ
    public long gold;
    public long ruby;

    // Kho đồ: Chỉ lưu ID của những thứ đã mua (Server quyết định có hay chưa)
    public List<int> ownedHeroes;
    public List<int> ownedVfxSkills;

    // Đang trang bị
    public int equippedHeroId;        // Tướng đang dùng
    public List<int> equippedVfxIds;  // Các VFX đang lắp cho tướng đó

    // Tiến độ
    public int highestStageClearedId; // Màn chơi cao nhất đã vượt qua

    // Lịch sử nhận quà (Để check xem đã nhận quà tuần/tháng chưa)
    public List<int> claimedPeriodicRewardIds;
}