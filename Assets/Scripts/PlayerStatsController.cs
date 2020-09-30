using UnityEngine;
using System.Collections;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void AddGamePlayed() {
        if (PhotonNetwork.offlineMode) {
            LocalUser.locUser.offlineGamesPlayed++;
            Database.UpdateUser("offlineGamesPlayed", LocalUser.locUser.offlineGamesPlayed);
        } else {
            LocalUser.locUser.onlineGamesPlayed++;
            Database.UpdateUser("onlineGamesPlayed", LocalUser.locUser.onlineGamesPlayed);
        }
    }

    public void AddGameWon() {
        if (PhotonNetwork.offlineMode) {
            LocalUser.locUser.offlineGamesWon++;
            Database.UpdateUser("offlineGamesWon", LocalUser.locUser.offlineGamesWon);
        } else {
            LocalUser.locUser.onlineGamesWon++;
            Database.UpdateUser("onlineGamesWon", LocalUser.locUser.onlineGamesWon);
        }
    }
}
