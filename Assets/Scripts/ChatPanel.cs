using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatPanel : MonoBehaviour
{
    public static ChatPanel Instance { get; private set; }
    List<GameObject> messages = new List<GameObject>();
    int maxHistory = 25;
    [SerializeField] GameObject chatPanel;
    [SerializeField] Transform contentParent;
    [SerializeField] InputField messageInput;
    [SerializeField] Text numberText;
    Chat chat;
    int messageCounter = 0;

    float timeStartedLerping;
    float timeTakenDuringLerp = 1f;

    bool chatOpen = false;
    bool chatOpening = false;
    bool chatClosing = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        chat = GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChatPanelControl() {
        timeStartedLerping = Time.time;
        if (!chatOpen) {
            chatOpen = true;
            chatOpening = true;
            chatClosing = false;
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
            chat.SendPublicMessage(PhotonNetwork.room.Name, messageInput.text);
            messageInput.text = "";
        } 
    }

    public void ReceiveMessage(string user, string message) {
        if (!chatOpen) {
            messageCounter++;

        }
        GameObject text = Instantiate(Resources.Load<GameObject>("MessageText"), contentParent);
        if (messages.Count >= maxHistory) {
            Destroy(messages[0]);
            messages.Remove(messages[0]);
        }
        messages.Add(text);
        text.GetComponent<Text>().text = GetUserName(user) + " : " + message; 
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
        if (chatOpening) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chatPanel.transform.localScale = Vector3.Lerp(new Vector3(1, 0, 1), new Vector3(1, 1, 1), percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                chatOpening = false;
            }
        }
        if (chatClosing) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chatPanel.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1, 0, 1), percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                chatClosing = false;
            }
        }
    }
}
