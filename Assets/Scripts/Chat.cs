using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;

public class Chat : MonoBehaviour, IChatClientListener
{
    public static Chat Instance { get; private set; }
    ChatClient chatClient;

    bool chatConnected = false;

    ExitGames.Client.Photon.ConnectionProtocol connectProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;

    string globalChannel = "global";

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
        Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
        authValues.UserId = PhotonNetwork.player.UserId;
        authValues.AuthType = Photon.Chat.CustomAuthenticationType.None;
        chatClient.Connect(GameGlobalSettings.PhotonChatAppId(), GameGlobalSettings.Version(), authValues);
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
        StartCoroutine(UpdateFriends());
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
        GameObject invitePanel = Instantiate(Resources.Load<GameObject>("GameInvitePanel"), GameObject.Find("Canvas").transform);
        invitePanel.GetComponent<InvitedToGamePanel>().SetUpAcceptButton((string)message);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        throw new System.NotImplementedException();
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

    IEnumerator UpdateFriends() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        User user = JsonUtility.FromJson<User>(task.Result);
        chatClient.AddFriends(user.friends.ToArray());
    }

    public void SendGameInvite(string userId, string message) {
        chatClient.SendPrivateMessage(userId, message);
    }

    public void Disconnect() {
        chatClient.Disconnect();
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
