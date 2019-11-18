using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomBrowser : MonoBehaviour
{
    [SerializeField]
    GameObject roomsParent;

    public GameObject roomFullPanel;

    List<int> filters = new List<int>();
    List<GameObject> roomButtons = new List<GameObject>();

    public Button[] filterButtons;

    public void OnEnable() {
        RefreshGames();
    }

    public void RefreshGames() {

        DestroyButtons();

        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (filters.Count > 0) {
                for (int i = 0; i < filters.Count; i++) {
                    if ((int)game.CustomProperties[PhotonRooms.DeckSize()] == filters[i]) {
                        SpawnRoomButton(game);
                    }
                }
            } else {
                SpawnRoomButton(game);
            }
        }
    }

    private void SpawnRoomButton(RoomInfo game) {
        GameObject roomInfoPanel = Instantiate(Resources.Load<GameObject>("BrowserInfoPanel") as GameObject, roomsParent.transform);//instatiate a new button for each room in the rooms list
        roomInfoPanel.transform.localScale = new Vector3(1,1,1);
        roomButtons.Add(roomInfoPanel);//add the room button to the list so we can easily destroy them
        roomInfoPanel.GetComponent<RoomBrowserPanel>().SetText(game);//set the text up on each button to present the info
    }

    private void DestroyButtons() {
        foreach (GameObject button in roomButtons) {
            Destroy(button);
        }
    }

    public void SetFilter(int filterAmmount) {
        filters.Add(filterAmmount);
    }

    public void DisableButton(Button button) {
        button.interactable = false;
    }

    public void ResetFilters() {
        filters.Clear();
        for (int i = 0; i < filterButtons.Length; i++) {
            filterButtons[i].GetComponent<Button>().interactable = true;
        }
    }

}
