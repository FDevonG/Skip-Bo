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
            } else {
                user.onlineGamesPlayed += 1;
            }
            StartCoroutine(FireBaseScript.UpdateUser(user));
        }
    }

    public IEnumerator AddGameWon() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (!task.IsFaulted) {
            User user = JsonUtility.FromJson<User>(task.Result);
            if (PhotonNetwork.offlineMode) {
                user.offlineGamesWon += 1;
            } else {
                user.onlineGamesWon += 1;
            }
            StartCoroutine(FireBaseScript.UpdateUser(user));
        }
    }
}
