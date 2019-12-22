public class Rating 
{
    public static bool CheckRated() {
        bool rated = false;
        if (PlayerPrefsHandler.GetRated() == 1) {
            rated = true;
        }
        return rated;
    }

    public static bool CheckGamesPlayed() {
        bool gamesPlayed = false;
        if (PlayerPrefsHandler.GetGamesPlayed() >= 5) {
            gamesPlayed = true;
        }
        return gamesPlayed;
    }
}
