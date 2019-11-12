using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        LoadPlayer();
    }

    private void LoadPlayer() {
        PlayerData data = SaveSystem.LoadPlayer();
        player = new Player(data.name, data.hair, data.face, data.kit, data.body, data.offlineGamesPlayed, data.onlineGamesPlayed, data.offlineGamesWon, data.onlineGamesWon);
        AddGamePlayed();
    }

    private void AddGamePlayed() {
        if (PhotonNetwork.offlineMode) {
            player.OfflineGamesPlayed += 1;
        } else {
            player.OnlineGamesPlayed += 1;
        }
        SaveSystem.SavePlayer(player);
    }

    public void AddGameWon() {
        if (PhotonNetwork.offlineMode) {
            player.OfflineGamesWon += 1;
        } else {
            player.OnlineGamesWon += 1;
        }
        SaveSystem.SavePlayer(player);
    }
}
