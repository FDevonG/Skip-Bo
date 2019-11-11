
public class Player 
{
    private string name;
    private string hair;
    private string face;
    private string kit;
    private string body;

    private string uniqueID;

    private int offlineGamesPlayed;
    private int onlineGamesPlayed;
    private int offlineGamesWon;
    private int onlineGamesWon;

    public Player(string playerName, string playerHair, string playerFace, string playerKit, string playerBody, string id) {
        Name = playerName;
        Hair = playerHair;
        Face = playerFace;
        Kit = playerKit;
        Body = playerBody;

        UniqueID = id;
    }

    public Player(string playerName, string playerHair, string playerFace, string playerKit, string playerBody, int offGamesPlayed, int onGamesPlayed, int offGamesWon, int onGamesWon, string id) {
        Name = playerName;
        Hair = playerHair;
        Face = playerFace;
        Kit = playerKit;
        Body = playerBody;

        OfflineGamesPlayed = offGamesPlayed;
        OnlineGamesPlayed = onGamesPlayed;
        OfflineGamesWon = offGamesWon;
        OnlineGamesWon = onGamesWon;

        UniqueID = id;
    }

    public Player() {

    }

    public string Name {
        get { return name; }
        set { name = value; }
    }

    public string Hair {
        get { return hair; }
        set { hair = value; }
    }

    public string Face {
        get { return face; }
        set { face = value; }
    }

    public string Kit {
        get { return kit; }
        set { kit = value; }
    }

    public string Body {
        get { return body; }
        set { body = value; }
    }

    public int OfflineGamesPlayed {
        get { return offlineGamesPlayed; }
        set { offlineGamesPlayed = value; }
    }

    public int OnlineGamesPlayed {
        get { return onlineGamesPlayed; }
        set { onlineGamesPlayed = value; }
    }

    public int OfflineGamesWon {
        get { return offlineGamesWon; }
        set { offlineGamesWon = value; }
    }

    public int OnlineGamesWon {
        get { return onlineGamesWon; }
        set { onlineGamesWon = value; }
    }

    public string UniqueID {
        get { return uniqueID; }
        set { uniqueID = value; }
    }

}
