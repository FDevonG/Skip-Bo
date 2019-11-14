using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{

    public void AddGamePlayed() {
        if (PhotonNetwork.offlineMode) {
            LocalUser.user.offlineGamesPlayed += 1;
        } else {
            LocalUser.user.onlineGamesPlayed += 1;
        }
        FireBaseScript.UpdateUser(LocalUser.user);
    }

    public void AddGameWon() {
        if (PhotonNetwork.offlineMode) {
            LocalUser.user.offlineGamesWon += 1;
        } else {
            LocalUser.user.onlineGamesWon += 1;
        }
        FireBaseScript.UpdateUser(LocalUser.user);
    }
}
