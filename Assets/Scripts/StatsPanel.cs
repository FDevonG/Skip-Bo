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

        if (LocalUser.user == null) {
            FireBaseScript.GetCurrentUser();
        }

        int totalGamesPlayed = LocalUser.user.offlineGamesPlayed + LocalUser.user.onlineGamesPlayed;
        int totalGamesWon = LocalUser.user.offlineGamesWon + LocalUser.user.onlineGamesWon;

        totalGamesPlayedText.text = "Total Games Played - " + totalGamesPlayed.ToString();
        offlineGamesPlayedText.text = "Offline Games Played - " + LocalUser.user.offlineGamesPlayed.ToString();
        onlineGamesPlayedText.text = "Online Games Played - " + LocalUser.user.onlineGamesPlayed.ToString();
        totalGamesWonText.text = "Total Games Won - " + totalGamesWon.ToString();
        offlineGamesWonText.text = "Offline Games Won - " + LocalUser.user.offlineGamesWon.ToString();
        onelineGamesWonText.text = "Online Games Won - " + LocalUser.user.onlineGamesWon.ToString();
        totalWinPercentageText.text = "Total Win Percent - " + GetPercentage(totalGamesWon, totalGamesPlayed).ToString() + "%";
        offlineGamesWonPercentageText.text = "Offline Win Percent - " + GetPercentage(LocalUser.user.offlineGamesWon, LocalUser.user.offlineGamesPlayed) + "%";
        onlineGamesWonPercentageText.text = "Online Win Percent - " + GetPercentage(LocalUser.user.onlineGamesWon, LocalUser.user.onlineGamesPlayed) + "%";
    }

    private int GetPercentage(int smallNumber, int largeNumber) {
        if (smallNumber == 0) {
            return 0;
        } else {
            return (smallNumber / largeNumber) * 100;
        }
    }
}
