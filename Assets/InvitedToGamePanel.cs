using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvitedToGamePanel : MonoBehaviour
{

    [SerializeField] Button closePanelButton;
    [SerializeField] Button acceptInviteButton;
    [SerializeField] Text inviteText;

    // Start is called before the first frame update
    void Start()
    {
        closePanelButton.onClick.AddListener(DestroyThisPanel);
    }

    public void SetUpAcceptButton(string message) {
        string[] messages = message.Split("@"[0]);
        string roomName = messages[0];
        string userName = messages[1];
        inviteText.text = userName + " has invited you to play";
        acceptInviteButton.onClick.AddListener(() => JoinRoom(roomName));
    }

    void JoinRoom(string roomName) {
        StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonRooms>().JoinRoom(roomName));
        DestroyThisPanel();
    }

    void DestroyThisPanel() {
        Destroy(this.gameObject);
    }
}
