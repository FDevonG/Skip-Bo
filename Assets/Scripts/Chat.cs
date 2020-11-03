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

    public static List<string> friendsOnline = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        chatClient = new ChatClient(Instance, connectProtocol);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectToChat()
    {
        if (!chatClient.CanChat)
        {
            Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues();
            authValues.UserId = PhotonNetwork.player.UserId;
            authValues.AuthType = Photon.Chat.CustomAuthenticationType.None;
            chatClient.Connect(GameGlobalSettings.PhotonChatAppId(), GameGlobalSettings.Version(), authValues);
            chatClient.MessageLimit = 25;
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void OnConnected()
    {
        chatConnected = true;
        //chatClient.ChatRegion = "US";
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        UpdateFriends();
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconected from chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        { //go through each received msg
            string sender = senders[i];
            string msg = (string)messages[i];
            if (!Friends.IsPlayerBlocked(sender))
                ChatPanel.Instance.ReceiveMessage(sender, msg);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (sender != FirebaseAuthentication.AuthenitcationKey() && !Friends.IsPlayerBlocked(sender))
        {
            StartCoroutine(SpawnGameInvite((string)message));
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        StartCoroutine(SpawnStatusNotification(user, status));
        UpdateOnlineFriends(user, status);
    }

    public void SubcsribeToChannel(string name)
    {
        chatClient.Subscribe(new string[] { name });
        SendPublicMessage(name, "Has joined.");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //foreach(string channel in channels) {
        //    SendPublicMessage(channel, "Has joined.");
        //}
    }

    public void UnsubscribeToChannel(string channelName)
    {
        //SendPublicMessage(channelName, "Has left.");
        chatClient.Unsubscribe(new string[] { channelName });
    }

    public void OnUnsubscribed(string[] channels)
    {

    }

    public void OnUserSubscribed(string channel, string user)
    {

    }

    public void OnUserUnsubscribed(string channel, string user)
    {

    }

    public void SendPublicMessage(string channelName, string message)
    {
        chatClient.PublishMessage(channelName, message);
    }

    public void UpdateFriends()
    {
        chatClient.AddFriends(LocalUser.locUser.friends.ToArray());
    }

    public void AddFriend(string[] friends)
    {
        chatClient.AddFriends(friends);
    }

    public void DeleteFriends(string[] friends)
    {
        chatClient.RemoveFriends(friends);
    }

    void UpdateOnlineFriends(string user, int status)
    {
        if (status == 2)
        {
            friendsOnline.Add(user);
        }
        if (status == 0)
        {
            foreach (string friend in friendsOnline)
            {
                if (friend == user)
                {
                    friendsOnline.Remove(friend);
                    break;
                }
            }
        }
    }

    public void SendGameInvite(string userId, string message)
    {
        chatClient.SendPrivateMessage(userId, message);
    }

    public void Disconnect()
    {
        chatClient.Disconnect();
    }

    private IEnumerator SpawnGameInvite(string message)
    {
        while (GameObject.FindGameObjectWithTag("GameInvitePanel") != null && CardDragHandler.itemBeingDragged != null)
        {
            yield return new WaitForSeconds(1);
        }
        GameObject invitePanel = Instantiate(Resources.Load<GameObject>("GameInvitePanel"));
        invitePanel.transform.localScale = new Vector3(1, 1, 1);
        invitePanel.GetComponent<InvitedToGamePanel>().SetUpAcceptButton(message);
    }

    private IEnumerator SpawnStatusNotification(string user, int status)
    {

        var getUser = BackendFunctions.GetUser(user);
        yield return new WaitUntil(() => getUser.IsCompleted);
        if (!getUser.IsFaulted)
        {
            User tempFriend = JsonUtility.FromJson<User>(getUser.Result);
            if (!Friends.AmIBlocked(tempFriend))
            {
                if (status == 2)
                {
                    StartCoroutine(Notifications.Instance.SpawnNotification(tempFriend.userName + " has logged on"));
                }
                if (status == 0)
                {
                    StartCoroutine(Notifications.Instance.SpawnNotification(tempFriend.userName + " has logged off"));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    void OnApplicationQuit()
    {
        if (chatClient != null) { chatClient.Disconnect(); }
    }
}
