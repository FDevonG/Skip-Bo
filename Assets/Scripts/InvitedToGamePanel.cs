using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InvitedToGamePanel : MonoBehaviour
{
    int currCountdownValue;
    int storedTime = 11;
    [SerializeField] Button closePanelButton;
    [SerializeField] Button acceptInviteButton;
    [SerializeField] Text inviteText;

    private void Start() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartCountdown());
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>().PlayGameInvite();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(StartCountdown());
    }

    public IEnumerator StartCountdown(int countdownValue = 10) {
        if (storedTime == 11) {
            currCountdownValue = countdownValue;
        } else {
            currCountdownValue = storedTime;
        }
        
        while (currCountdownValue > 0) {
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
            storedTime = currCountdownValue;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }

    public void SetUpAcceptButton(string message) {
        string[] messages = message.Split("@"[0]);
        string roomName = messages[0];
        string userName = messages[1];
        inviteText.text = userName + " has invited you to play";
        acceptInviteButton.onClick.AddListener(() => StartCoroutine(JoinRoom(roomName)));
        closePanelButton.onClick.AddListener(() => Destroy(gameObject));
    }

    IEnumerator JoinRoom(string roomName) {
        if (PhotonNetwork.room != null) {
            PhotonNetwork.LeaveRoom();
        }
        yield return new WaitUntil(() => PhotonNetwork.connectedAndReady);
        bool roomFound = false;
        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (game.Name == roomName) {
                roomFound = true;
                if (game.IsOpen) {
                    StopAllCoroutines();
                    PhotonNetwork.JoinRoom(game.Name);
                    SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
