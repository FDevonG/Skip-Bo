using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class Achievments : MonoBehaviour
{
    public static int itsAStartIndex = 0;

    public static List<Achievment> BuildAchievmentsList() {
        List<Achievment> achievments = new List<Achievment>();

        achievments.Add(new Achievment("Its a start", false, "Reach level 5", "LevelFiveAchievement", "LevelFiveAchievementGreyscale"));
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

    public static IEnumerator SaveAchievments() {
        var task = Database.SaveUserAchievments();
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = Database.SaveUserAchievments();
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }
}
