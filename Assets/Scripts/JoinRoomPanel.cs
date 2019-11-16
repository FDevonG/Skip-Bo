using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomPanel : MonoBehaviour
{
    [SerializeField]
    Text infoTex;
    [SerializeField]
    Text roomNameText;

    public void JoinRoom() {
        bool roomFound = false;
        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (game.Name == roomNameText.text) {
                roomFound = true;
                if (game.IsOpen) {
                    PhotonNetwork.JoinRoom(game.Name);
                } else {
                    infoTex.text = "Room Is Full";
                }
            }
        }
        if (!roomFound) {
            infoTex.text = "Room Not Found";
        }
    }

}
