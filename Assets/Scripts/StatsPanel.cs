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
        SpawnStats();
    }

    private void SpawnStats() {
        
        int totalGamesPlayed = LocalUser.locUser.offlineGamesPlayed + LocalUser.locUser.onlineGamesPlayed;
        int totalGamesWon = LocalUser.locUser.offlineGamesWon + LocalUser.locUser.onlineGamesWon;
        
        totalGamesPlayedText.text = "Total Games Played - " + totalGamesPlayed;
        offlineGamesPlayedText.text = "Offline Games Played - " + LocalUser.locUser.offlineGamesPlayed;
        onlineGamesPlayedText.text = "Online Games Played - " + LocalUser.locUser.onlineGamesPlayed;
        totalGamesWonText.text = "Total Games Won - " + totalGamesWon.ToString();
        offlineGamesWonText.text = "Offline Games Won - " + LocalUser.locUser.offlineGamesWon;
        onelineGamesWonText.text = "Online Games Won - " + LocalUser.locUser.onlineGamesWon;
        totalWinPercentageText.text = "Total Win Percent - " + GetPercentage(totalGamesWon, totalGamesPlayed) + "%";
        offlineGamesWonPercentageText.text = "Offline Win Percent - " + GetPercentage(LocalUser.locUser.offlineGamesWon, LocalUser.locUser.offlineGamesPlayed) + "%";
        onlineGamesWonPercentageText.text = "Online Win Percent - " + GetPercentage(LocalUser.locUser.onlineGamesWon, LocalUser.locUser.onlineGamesPlayed) + "%";
    }

    private int GetPercentage(int smallNumber, int largeNumber) {
        if (smallNumber == 0) {
            return 0;
        } else {
            return (smallNumber / largeNumber) * 100;
        }
    }
}
