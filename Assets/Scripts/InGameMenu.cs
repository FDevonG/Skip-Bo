using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    GameControl gameControl;
    public GameObject menuWindow;
    public GameObject settingsWindow;
    public GameObject howToPlayWindow;
    public GameObject victoryPanel;
    public Button menuButton;
    [SerializeField] GameObject FriendListInfoPanel;

    public GameObject activePanel;

    // Start is called before the first frame update
    void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
        menuButton.onClick.AddListener(Sounds.Instance.PlayButtonClick);
        settingsWindow.GetComponent<Settings>().musicToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
            Sounds.Instance.SwitchMusic();
        });
        settingsWindow.GetComponent<Settings>().soundEffectsToggle.onValueChanged.AddListener(delegate {
            Sounds.Instance.PlayButtonClick();
        });
    }

    public void OpenMenu() {
        if (!GetComponent<GameControl>().playerWon) {
            if (!menuWindow.GetActive()) {
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

    public void LeaveMatch() {
        if(!gameControl.playerWon)
            AdManager.Instance.LeaveMatchAd();
        if (!PhotonNetwork.offlineMode) {
            Chat.Instance.UnsubscribeToChannel(PhotonNetwork.room.Name);
        }
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        PhotonNetwork.LeaveRoom();
        if (gameControl.playerWon)
            SceneController.LoadStartMenu();
    }

    public void PlayAgain() {
        if (PhotonNetwork.offlineMode)
        {
            SceneController.ReloadScene();
        }
        else if (!PhotonNetwork.offlineMode) {
            Chat.Instance.UnsubscribeToChannel(PhotonNetwork.room.Name);
            PhotonNetwork.LeaveRoom();
            SceneController.LoadGameSetup();
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
