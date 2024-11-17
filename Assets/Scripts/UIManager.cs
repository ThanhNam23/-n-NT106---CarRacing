using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    [SerializeField] private Button startServerbtn;
    [SerializeField] private Button startClientbtn;
    [SerializeField] private Button startHostbtn;
    //[SerializeField] private TextMeshProUGUI playerAmountText;
    //private NetworkVariable<int> playerAmount=new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    private void Awake()
    {
        startHostbtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        startClientbtn.onClick.AddListener(() =>
        { 
            NetworkManager.Singleton.StartClient();
        });
        startServerbtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
    }
    //private void Update()
    //{
    //    playerAmountText.text = "Players: " + playerAmount.Value.ToString();
    //    if (!IsServer) { return; }
    //    playerAmount.Value=NetworkManager.Singleton.ConnectedClients.Count;
    //}
}
