using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterPreviewUI : MonoBehaviour
{
    [Header("Render Texture từ 3D Camera")]
    [SerializeField] private RenderTexture previewRT;

    [Header("Cấu hình Addressables")]
    public Transform characterPreviewParent; 
    [SerializeField] private string characterAddressKey = ""; 

    private VisualElement previewViewport;
    private GameObject spawnedCharacter;
    
    // Lưu trữ danh sách Material của tất cả các phần trên nhân vật
    private List<Material> characterMaterials = new List<Material>();
    private Texture characterTexture;
    private Color selectedColor = new Color32(52, 152, 219, 255);

    public string CharacterAddressKey => characterAddressKey;
    public Color SelectedColor => selectedColor;

    public void InitPreview(VisualElement root)
    {
        CreateCharacterPreview();

        if (root == null)
        {
            Debug.LogError("Root VisualElement truyền vào InitPreview bị null!");
            return;
        }

        previewViewport = root.Q<VisualElement>("preview-viewport");

        if (previewViewport == null)
        {
            Debug.LogError("LỖI UXML: Không tìm thấy VisualElement nào có tên đúng là 'preview-viewport' trong file CharacterSelect.uxml!");
            return;
        }

        if (previewRT == null)
        {
            Debug.LogError("LỖI INSPECTOR: Chưa kéo file RenderTexture vào ô Preview RT trên component CharacterPreviewUI!");
            return;
        }

        if (!previewRT.IsCreated())
        {
            previewRT.Create();
        }

        previewViewport.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(previewRT));
    }

    public void CreateCharacterPreview()
    {
        if (string.IsNullOrEmpty(characterAddressKey))
        {
            Debug.LogError("LỖI: Chưa nhập Address Key cho nhân vật!");
            return;
        }

        if (characterPreviewParent == null)
        {
            Debug.LogError("LỖI: Chưa gán Character Preview Parent!");
            return;
        }

        if (spawnedCharacter != null)
        {
            Addressables.ReleaseInstance(spawnedCharacter);
        }

        Addressables.InstantiateAsync(characterAddressKey, characterPreviewParent).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                spawnedCharacter = handle.Result;
                spawnedCharacter.transform.localPosition = Vector3.zero;
                spawnedCharacter.transform.localRotation = Quaternion.Euler(0, 100, 0);

                // --- QUÉT TẤT CẢ CÁC RENDERER VÀ MATERIAL ---
                characterMaterials.Clear();
                Renderer[] allRenderers = spawnedCharacter.GetComponentsInChildren<Renderer>();
                
                foreach (var rend in allRenderers)
                {
                    if (rend.material != null)
                    {
                        characterMaterials.Add(rend.material);
                        // Lấy texture chính từ material đầu tiên bắt gặp để tham chiếu
                        if (characterTexture == null)
                        {
                            characterTexture = rend.material.mainTexture;
                        }
                    }
                }

                ApplySelectedColor();
                Debug.Log($"Đã load thành công nhân vật! Tìm thấy {characterMaterials.Count} Material.");
            }
            else
            {
                Debug.LogError($"LỖI: Không thể load prefab với Address Key: {characterAddressKey}");
            }
        };
    }

    // --- HÀM ĐỔI MÀU CHO TOÀN BỘ MATERIAL (TƯƠNG THÍCH URP LIT _BaseColor) ---
    public void ChangeCharacterColor(Color newColor)
    {
        selectedColor = newColor;
        ApplySelectedColor();
    }

    private void ApplySelectedColor()
    {
        if (characterMaterials.Count > 0)
        {
            foreach (var mat in characterMaterials)
            {
                // URP Lit sử dụng _BaseColor làm màu chủ đạo
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", selectedColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", selectedColor);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (spawnedCharacter != null)
        {
            Addressables.ReleaseInstance(spawnedCharacter);
        }
    }
}