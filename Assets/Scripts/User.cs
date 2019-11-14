using System;
using System.Collections.Generic;

[Serializable]
public class User {
    public string email;
    public string userName;
    public string uniqueID;
    public Dictionary<string, string> avatar = new Dictionary<string, string>();
    public Dictionary<string, int> playerStats = new Dictionary<string, int>();
    public List<string> friends = new List<string>();

    public User(string userEmail) {
        email = userEmail;
    }

    public User (string userEmail, string id) {
        email = userEmail;
        uniqueID = id;
    }

    public User () {

    }

    public void BuildStartingStatsDictionary() {
        playerStats.Add("offlineGamesPlayed", 0);
        playerStats.Add("onlineGamesPlayed", 0);
        playerStats.Add("offlineGamesWon", 0);
        playerStats.Add("onlineGamesWon", 0);
    }

}
