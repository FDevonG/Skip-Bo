using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{

    public Text totalGamesPlayedText;
    public Text offlineGamesPlayedText;
    public Text onlineGamesPlayedText;
    public Text totalGamesWonText;
    public Text offlineGamesWonText;
    public Text onelineGamesWonText;
    public Text totalWinPercentageText;
    public Text offlineGamesWonPercentageText;
    public Text onlineGamesWonPercentageText;

    private void OnEnable() {
        StartCoroutine(SpawnStats());
    }

    private IEnumerator SpawnStats() {
        var task = FirebaseCloudFunctions.GetUser();
        User user = new User();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            Debug.Log("Failed to load User");
        } else {
            user = JsonUtility.FromJson<User>(task.Result);
        }

        int totalGamesPlayed = LocalUser.user.offlineGamesPlayed + user.onlineGamesPlayed;
        int totalGamesWon = LocalUser.user.offlineGamesWon + user.onlineGamesWon;

        totalGamesPlayedText.text = "Total Games Played - " + totalGamesPlayed.ToString();
        offlineGamesPlayedText.text = "Offline Games Played - " + user.offlineGamesPlayed.ToString();
        onlineGamesPlayedText.text = "Online Games Played - " + user.onlineGamesPlayed.ToString();
        totalGamesWonText.text = "Total Games Won - " + totalGamesWon.ToString();
        offlineGamesWonText.text = "Offline Games Won - " + user.offlineGamesWon.ToString();
        onelineGamesWonText.text = "Online Games Won - " + user.onlineGamesWon.ToString();
        totalWinPercentageText.text = "Total Win Percent - " + GetPercentage(totalGamesWon, totalGamesPlayed).ToString() + "%";
        offlineGamesWonPercentageText.text = "Offline Win Percent - " + GetPercentage(user.offlineGamesWon, user.offlineGamesPlayed) + "%";
        onlineGamesWonPercentageText.text = "Online Win Percent - " + GetPercentage(user.onlineGamesWon, user.onlineGamesPlayed) + "%";
    }

    private int GetPercentage(int smallNumber, int largeNumber) {
        if (smallNumber == 0) {
            return 0;
        } else {
            return (smallNumber / largeNumber) * 100;
        }
    }
}
