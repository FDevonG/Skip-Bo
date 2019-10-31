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
        PlayerData data = SaveSystem.LoadPlayer();
        int totalGamesPlayed = data.offlineGamesPlayed + data.onlineGamesPlayed;
        int totalGamesWon = data.offlineGamesWon + data.onlineGamesWon;

        totalGamesPlayedText.text = "Total Games Played - " + totalGamesPlayed.ToString();
        offlineGamesPlayedText.text = "Offline Games Played - " + data.offlineGamesPlayed.ToString();
        onlineGamesPlayedText.text = "Online Games Played - " + data.onlineGamesPlayed.ToString();
        totalGamesWonText.text = "Total Games Won - " + totalGamesWon.ToString();
        offlineGamesWonText.text = "Offline Games Won - " + data.offlineGamesWon.ToString();
        onelineGamesWonText.text = "Online Games Won - " + data.onlineGamesWon.ToString();
        totalWinPercentageText.text = "Total Win Percent - " + GetPercentage(totalGamesWon, totalGamesPlayed).ToString() + "%";
        offlineGamesWonPercentageText.text = "Offline Win Percent - " + GetPercentage(data.offlineGamesWon, data.offlineGamesPlayed) + "%";
        onlineGamesWonPercentageText.text = "Online Win Percent - " + GetPercentage(data.onlineGamesWon, data.onlineGamesPlayed) + "%";
    }

    private int GetPercentage(int smallNumber, int largeNumber) {
        if (smallNumber == 0) {
            return 0;
        } else {
            return (smallNumber / largeNumber) * 100;
        }
    }
}
