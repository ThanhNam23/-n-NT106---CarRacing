using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; } // Singleton instance

    public string playerName; // Biến lưu tên người chơi
    public int playerCount;   // Số lượng người chơi trong lobby
    public List<string> playerNames = new List<string>();
    public bool isHost;
    private void Awake()
    {
        // Kiểm tra xem đã có một instance nào chưa
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Nếu đã có, xóa object này để giữ 1 instance duy nhất
            return;
        }

        Instance = this; // Gán instance hiện tại
        DontDestroyOnLoad(gameObject); // Không xóa object này khi chuyển scene
    }
    public void SetPlayerNames(List<string> playerNames)
    {
        this.playerNames = playerNames;
        playerCount = playerNames.Count; // Cập nhật số lượng người chơi
    }
    //public void SavePlayerNames()
    //{
    //    string playerNamesString = string.Join(",", playerNames); // Nối danh sách thành chuỗi
    //    PlayerPrefs.SetString("PlayerNames", playerNamesString);
    //    PlayerPrefs.Save();
    //}
    //public void LoadPlayerNames()
    //{
    //    string playerNamesString = PlayerPrefs.GetString("PlayerNames", ""); // Chuỗi rỗng nếu không có giá trị
    //    if (!string.IsNullOrEmpty(playerNamesString))
    //    {
    //        playerNames = new List<string>(playerNamesString.Split(',')); // Tách chuỗi thành danh sách
    //    }
    //    else
    //    {
    //        playerNames = new List<string>(); // Tạo danh sách rỗng nếu không có dữ liệu
    //    }
    //}
}
