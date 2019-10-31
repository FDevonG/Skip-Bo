using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenu : MonoBehaviour
{
    GameControl gameControl;
    public GameObject menuWindow;
    public GameObject settingsWindow;
    public GameObject howToPlayWindow;
    public GameObject victoryPanel;
    public Button menuButton;

    public GameObject activePanel;

    // Start is called before the first frame update
    void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
        Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        menuButton.onClick.AddListener(sounds.PlayButtonClick);
        settingsWindow.GetComponent<Settings>().musicToggle.onValueChanged.AddListener(delegate {
            sounds.PlayButtonClick();
            sounds.SwitchMusic();
        });
        settingsWindow.GetComponent<Settings>().soundEffectsToggle.onValueChanged.AddListener(delegate {
            sounds.PlayButtonClick();
        });
    }



    public void OpenMenu() {
        if (!GetComponent<GameControl>().playerWon) {
            if (!menuWindow.active) {
                menuWindow.SetActive(true);
                settingsWindow.SetActive(true);
                howToPlayWindow.SetActive(false);
                if (PhotonNetwork.offlineMode) {
                    Time.timeScale = 0;
                }
            } else {
                menuWindow.SetActive(false);
                if (PhotonNetwork.offlineMode) {
                    Time.timeScale = 1;
                }
            }
        }
    }

    public void SwitchPanel(GameObject panel) {
        activePanel.SetActive(false);
        activePanel = panel;
        panel.SetActive(true);
    }

    public void LeaveMatch() {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.Disconnect();
        SceneController.LoadStartMenu();
        Time.timeScale = 1;
    }

    public void PlayAgain() {
        if (PhotonNetwork.offlineMode) {
            SceneController.ReloadScene();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!gameControl.playerWon) {
                OpenMenu();
            }
        }
    }
}
