using UnityEngine;
using UnityEngine.UI;

public class RoomSetup : MonoBehaviour {

    public Text placeholderText;
    [SerializeField] GameObject roomNameInput;
    public Text cardsText;
    public Slider cardSlider;

    [SerializeField] Toggle twoPlayerToggle;
    [SerializeField] Toggle fourPlayerToggle;

    [SerializeField] Toggle privateToggle;

    private void Start() {
        UpdateCardsText();
    }

    public void UpdateCardsText() {
        cardsText.text = cardSlider.value * 5 + " Cards";
    }

    public void CreateRoom() {
        string roomName = roomNameInput.GetComponent<InputField>().text;
        if (!string.IsNullOrEmpty(roomName) && !string.IsNullOrWhiteSpace(roomName)) {
            byte players = 0;
            if (twoPlayerToggle.isOn) {
                players = 2;
            }
            if (fourPlayerToggle.isOn) {
                players = 4;
            }
            PhotonRooms.Instance.CreateOnlineGame(roomName, ((int)cardSlider.value * 5), players, privateToggle.isOn);
        } else {
            Announcer.Instance.AnnouncerAnError();
            placeholderText.text = "Enter Name";
            placeholderText.color = Color.red;
        }
    }

    public void TwoPlayerToggle() {
        if (twoPlayerToggle.isOn) {
            fourPlayerToggle.isOn = false;
        } else {
            fourPlayerToggle.isOn = true;
        }
    }

    public void FourPlayerToggle() {
        if (fourPlayerToggle.isOn) {
            twoPlayerToggle.isOn = false;
        } else {
            twoPlayerToggle.isOn = true;
        }
    }

    void OnDisable() {
        roomNameInput.GetComponent<InputField>().text = "";
    }

}
