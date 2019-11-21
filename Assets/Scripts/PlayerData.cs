using System.Collections.Generic;

[System.Serializable]
public class PlayerData 
{
    public string name;
    public string hair;
    public string face;
    public string kit;
    public string body;

    public int offlineGamesPlayed;
    public int onlineGamesPlayed;
    public int offlineGamesWon;
    public int onlineGamesWon;

    public List<string> friends;

    public PlayerData(User user) {
        name = user.userName;
        hair = user.hair;
        face = user.face;
        kit = user.kit;
        body = user.body;

        offlineGamesPlayed = user.offlineGamesPlayed;
        onlineGamesPlayed = user.onlineGamesPlayed;
        offlineGamesWon = user.offlineGamesWon;
        onlineGamesWon = user.onlineGamesWon;

        friends = user.friends;
    }

}
