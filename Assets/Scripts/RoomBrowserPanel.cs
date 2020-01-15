using UnityEngine;
using UnityEngine.UI;

public class RoomBrowserPanel : MonoBehaviour
{

    public Text nameText;
    public Text cardsText;
    public Text playersText;
    RoomInfo gameInfo;

    public void SetText(RoomInfo game) {
        nameText.text = game.Name;
        cardsText.text = game.CustomProperties[PhotonRooms.DeckSize()].ToString();
        playersText.text = game.MaxPlayers.ToString();
        gameInfo = game;
    }

    public void JoinRoom() {

        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (game.Name == gameInfo.Name) {
                if (game.IsOpen) {
                    PhotonNetwork.JoinRoom(game.Name);
                } else {
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GetComponentInParent<RoomBrowser>().roomFullPanel);
                }
            }
        }
    }
}
