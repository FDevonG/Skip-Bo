using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Firebase.Database;

public class Chat : MonoBehaviour, IChatClientListener
{
    public static Chat Instance { get; private set; }
    ChatClient chatClient;

    bool chatConnected = false;

    ExitGames.Client.Photon.ConnectionProtocol connectProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;

    string globalChannel = "global";

    public static List<string> friendsOnline = new List<string>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        chatClient = new ChatClient(Instance, connectProtocol);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectToChat() {
        if (!chatClient.CanChat) {
            Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
            authValues.UserId = PhotonNetwork.player.UserId;
            authValues.AuthType = Photon.Chat.CustomAuthenticationType.None;
            chatClient.Connect(GameGlobalSettings.PhotonChatAppId(), GameGlobalSettings.Version(), authValues);
        }
    }

    public void DebugReturn(DebugLevel level, string message) {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state) {
        Debug.Log(state);
    }

    public void OnConnected() {
        chatConnected = true;
        chatClient.ChatRegion = "Us";
        chatClient.Subscribe(new string[] { globalChannel });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        UpdateFriends();
    }

    public void OnDisconnected() {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        for (int i = 0; i < messages.Length; i++) { //go through each received msg
            string sender = senders[i];
            string msg = (string)messages[i];
            Debug.Log(sender + ": " + msg);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) {
        if (sender != FireBaseScript.AuthenitcationKey()) {
            StartCoroutine(SpawnGameInvite((string)message));
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        StartCoroutine(SpawnStatusNotification(user, status));
        UpdateOnlineFriends(user, status);
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        Debug.Log("Connected to channel");
    }

    public void OnUnsubscribed(string[] channels) {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user) {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user) {
        throw new System.NotImplementedException();
    }

    public void UpdateFriends() {
        chatClient.AddFriends(LocalUser.locUser.friends.ToArray());
    }

    void UpdateOnlineFriends(string user, int status) {
        if (status == 2) {
            friendsOnline.Add(user);
        }
        if (status == 0) {
            foreach (string friend in friendsOnline) {
                if (friend == user) {
                    friendsOnline.Remove(friend);
                    break;
                }
            }
        }
    }

    public void SendGameInvite(string userId, string message) {
        chatClient.SendPrivateMessage(userId, message);
    }

    public void Disconnect() {
        chatClient.Disconnect();
    }

    private IEnumerator SpawnGameInvite( string message) {
        while (GameObject.FindGameObjectWithTag("GameInvitePanel") != null && CardDragHandler.itemBeingDragged != null) {
            yield return new WaitForSeconds(1);
        }
        GameObject invitePanel = Instantiate(Resources.Load<GameObject>("GameInvitePanel"), GameObject.Find("Canvas").transform);
        invitePanel.transform.localScale = new Vector3(1, 1, 1);
        invitePanel.GetComponent<InvitedToGamePanel>().SetUpAcceptButton(message);
    }

    private IEnumerator SpawnStatusNotification(string user, int status) {
        while (GameObject.FindGameObjectWithTag("NotificationPanel") != null && CardDragHandler.itemBeingDragged != null) {
            yield return new WaitForSeconds(1);
        }
        string userName = "";
        foreach (User friend in Friends.friends) {
            if (friend.userID == user) {
                userName = friend.userName;
                break;
            }
        }
        GameObject notificationPanel = Instantiate(Resources.Load<GameObject>("NotificationPanel"), GameObject.Find("Canvas").transform);
        notificationPanel.transform.localScale = new Vector3(1,1,1);
        if (status == 2) {
            notificationPanel.GetComponent<NotificationPanel>().SetText(userName + " has logged on");
        }
        if (status == 0) {
            notificationPanel.GetComponent<NotificationPanel>().SetText(userName + " has logged off");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null) {
            chatClient.Service();
        }
    }

    void OnApplicationQuit() {
        if (chatClient != null) { chatClient.Disconnect(); }
    }
}
