using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    GameControl gameControl;
    AdManager adManager;
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
        adManager = GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>();
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
            StartCoroutine(adManager.LeaveMatchAd());
        if (!PhotonNetwork.offlineMode) {
            GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().UnsubscribeToChannel(PhotonNetwork.room.Name);
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
            GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().UnsubscribeToChannel(PhotonNetwork.room.Name);
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
