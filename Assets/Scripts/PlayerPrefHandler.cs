using UnityEngine;

public class PlayerPrefsHandler {

    public static void SetMusicSetting(int value) {
        PlayerPrefs.SetInt("music", value);
    }

    public static int GetMusicSetting() {
        return PlayerPrefs.GetInt("music");
    }

    public static void SetSoundEffectsSetting(int value) {
        PlayerPrefs.SetInt("soundEffects", value);
    }

    public static int GetSoundEffectsSetting() {
        return PlayerPrefs.GetInt("soundEffects");
    }

    public static void SetGameRated(int value) {
        PlayerPrefs.SetInt("rated", value);
    }

    public static int GetRated() {
        return PlayerPrefs.GetInt("rated");
    }

    public static void SetGamesPlayed() {
        PlayerPrefs.SetInt("gamesPlayed", GetGamesPlayed() + 1);
    }

    public static void ResetGamesPlayed() {
        PlayerPrefs.SetInt("gamesPlayed", 0);
    }

    public static int GetGamesPlayed() {
        return PlayerPrefs.GetInt("gamesPlayed");
    }
}
