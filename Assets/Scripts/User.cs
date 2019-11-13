using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User {
    string email;
    string userName;
    string userID;
    Dictionary<string, string> avatar = new Dictionary<string, string>();
    Dictionary<string, int> playerStats = new Dictionary<string, int>();
    List<string> friends = new List<string>();

    public User (string userEmail, string id) {
        Email = userEmail;
        UserID = id;
    }

    public string Email {
        get { return email; }
        set { email = value; }
    }

    public string UserName {
        get { return userName; }
        set { userName = value; }
    }

    public string UserID {
        get { return userID; }
        set { userID = value; }
    }

    public Dictionary<string, string> Avatar {
        get { return avatar; }
        set { avatar = value; }
    }

    public Dictionary<string, int> PlayerStats {
        get { return playerStats; }
        set { playerStats = value; }
    }

    public List<string> Friends {
        get { return friends; }
        set { friends = value; }
    }

}
