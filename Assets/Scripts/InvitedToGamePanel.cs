using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvitedToGamePanel : MonoBehaviour
{

    [SerializeField] Button closePanelButton;
    [SerializeField] Button acceptInviteButton;
    [SerializeField] Text inviteText;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void SetUpAcceptButton(string message) {
        string[] messages = message.Split("@"[0]);
        string roomName = messages[0];
        string userName = messages[1];
        inviteText.text = userName + " has invited you to play";
        acceptInviteButton.onClick.AddListener(() => JoinRoom(roomName));
        closePanelButton.onClick.AddListener(() => Destroy(gameObject));
    }

    void JoinRoom(string roomName) {
        bool roomFound = false;
        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (game.Name == roomName) {
                roomFound = true;
                if (game.IsOpen) {
                    PhotonNetwork.JoinRoom(game.Name);
                    Destroy(gameObject);
                } else {
                    inviteText.text = "Room Is Full";
                }
            }
        }
        if (!roomFound) {
            inviteText.text = "Room Not Found";
        }
    }

    private void Update() {
        if (transform.parent == null) {
            if (GameObject.Find("Canvas") != null) {
                transform.SetParent(GameObject.Find("Canvas").transform);
            }
        }
    }
}
