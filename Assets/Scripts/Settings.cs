using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle soundEffectsToggle;

    private void OnEnable() {
        musicToggle.isOn = Sounds.IsMusicPlaying();
        soundEffectsToggle.isOn = Sounds.IsSoundEffectsActive();
        Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        musicToggle.onValueChanged.AddListener(delegate {
            sounds.PlayButtonClick();
            sounds.SwitchMusic();
        });
        soundEffectsToggle.onValueChanged.AddListener(delegate {
            sounds.PlayButtonClick();

        });
    }

    public void SwitchMusic() {
        if (musicToggle.isOn) {
            PlayerPrefsHandler.SetMusicSetting(1);
        } else {
            PlayerPrefsHandler.SetMusicSetting(0);
        }
    }

    public void SwitchSoundEffects() {
        if (soundEffectsToggle.isOn) {
            PlayerPrefsHandler.SetSoundEffectsSetting(1);
        } else {
            PlayerPrefsHandler.SetSoundEffectsSetting(0);
        }
    }
}
