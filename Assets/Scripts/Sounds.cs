using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource buttonClickSource;
    [SerializeField] AudioSource yourTurn;
    [SerializeField] AudioSource playerNotification;

    public static Sounds Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        if (IsMusicPlaying()) {
            SwitchMusic();
        }
    }

    public static bool IsMusicPlaying() {
        bool music = true;
        int musicSetting = PlayerPrefsHandler.GetMusicSetting();
        if (musicSetting == 0) {
            music = false;
        }
        if (musicSetting == 1) {
            music = true;
        }
        return music;
    }

    public static bool IsSoundEffectsActive() {
        bool soundEffects = true;
        int soundSettings = PlayerPrefsHandler.GetSoundEffectsSetting();
        if (soundSettings == 0) {
            soundEffects = false;
        }
        if (soundSettings == 1) {
            soundEffects = true;
        }
        return soundEffects;
    }

    public void SwitchMusic() {
        if (IsMusicPlaying()) {
            musicSource.Play();
        } else {
            musicSource.Stop();
        }
    }

    public void PlayButtonClick() {
        if (IsSoundEffectsActive()) {
            buttonClickSource.Play();
        }
    }

    public void PlayCardFlip(AudioSource card) {
        if (IsSoundEffectsActive()) {
            card.Play();
        }
    }

    public void YourTurn() {
        if (IsSoundEffectsActive()) {
            yourTurn.Play();
        }
    }

    public void PlayerNotification() {
        if (IsSoundEffectsActive()) {
            playerNotification.Play();
        }
    }

}
