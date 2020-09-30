using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievments : MonoBehaviour
{

    public static Achievments Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
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

    public void GamesWon() {
        if (LocalUser.locUser.gamesWonInARow >= 5) {
            UnlockAchievement("Hot streak");
        }
    }

    public void CheckLevelAchievments(int level) {
        switch (level) {
            case 5:
                UnlockAchievement("It's a start");
                break;
            case 10:
                UnlockAchievement("One step at a time");
                break;
            case 25:
                UnlockAchievement("New heights achieved");
                break;
            case 50:
                UnlockAchievement("Half way");
                break;
            case 75:
                UnlockAchievement("Devoted");
                break;
            case 100:
                UnlockAchievement("Got what it takes");
                break;
        }
    }

    public void FreindAdded() {
        UnlockAchievement("Friends in high places");
    }

    public void UnlockAchievement(string name) {
        if (!FirebaseAuthentication.IsPlayerAnonymous())
        {
            foreach (Achievment achievment in LocalUser.locUser.achievments)
            {
                if (achievment.name == name)
                {
                    if (!achievment.unlocked)
                    {
                        achievment.unlocked = true;
                        StartCoroutine(SaveAchievments());
                        StartCoroutine(Notifications.Instance.SpawnNotification("You have unlocked '" + achievment.name + "'"));
                        break;
                    }
                }
            }
        }
    }
}
