using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle soundEffectsToggle;

    private void OnEnable() {
        musicToggle.isOn = Sounds.IsMusicPlaying();
        soundEffectsToggle.isOn = Sounds.IsSoundEffectsActive();
        musicToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
            Sounds.Instance.SwitchMusic();
        });
        soundEffectsToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
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
