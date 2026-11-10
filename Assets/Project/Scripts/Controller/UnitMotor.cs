using UnityEngine;

public class UnitMotor : MonoBehaviour
{
    private UnitController unitController;
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody rb;
    [SerializeField] private float jumpForce = 7f;
    private Collider col;

    // Biến lưu trữ hướng đi hiện tại
    private Vector3 horizontalInput;

    public void Configure(UnitController controller)
    {
        this.unitController = controller;

        if (!TryGetComponent<Rigidbody>(out rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        if (!TryGetComponent<Collider>(out col))
        {
            col = gameObject.AddComponent<CapsuleCollider>();
        }

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        this.unitController.UnitEvent.OnJump += Jump;
        this.unitController.UnitEvent.OnDizzyStop += DizzyStop;
        this.unitController.UnitEvent.OnMovement += Move;

        // BỔ SUNG DÒNG NÀY: Nối dây để Motor nghe được lệnh Stop
        this.unitController.UnitEvent.OnStopMovement += StopMovement;
    }



    public void TickUpdate(float deltaTime)
    {
        // Để trống hoặc xử lý các logic không liên quan đến vật lý ở đây
    }

    // Hàm FixedUpdate tự động chạy đồng bộ với frame rate của hệ thống vật lý
    private void FixedUpdate()
    {
        if (rb == null) return;

        // 1. Tính toán vận tốc di chuyển ngang (X, Z) dựa trên input
        Vector3 targetVelocity = horizontalInput * moveSpeed;

        // 2. Giữ nguyên vận tốc rơi tự do của trục Y (để nhảy và rớt xuống đất mượt)
        targetVelocity.y = rb.linearVelocity.y;

        // 3. Gán vận tốc mới cho Rigidbody (Cách di chuyển mượt nhất)
        rb.linearVelocity = targetVelocity;

        // 4. Xoay mặt nhân vật bằng vật lý
        if (horizontalInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalInput);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    public void Move(Vector3 direction)
    {
        // ĐÚNG NHƯ BẠN COMMENT: Ở đây CHỈ lưu trữ hướng bấm phím từ Event gửi sang
        horizontalInput = direction;
    }

    public void MoveToNetworkPosition(Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - transform.position);
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Move(direction.normalized);
        }
    }

    public void StopMovement()
    {
        horizontalInput = Vector3.zero;
        
    }

    public void DizzyStop()
    {
        horizontalInput = Vector3.zero;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            // Tranh thủ reset trục Y trước khi add force để nhảy đều mọi lúc
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        if (col == null) return false;
        float distanceToGround = col.bounds.extents.y + 0.1f;
        return Physics.Raycast(col.bounds.center, Vector3.down, distanceToGround);
    }

    private void OnDisable()
    {
        if (this.unitController != null && this.unitController.UnitEvent != null)
        {
            this.unitController.UnitEvent.OnJump -= Jump;
            this.unitController.UnitEvent.OnMovement -= Move;
            this.unitController.UnitEvent.OnDizzyStop -= DizzyStop;

            // HỦY NỐI DÂY KHI NHÂN VẬT CHẾT/BIẾN MẤT
            this.unitController.UnitEvent.OnStopMovement -= StopMovement;
        }
    }
}