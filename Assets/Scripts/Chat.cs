using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;

public class Chat : MonoBehaviour, IChatClientListener
{
    public static Chat Instance { get; private set; }
    ChatClient chatClient = new ChatClient(Instance);

    bool chatConnected = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        ConnectToChat();
    }

    public void ConnectToChat() {
        chatClient.Connect(GameGlobalSettings.PhotonAppID(), GameGlobalSettings.Version(), null);
    }

    public void DebugReturn(DebugLevel level, string message) {
        throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state) {
        throw new System.NotImplementedException();
    }

    public void OnConnected() {
        Debug.Log("Hi");
        chatConnected = true;
        chatClient.ChatRegion = "Us";
        chatClient.Subscribe(new string[] { "global" });
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        StartCoroutine(UpdateFriends());
    }

    public void OnDisconnected() {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages) {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName) {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results) {
        throw new System.NotImplementedException();
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
        string[] friends = new string[user.friends.Count];
        for (int i = 0; i < user.friends.Count; i++) {
            friends[i] = user.friends[i];
        }
        chatClient.AddFriends(friends);
        Debug.Log("Friends Added");
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient == null || !chatClient.CanChat) {
            return;
        }
        chatClient.Service();
    }
}
