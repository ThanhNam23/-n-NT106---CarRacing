using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    public float speed = 5.0f; // Tốc độ di chuyển

    void Update()
    {
        if (IsClient && IsOwner)
        {
            HandleClientInput();
        }
    }

    void HandleClientInput()
    {
        // Nhận đầu vào từ client
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;

        // Gửi lệnh điều khiển đến server
        SubmitMovementServerRpc(move);
    }

    [ServerRpc]
    void SubmitMovementServerRpc(Vector3 move)
    {
        // Xử lý di chuyển trên server và cập nhật vị trí cho tất cả client
        transform.position += move;
        UpdatePositionClientRpc(transform.position);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 newPosition)
    {
        // Đồng bộ vị trí trên client
        if (!IsOwner) // Tránh xung đột với vị trí của client owner
        {
            transform.position = newPosition;
        }
    }
}
