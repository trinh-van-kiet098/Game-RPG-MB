using UnityEngine;

namespace Game.Events
{

    public struct OtherPlayerJoinedMapEvent
    {
        public string PlayerName;
        public Vector3 SpawnPosition;
    }
    public enum CameraMode
    {
        Home,
        Customize,
        Farm
    }

    public struct ChangeCameraModeEvent
    {
        public CameraMode TargetMode;
    }
    public struct PointerDownEvent
    {
        public Vector2 ScreenPosition; // Tọa độ trên màn hình 2D
    }

    public struct PointerDragEvent
    {
        public Vector2 ScreenPosition;
        public Vector2 DeltaPosition; // Độ dời so với frame trước (Dùng để vuốt camera)
    }

    public struct PointerUpEvent
    {
        public Vector2 ScreenPosition;
    }
    public struct PointerZoomEvent
    {
        // Giá trị Zoom: 
        // Dương (+) là phóng to (Zoom In)
        // Âm (-) là thu nhỏ (Zoom Out)
        public float ZoomDelta;
    }
    public struct DirectionalInputEvent
    {
        public Vector2 Direction; // Hướng di chuyển (-1 đến 1)
    }

    public struct CommandMoveToEvent
    {
        public Vector3 TargetPosition; // Tọa độ 3D trên mặt đất
    }
    public struct CommandSkillEvent
    {
        public int SkillIndex; // 0: Đánh thường (J), 1: Chiêu 1 (K), 2: Chiêu 2 (L)
    }
}