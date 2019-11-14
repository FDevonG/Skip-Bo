using System;
using System.Collections.Generic;

[Serializable]
public class User {
    public string email;
    public string userName;

    public string hair;
    public string face;
    public string kit;
    public string body;

    public int offlineGamesPlayed;
    public int onlineGamesPlayed;
    public int offlineGamesWon;
    public int onlineGamesWon;

    public List<string> friends = new List<string>();

    public User(string userEmail) {
        email = userEmail;
    }

    public User (string name, string uHair, string uFace, string Ukit, string Ubody) {
        userName = name;
        hair = uHair;
        face = uFace;
        kit = Ukit;
        body = Ubody;
    }

    public User () {

    }

}
