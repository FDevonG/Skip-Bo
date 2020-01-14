using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievments : MonoBehaviour
{

    public static Achievments Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public List<Achievment> BuildAchievmentsList() {
        List<Achievment> achievments = new List<Achievment>();

        achievments.Add(new Achievment("It's a start", false, "Reach level 5", "LevelFiveAchievement", "LevelFiveAchievementGreyscale"));
        achievments.Add(new Achievment("One step at a time", false, "Reach level 10", "LevelTenAchievement", "LevelTenAchievementGreyscale"));
        achievments.Add(new Achievment("New heights achieved", false, "Reach level 25", "LevelTwentyFiveAchievement", "LevelTwentyFiveAchievementGreyscale"));
        achievments.Add(new Achievment("Half way", false, "Reach level 50", "LevelFifteyAchievement", "LevelFiftyAchievementGreyscale"));
        achievments.Add(new Achievment("Devoted", false, "Reach level 75", "LevelSeventyFiveAchievement", "LevelSeventyFiveAchievementGreyscale"));
        achievments.Add(new Achievment("Got what it takes", false, "Reach level 100", "LevelOneHundredAchievement", "LevelOneHundredAchievementGreyscale"));

        achievments.Add(new Achievment("Champion", false, "Win first game", "WonGameAchievement", "WonGameAchievementGreyscale"));
        achievments.Add(new Achievment("Online champion", false, "Win an online game", "OnlineGameWonAchievement", "OnlineGameWonAchievementGreyscale"));
        achievments.Add(new Achievment("Hot streak", false, "Win 5 games in a row", "WonFiveInARowAchievement", "WonFiveInARowAchievementGreyscale"));

        achievments.Add(new Achievment("Friends in high places", false, "Add first friend", "FriendAddedAchievement", "FriendAddedAchievementGreyscale"));
        achievments.Add(new Achievment("Better with friends", false, "Sent game invite to friend", "GameInviteAchievement", "GameInviteAchievementGreyscale"));

        return achievments;
    }

    public IEnumerator SaveAchievments() {
        var task = Database.SaveUserAchievments();
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = Database.SaveUserAchievments();
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public IEnumerator GamesWon() {
        if (LocalUser.locUser.gamesWonInARow >= 5) {
            StartCoroutine(UnlockAchievement("Hot streak"));
        }
        var task = Database.UpdateUser("gamesWonInARow", LocalUser.locUser.gamesWonInARow);
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = Database.UpdateUser("gamesWonInARow", LocalUser.locUser.gamesWonInARow);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public void CheckLevelAchievments(int level) {
        switch(level) {
            case 5:
                StartCoroutine(UnlockAchievement("It's a start"));
                break;
            case 10:
                StartCoroutine(UnlockAchievement("One step at a time"));
                break;
            case 25:
                StartCoroutine(UnlockAchievement("New heights achieved"));
                break;
            case 50:
                StartCoroutine(UnlockAchievement("Half way"));
                break;
            case 75:
                StartCoroutine(UnlockAchievement("Devoted"));
                break;
            case 100:
                StartCoroutine(UnlockAchievement("Got what it takes"));
                break;
        }
    }

    public void FreindAdded() {
        StartCoroutine(UnlockAchievement("Friends in high places"));
    }

    public IEnumerator UnlockAchievement(string name) {
        foreach (Achievment achievment in LocalUser.locUser.achievments) {
            if (achievment.name == name) {
                if (!achievment.unlocked) {
                    achievment.unlocked = true;
                    StartCoroutine(SaveAchievments());
                    while (GameObject.FindGameObjectWithTag("NotificationPanel") != null) {
                        yield return new WaitForSeconds(0.5f);
                    }
                    GameObject notificationPanel = Instantiate(Resources.Load<GameObject>("NotificationPanel"));
                    notificationPanel.GetComponent<NotificationPanel>().SetText("You have reached unlocked '" + achievment.name + "'");
                    break;
                }
            }
        }
    }
}
