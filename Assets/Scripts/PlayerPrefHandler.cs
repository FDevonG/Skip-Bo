using UnityEngine;

public class PlayerPrefsHandler {

    public static void SetMusicSetting(int value) {
        PlayerPrefs.SetInt("music", value);
    }

    public static int GetMusicSetting() {
        int musicValue = PlayerPrefs.GetInt("music");
        return musicValue;
    }

    public static void SetSoundEffectsSetting(int value) {
        PlayerPrefs.SetInt("soundEffects", value);
    }

    public static int GetSoundEffectsSetting() {
        int soundEffectsValue = PlayerPrefs.GetInt("soundEffects");
        return soundEffectsValue;
    }

}
