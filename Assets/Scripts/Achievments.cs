using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievments : MonoBehaviour
{
    public static int itsAStartIndex = 0;

    public static List<string[]> BuildAchievmentsList() {
        List<string[]> achievments = new List<string[]>();

        achievments.Add(new string[3] { "Its a start", "false", "Reach level 5" });
        achievments.Add(new string[3] { "", "false", "Reach level 10" });
        achievments.Add(new string[3] { "", "false", "Reach level 25" });
        achievments.Add(new string[3] { "", "false", "Reach level 50" });
        achievments.Add(new string[3] { "", "false", "Reach level 75" });
        achievments.Add(new string[3] { "", "false", "Reach level 100" });

        achievments.Add(new string[3] { "", "false", "Win first game" });
        achievments.Add(new string[3] { "", "false", "Win an online game" });
        achievments.Add(new string[3] { "Hot streak", "false", "Win 5 games in a row" });

        achievments.Add(new string[3] { "Friends in high places", "false", "Add first friend" });

        return achievments;
    }

    public static IEnumerator SaveAchievments() {
        var task = Database.UpdateUser("achievments", LocalUser.locUser.achievments);
        yield return new WaitUntil(() => task.IsCompleted);
        while (task.IsFaulted) {
            task = Database.UpdateUser("achievments", LocalUser.locUser.achievments);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }
}
