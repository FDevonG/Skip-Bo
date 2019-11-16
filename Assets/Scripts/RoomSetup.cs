using UnityEngine;
using UnityEngine.UI;

public class RoomSetup : MonoBehaviour
{

    public Text placeholderText;
    public Text roomNameText;
    public Text cardsText;
    public Slider cardSlider;

    private void Start() {
        UpdateCardsText();
    }

    public void UpdateCardsText() {
        cardsText.text = cardSlider.value * 5 + " Cards";
    }

    public void CreateRoom() {
        string roomName = roomNameText.text;
        if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrWhiteSpace(roomName)) {
            StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonRooms>().CreateOnlineGame(roomName, (int)cardSlider.value * 5));
        } else {
            placeholderText.text = "Enter Name";
            placeholderText.color = Color.red;
        }
    }

}
