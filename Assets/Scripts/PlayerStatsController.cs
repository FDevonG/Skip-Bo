using UnityEngine;
using System.Collections;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator AddGamePlayed() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (!task.IsFaulted) {
            User user = JsonUtility.FromJson<User>(task.Result);
            if (PhotonNetwork.offlineMode) {
                user.offlineGamesPlayed += 1;
                var saveTask = FireBaseScript.UpdateUser("offlineGamesPlayed", user.offlineGamesPlayed);
            } else {
                user.onlineGamesPlayed += 1;
                var saveTask = FireBaseScript.UpdateUser("onlineGamesPlayed", user.onlineGamesPlayed);
            }
            
        }
    }

    public IEnumerator AddGameWon() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (!task.IsFaulted) {
            User user = JsonUtility.FromJson<User>(task.Result);
            if (PhotonNetwork.offlineMode) {
                user.offlineGamesWon += 1;
                var saveTask = FireBaseScript.UpdateUser("offlineGamesWon", user.offlineGamesWon);
            } else {
                user.onlineGamesWon += 1;
                var saveTask = FireBaseScript.UpdateUser("onlineGamesWon", user.onlineGamesWon);
            }
        }
    }
}
