using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Linq;
using System;

public class Leaderboards : MonoBehaviour
{
    [SerializeField] Transform leadeboardInfoParent;
    List<GameObject> leaderboardPanels = new List<GameObject>();
    [SerializeField] Dropdown selection;
    [SerializeField] GameObject failedText;
    [SerializeField] GameObject scrollHolder;
    Transform playersPanel;//this is used to store the users panel

    private void OnEnable() {
        BuildLeaderboards();
    }

    private void OnDisable() {
        DestroyPanels();
        failedText.SetActive(false);
    }

    private void DestroyPanels() {
        foreach (GameObject child in leaderboardPanels) {
            Destroy(child);
        }
        leaderboardPanels.Clear();
    }

    public void BuildLeaderboards() {
        DestroyPanels();
        failedText.SetActive(false);
        StartCoroutine(BuildingLeaderboards());
    }

    private IEnumerator BuildingLeaderboards() {
        var task = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            failedText.SetActive(true);
        } else {
            SpawnLeaderPanels(GetArray(task.Result));
        }
    }

    private void SpawnLeaderPanels(List<Leaderboard> leaders) {
        for (int i = 0; i < leaders.Count; i++) {
            GameObject leaderboardPanel = Instantiate(Resources.Load<GameObject>("LeaderboardInfoPanel"), leadeboardInfoParent);
            leaderboardPanel.transform.localScale = new Vector3(1,1,1);
            leaderboardPanels.Add(leaderboardPanel);
            LeaderboardInfoPanel leaderboardInfoPanel = leaderboardPanel.GetComponent<LeaderboardInfoPanel>();
            leaderboardInfoPanel.standingText.text = (i + 1).ToString() + ".";
            leaderboardInfoPanel.nameText.text = leaders[i].name;
            if (selection.value == 0) {
                leaderboardInfoPanel.infoText.text = leaders[i].onlineWins.ToString();
            }
            if (selection.value == 1) {
                leaderboardInfoPanel.infoText.text = leaders[i].onlineWinPercentage.ToString() + "%";
            }
            if (selection.value == 2) {
                leaderboardInfoPanel.infoText.text = leaders[i].offlineWins.ToString();
            }
            if (selection.value == 3) {
                leaderboardInfoPanel.infoText.text = leaders[i].offlineWinPercentage.ToString() + "%";
            }
            if (leaders[i].id == LocalUser.locUser.userID) {
                playersPanel = leaderboardPanel.transform;
                leaderboardInfoPanel.standingText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
                leaderboardInfoPanel.nameText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
                leaderboardInfoPanel.infoText.color = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);
            }
        }
        ScrollToPlayerPanel();
    }

    private void ScrollToPlayerPanel() {
        Canvas.ForceUpdateCanvases();
        leadeboardInfoParent.GetComponent<RectTransform>().anchoredPosition = (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(leadeboardInfoParent.GetComponent<RectTransform>().position) - (Vector2)scrollHolder.GetComponent<ScrollRect>().transform.InverseTransformPoint(playersPanel.position);
        leadeboardInfoParent.GetComponent<RectTransform>().localPosition = new Vector3(-25, leadeboardInfoParent.GetComponent<RectTransform>().localPosition.y, leadeboardInfoParent.GetComponent<RectTransform>().localPosition.z);
    }

    private List<Leaderboard> GetArray(DataSnapshot snap) {
        List<Leaderboard> arrayOfStats = new List<Leaderboard>();
        foreach (DataSnapshot snapShot in snap.Children) {
            User user = JsonUtility.FromJson<User>(snapShot.GetRawJsonValue());
            if (!string.IsNullOrEmpty(user.userName) && !string.IsNullOrWhiteSpace(user.userName)) {
                arrayOfStats.Add(new Leaderboard(user.userName, user.onlineGamesWon, GetPercentage(user.onlineGamesWon, user.onlineGamesPlayed), user.offlineGamesWon, GetPercentage(user.offlineGamesWon, user.offlineGamesPlayed), user.userID));
            }
        }
        return SortStandings(arrayOfStats);
    }

    private float GetPercentage(float numerator, float denominator) {
        float percentage = 0;
        if (numerator > 0) {
            percentage = (numerator / denominator) * 100;
            percentage = (float)Math.Round((decimal)percentage, 2);
        }
        return percentage;
    }

    private List<Leaderboard> SortStandings(List<Leaderboard> arrayOfStats) {
        List<Leaderboard> listToReturn = new List<Leaderboard>();
        if (selection.value == 0) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.onlineWins).ToList();
        }
        if (selection.value == 1) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.onlineWinPercentage).ToList();
        }
        if (selection.value == 2) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.offlineWins).ToList();
        }
        if (selection.value == 3) {
            listToReturn = arrayOfStats.OrderBy(leaderboard => leaderboard.offlineWinPercentage).ToList();
        }
        listToReturn.Reverse();
        return listToReturn;
    }

    public class Leaderboard {
        public string name;
        public int onlineWins;
        public float onlineWinPercentage;
        public int offlineWins;
        public float offlineWinPercentage;
        public string id;

        public Leaderboard(string n, int onw, float onwp, int offw, float offwp, string ID) {
            name = n;
            onlineWins = onw;
            onlineWinPercentage = onwp;
            offlineWins = offw;
            offlineWinPercentage = offwp;
            id = ID;
        }

    }

}
