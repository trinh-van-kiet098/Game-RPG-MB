using System.IO;
using UnityEngine;

public class CharacterDataManager : MonoBehaviour
{
    private const string ProfileFileName = "character-profile.json";

    public CharacterProfile Profile { get; private set; }

    private string ProfilePath => Path.Combine(Application.persistentDataPath, ProfileFileName);

    public void Initialize()
    {
        if (Profile != null) return;

        if (!File.Exists(ProfilePath))
        {
            Profile = new CharacterProfile();
            return;
        }

        try
        {
            string json = File.ReadAllText(ProfilePath);
            Profile = JsonUtility.FromJson<CharacterProfile>(json);
            if (Profile == null)
            {
                Debug.LogError($"Không thể đọc hồ sơ nhân vật từ {ProfilePath}: dữ liệu không hợp lệ.");
                Profile = new CharacterProfile();
            }
            NormalizeProfile();
        }
        catch (IOException exception)
        {
            Debug.LogError($"Không thể đọc hồ sơ nhân vật tại {ProfilePath}: {exception.Message}");
            Profile = new CharacterProfile();
        }
        catch (System.UnauthorizedAccessException exception)
        {
            Debug.LogError($"Không có quyền đọc hồ sơ nhân vật tại {ProfilePath}: {exception.Message}");
            Profile = new CharacterProfile();
        }
    }

    public void ApplyProfile(CharacterProfile profile, bool saveLocalCopy = true)
    {
        if (profile == null)
        {
            Debug.LogError("Không thể áp dụng hồ sơ nhân vật null.");
            return;
        }

        Profile = profile;
        NormalizeProfile();
        if (saveLocalCopy) SaveProfile();
    }

    public void SaveProfile()
    {
        if (Profile == null)
        {
            Debug.LogError("Không thể lưu hồ sơ nhân vật vì Profile chưa được khởi tạo.");
            return;
        }

        try
        {
            string directory = Path.GetDirectoryName(ProfilePath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(ProfilePath, JsonUtility.ToJson(Profile, true));
        }
        catch (IOException exception)
        {
            Debug.LogError($"Không thể lưu hồ sơ nhân vật tại {ProfilePath}: {exception.Message}");
        }
        catch (System.UnauthorizedAccessException exception)
        {
            Debug.LogError($"Không có quyền lưu hồ sơ nhân vật tại {ProfilePath}: {exception.Message}");
        }
    }

    private void NormalizeProfile()
    {
        if (Profile.inventory == null) Profile.inventory = new System.Collections.Generic.List<InventoryItemData>();
        if (Profile.ownedMaterialIds == null) Profile.ownedMaterialIds = new System.Collections.Generic.List<int>();
        if (Profile.ownedSkillIds == null) Profile.ownedSkillIds = new System.Collections.Generic.List<int>();
        if (Profile.equippedSkillIds == null) Profile.equippedSkillIds = new System.Collections.Generic.List<int>();
        if (string.IsNullOrEmpty(Profile.equippedWeaponId)) Profile.equippedWeaponId = "sword";
        if (string.IsNullOrEmpty(Profile.characterAddressKey)) Profile.characterAddressKey = string.Empty;
    }
}
