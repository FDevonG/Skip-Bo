using System;
using System.Collections.Generic;

[Serializable]
public class User {
    public string email;
    public string userName;
    public string userID;

    public string hair;
    public string face;
    public string kit;
    public string body;

    public int offlineGamesPlayed;
    public int onlineGamesPlayed;
    public int offlineGamesWon;
    public int onlineGamesWon;

    public List<string> friends = new List<string>();
    public List<string> blocked = new List<string>();

    public int level;
    public int experience;
    public int experienceToNextLevel;

    public List<Achievment> achievments = new List<Achievment>();

    public int gamesWonInARow;

    public bool adsBlocked;

    public int gems = 0;

    public List<string> unlockedHair = new List<string>();
    public List<string> unlockedFace = new List<string>();
    public List<string> unlockedClothes = new List<string>();

    public User(string userEmail, string id, List<Achievment> ach) {
        email = userEmail;
        userID = id;
        achievments = ach;
        level = 1;
        experienceToNextLevel = 100;
    }

    public User(string name, string uHair, string uFace, string Ukit, string Ubody) {
        userName = name;
        hair = uHair;
        face = uFace;
        kit = Ukit;
        body = Ubody;
    }

    public User () {

    }

}
