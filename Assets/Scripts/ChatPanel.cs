using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatPanel : MonoBehaviour
{
    public static ChatPanel Instance { get; private set; }
    List<GameObject> messages = new List<GameObject>();
    int maxHistory = 35;
    [SerializeField] GameObject chatPanel;
    //[SerializeField] Transform contentParent;
    [SerializeField] Transform messageParent;
    [SerializeField] InputField messageInput;
    [SerializeField] Text numberText;
    int messageCounter = 0;

    float timeStartedLerping;
    float timeTakenDuringLerp = 1f;

    public bool chatOpen = false;
    bool chatOpening = false;
    bool chatClosing = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 3) {
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject child in objects) {
                if (child.tag == "ChatButton") {
                    child.SetActive(true);
                    break;
                }
            }
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        if (scene.buildIndex == 0 || scene.buildIndex == 1) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    public void ChatPanelControl() {
        timeStartedLerping = Time.time;
        if (!chatOpen) {
            chatOpen = true;
            chatOpening = true;
            chatClosing = false;
            messageCounter = 0;
            numberText.text = "";
            return;
        }
        if (chatOpen) {
            chatOpen = false;
            chatOpening = false;
            chatClosing = true;
        }
    }

    public void SendMessage() {
        if (!string.IsNullOrEmpty(messageInput.text) && !string.IsNullOrWhiteSpace(messageInput.text)) {
            Chat.Instance.SendPublicMessage(PhotonNetwork.room.Name, messageInput.text);
            messageInput.text = "";
        } 
    }

    public void ReceiveMessage(string user, string message) {
        if (!Friends.IsPlayerBlocked(user)) {
            if (!chatOpen) {
                messageCounter++;
                numberText.text = messageCounter.ToString();
            }
            GameObject text = Instantiate(Resources.Load<GameObject>("MessagePanel"), messageParent);
            if (messages.Count >= maxHistory) {
                Destroy(messages[0]);
                messages.Remove(messages[0]);
            }
            messages.Add(text);
            text.GetComponent<ChatMessageSetUp>().SetUpMessage(GetUserName(user) + ": ", message);
        }
        Sounds.Instance.NewMessage();
    }

    public void PlayerStatus(string message) {
        if (!chatOpen) {
            messageCounter++;
            numberText.text = messageCounter.ToString();
        }
        GameObject text = Instantiate(Resources.Load<GameObject>("MessageText"), messageParent);
        if (messages.Count >= maxHistory) {
            Destroy(messages[0]);
            messages.Remove(messages[0]);
        }
        messages.Add(text);
        text.GetComponent<Text>().text = message;
        Sounds.Instance.NewMessage();
    }

    private string GetUserName(string user) {
        string userName = "";
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList) {
            if (photonPlayer.UserId == user) {
                userName = (string)photonPlayer.CustomProperties["name"];
                break;
            }
        }
        return userName;
    }

    // Update is called once per frame
    void Update()
    {
        if (chatOpen) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                SendMessage();
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ChatPanelControl();
            }
        }
        if (chatOpening) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chatPanel.transform.localScale = Vector3.Lerp(new Vector3(1, 0, 1), new Vector3(1, 1, 1), percentageComplete);
            for (int i = 0; i < chatPanel.transform.childCount; i++) {
                if (chatPanel.transform.GetChild(i).name != "Scroll View") {
                    Color startColor = chatPanel.transform.GetChild(i).GetComponent<Image>().color;
                    chatPanel.transform.GetChild(i).GetComponent<Image>().color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.g, 0), new Color(startColor.r, startColor.g, startColor.g, 1), percentageComplete);
                }
            }
            //chatPanel.GetComponent<Image>().color = Color.Lerp(new Color(chatColor.r, chatColor.g, chatColor.b, 0), new Color(chatColor.r, chatColor.g, chatColor.b, 1), percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                chatOpening = false;
            }
        }
        if (chatClosing) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chatPanel.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1, 0, 1), percentageComplete);
            for (int i = 0; i < chatPanel.transform.childCount; i++) {
                if (chatPanel.transform.GetChild(i).name != "Scroll View") {
                    Color startColor = chatPanel.transform.GetChild(i).GetComponent<Image>().color;
                    chatPanel.transform.GetChild(i).GetComponent<Image>().color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.g, 1), new Color(startColor.r, startColor.g, startColor.g, 0), percentageComplete);
                }
            }
            //chatPanel.GetComponent<Image>().color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                chatClosing = false;
            }
        }
    }
}
