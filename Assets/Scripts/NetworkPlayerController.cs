using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    public float speed = 5.0f; // Tốc độ di chuyển của đối tượng

    void Update()
    {
        // Chỉ client sở hữu mới có thể nhận đầu vào và gửi lệnh di chuyển
        if (IsClient && IsOwner)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // Nhận đầu vào từ người chơi
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(moveX, 0, moveZ);

        // Gửi lệnh di chuyển tới server
        if (direction != Vector3.zero)
        {
            MoveServerRpc(direction);
        }
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 direction)
    {
        // Server nhận lệnh di chuyển và cập nhật vị trí đối tượng
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;

        // Gửi vị trí cập nhật tới tất cả các client
        UpdatePositionClientRpc(newPosition);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 newPosition)
    {
        // Đồng bộ vị trí trên client (tránh xung đột với client chủ sở hữu)
        if (!IsOwner)
        {
            transform.position = newPosition;
        }
    }
}
