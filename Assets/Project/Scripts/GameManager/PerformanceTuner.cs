using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PerformanceTuner : MonoBehaviour
{
    private void Start()
    {
        AutoConfigureGraphics();
    }

    private void AutoConfigureGraphics()
    {
        // Lấy dung lượng RAM thực tế của điện thoại (tính bằng Megabyte)
        int deviceRam = SystemInfo.systemMemorySize;
        // Debug.Log($"[Performance] Device RAM: {deviceRam} MB");

        // Lấy file cấu hình URP hiện tại đang dùng trong Project
        UniversalRenderPipelineAsset urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        // TÌNH HUỐNG 1: MÁY YẾU (Dưới 3GB RAM)
        if (deviceRam <= 3072) 
        {
            Application.targetFrameRate = 60; // Khóa FPS ở 60 để máy không bị nóng
            QualitySettings.SetQualityLevel(0, true); // Chuyển về mức Low Quality

            if (urpAsset != null)
            {
                urpAsset.renderScale = 0.6f; // Chỉ render 60% điểm ảnh -> Giảm giật cực mạnh
            }

            // Tắt gió cỏ và giảm tầm nhìn của Terrain
            if (Terrain.activeTerrain != null)
            {
                Terrain.activeTerrain.terrainData.wavingGrassAmount = 0f;
                Terrain.activeTerrain.detailObjectDistance = 30f; // Cỏ chỉ hiện khi lại rất gần
            }
        }
        // TÌNH HUỐNG 2: MÁY TẦM TRUNG (Từ 4GB - 6GB RAM)
        else if (deviceRam <= 6144) 
        {
            Application.targetFrameRate = 60;
            QualitySettings.SetQualityLevel(1, true); // Mức Medium

            if (urpAsset != null)
            {
                urpAsset.renderScale = 0.85f; // Render 85% điểm ảnh
            }

            if (Terrain.activeTerrain != null)
            {
                Terrain.activeTerrain.terrainData.wavingGrassAmount = 0.2f; // Gió thổi nhẹ
                Terrain.activeTerrain.detailObjectDistance = 80f;
            }
        }
        // TÌNH HUỐNG 3: MÁY MẠNH (Trên 6GB RAM)
        else 
        {
            Application.targetFrameRate = 60;
            QualitySettings.SetQualityLevel(2, true); // Mức High

            if (urpAsset != null)
            {
                urpAsset.renderScale = 1.0f; // Nét căng 100%
            }

            if (Terrain.activeTerrain != null)
            {
                Terrain.activeTerrain.terrainData.wavingGrassAmount = 0.5f; // Gió thổi mạnh, cỏ đẹp
                Terrain.activeTerrain.detailObjectDistance = 150f;
            }
        }
    }
}