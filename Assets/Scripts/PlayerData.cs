
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

    public PlayerData(Player player) {
        name = player.Name;
        hair = player.Hair;
        face = player.Face;
        kit = player.Kit;
        body = player.Body;

        offlineGamesPlayed = player.OfflineGamesPlayed;
        onlineGamesPlayed = player.OnlineGamesPlayed;
        offlineGamesWon = player.OfflineGamesWon;
        onlineGamesWon = player.OnlineGamesWon;
    }

}
