using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle soundEffectsToggle;
    public Toggle announcerToggle;

    private void OnEnable() {
        musicToggle.isOn = Sounds.Instance.IsMusicPlaying();
        soundEffectsToggle.isOn = Sounds.Instance.IsSoundEffectsActive();
        announcerToggle.isOn = Announcer.Instance.IsAnnouncerActive();
        musicToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
            Sounds.Instance.SwitchMusic();
        });
        soundEffectsToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
        });
        announcerToggle.onValueChanged.AddListener(delegate {
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

    public void SwitchAnnouncer()
    {
        if (announcerToggle.isOn)
        {
            PlayerPrefsHandler.SetAnnouncer(1);
        }
        else
        {
            PlayerPrefsHandler.SetAnnouncer(0);
        }
    }
}
