using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab; // Prefab của xe
    [SerializeField] private List<Transform> spawnPoints; // Danh sách các điểm spawn

    private List<ulong> spawnedPlayers = new List<ulong>(); // Lưu danh sách ID người chơi đã spawn

    private void Start()
    {
        //if (NetworkManager.Singleton.IsServer)
        //{
        //    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        //    if(GameData.Instance!=null)
        //    {
        //        Debug.Log("Player list: " + string.Join(", ", GameData.Instance.playerNames));
        //    }
        //    else
        //    {
        //        Debug.LogWarning("GameData has not been initialized!");
        //    }
        //}
        if (GameData.Instance.isHost)
        {
            Debug.Log("Người chơi này là Host!");
            NetworkManager.Singleton.StartHost(); // Bắt đầu với vai trò Host
        }
        else
        {
            Debug.Log("Người chơi này là Client!");
            NetworkManager.Singleton.StartClient(); // Bắt đầu với vai trò Client
        }

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // Kiểm tra số lượng điểm spawn đủ cho người chơi
        if (spawnedPlayers.Count < GameData.Instance.playerNames.Count)
        {
            // Spawn xe tại điểm spawn tiếp theo
            Transform spawnPoint = spawnPoints[spawnedPlayers.Count];
            GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

            // Gắn đối tượng vào Network
            car.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            // Thêm người chơi vào danh sách đã spawn
            spawnedPlayers.Add(clientId);
            // Gắn tên người chơi vào xe
            string playerName = GameData.Instance.playerNames[spawnedPlayers.Count - 1];
            car.GetComponent<Car>().SetPlayerName(playerName);

        }
        else
        {
            Debug.LogWarning("Không đủ điểm spawn cho người chơi mới.");
        }
    }
    private void OnClientDisconnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (spawnedPlayers.Contains(clientId))
        {
            spawnedPlayers.Remove(clientId);
            Debug.Log($"Client {clientId} đã rời phòng.");
        }
    }
}
