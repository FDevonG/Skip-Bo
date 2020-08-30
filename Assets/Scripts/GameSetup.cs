using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour {

    GameControl gameControl;
    NPCCreation npcCreation;

    [SerializeField]
    GameObject mainPlayerContainer;

    public Canvas canvas;

    PhotonView photonView;

    // Start is called before the first frame update
    void Start() {
        photonView = GetComponent<PhotonView>();
        npcCreation = GetComponent<NPCCreation>();
        gameControl = GetComponent<GameControl>();
        SetGameUp();
    }

    public void SetGameUp() {
        PlayerPrefsHandler.SetGamesPlayed();
        GameObject.FindGameObjectWithTag("StatsController").GetComponent<PlayerStatsController>().AddGamePlayed();
        //if we are in an offline game we want to build the array to build out the game with and to store later for checking if the play is still connected
        if (!PhotonNetwork.offlineMode) {
            gameControl.connectedPlayers = GetPhotonPlayerArray();
        }
        if (PhotonNetwork.isMasterClient) {
            gameControl.mainDeck = BuildDeck();
        }
        PanelSetUp();
    }

    //this builds the main deck and returns it
    public List<int> BuildDeck() {
        List<int> tempDeck = new List<int>();
        for (int x = 0; x < 12; x++) {
            for (int i = 1; i < 13; i++) {
                tempDeck.Add(i);
            }
        }
        for (int t = 0; t < 18; t++) {
            tempDeck.Add(gameControl.wildNumber);
        }
        for(int i = 0; i < 3; i++)
            tempDeck = ShuffleDeck(tempDeck);//shuffle the deck after building it
        return tempDeck;
    }

    //shuffles the deck of cards
    private List<int> ShuffleDeck(List<int> cardsToBeShuffled) {
        List<int> randomList = new List<int>();//new list of to put random items in
        while (cardsToBeShuffled.Count > 0) {
            int randomIndex = UnityEngine.Random.Range(0, cardsToBeShuffled.Count); //Choose a random object in the list
            randomList.Add(cardsToBeShuffled[randomIndex]); //add it to the new, random list
            cardsToBeShuffled.RemoveAt(randomIndex); //remove to avoid duplicates
        }
        return randomList;
    }

    private PhotonPlayer[] GetPhotonPlayerArray() {
        int[] playerActorIds = new int[PhotonNetwork.playerList.Length];

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            playerActorIds[i] = PhotonNetwork.playerList[i].ID;
        }
        Array.Sort(playerActorIds);
        PhotonPlayer[] playerList = new PhotonPlayer[PhotonNetwork.playerList.Length];
        for (int x = 0; x < playerActorIds.Length; x++) {
            for (int t = 0; t < PhotonNetwork.playerList.Length; t++) {
                if (playerActorIds[x] == PhotonNetwork.playerList[t].ID) {
                    playerList[x] = PhotonNetwork.playerList[t];
                }
            }
        }

        return playerList;
    }

    private void PanelSetUp() {

        if (PhotonNetwork.offlineMode) {
            gameControl.connectedPlayers = new PhotonPlayer[PhotonNetwork.room.MaxPlayers];
            gameControl.connectedPlayers[0] = PhotonNetwork.player;
        }

        canvas.gameObject.SetActive(true);

        GameObject localPlayerPanel = PhotonNetwork.Instantiate("MainPlayerPanel", new Vector3(540, 90, 0), Quaternion.identity, 0);
        localPlayerPanel.transform.SetParent(mainPlayerContainer.transform);
        gameControl.localPlayerPanel = localPlayerPanel;
        localPlayerPanel.transform.localScale = new Vector3(1,1,1);
        localPlayerPanel.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
        localPlayerPanel.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

        gameControl.playerPanels = new GameObject[PhotonNetwork.room.MaxPlayers];

        if (PhotonNetwork.room.MaxPlayers == 4) {
            int playerPosition = 0;
            //if we are playing online we want use the array of connected players to spawn the panels
            for (int i = 0; i < gameControl.connectedPlayers.Length; i++) {
                Transform panelParent = GameObject.FindGameObjectWithTag("OtherPlayerPanel").transform;
                GameObject playerPanel = null;
                if (gameControl.connectedPlayers[i] == null) {
                    gameControl.connectedPlayers[i] = npcCreation.CreateNpc(i);
                }

                if (gameControl.connectedPlayers[i] != PhotonNetwork.player) {
                    playerPanel = Instantiate(Resources.Load<GameObject>("OtherPlayerPanel") as GameObject, panelParent);
                    playerPanel.transform.localScale = new Vector3(1, 1, 1);
                    if (!PhotonNetwork.offlineMode) {
                        playerPanel.GetComponent<PanelControl>().avatarPanel.AddComponent<OpenPlayerInfoPanel>();
                    }
                    gameControl.playerPanels[i] = playerPanel;
                    SpawnPanelInfo(playerPanel, gameControl.connectedPlayers[i], (i + 1).ToString());
                } else {
                    gameControl.playerPanels[i] = gameControl.localPlayerPanel;
                    SpawnPanelInfo(localPlayerPanel, PhotonNetwork.player, (i + 1).ToString());
                    playerPosition = i;
                }
            }
            AdjustPanels(playerPosition);
        }

        if (PhotonNetwork.room.MaxPlayers == 2) {
            for (int i = 0; i < gameControl.connectedPlayers.Length; i++) {
                Transform panelParent = GameObject.FindGameObjectWithTag("OtherPlayerPanel").transform;
                panelParent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter; 
                panelParent.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
                panelParent.parent.transform.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = true;
                GameObject playerPanel = null;
                if (gameControl.connectedPlayers[i] == null) {
                    gameControl.connectedPlayers[i] = npcCreation.CreateNpc(i);
                }

                if (gameControl.connectedPlayers[i] != PhotonNetwork.player) {
                    playerPanel = Instantiate(Resources.Load<GameObject>("MainPlayerPanel") as GameObject, panelParent);
                    playerPanel.transform.localScale = new Vector3(1, 1, 1);
                    if (!PhotonNetwork.offlineMode) {
                        playerPanel.GetComponent<PanelControl>().avatarPanel.AddComponent<OpenPlayerInfoPanel>();
                        playerPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                    playerPanel.GetComponent<PanelControl>().deck.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    playerPanel.GetComponent<PanelControl>().handSlots[0].transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    playerPanel.GetComponent<PanelControl>().sideBarSlots[0].transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    gameControl.playerPanels[i] = playerPanel;
                    SpawnPanelInfo(playerPanel, gameControl.connectedPlayers[i], (i + 1).ToString());
                } else {
                    gameControl.playerPanels[i] = gameControl.localPlayerPanel;
                    SpawnPanelInfo(localPlayerPanel, PhotonNetwork.player, (i + 1).ToString());
                }
            }
        }

        if (PhotonNetwork.isMasterClient) {
            SpawnStartingCards();
            photonView.RPC("StartGame", PhotonTargets.All, gameControl.turnIndex);
        }
    }

    //this changes the panels around so the flow of the turn order works properly
    private void AdjustPanels(int playerPosition) {
        Transform panelParent = GameObject.FindGameObjectWithTag("OtherPlayerPanel").transform;
        if (playerPosition == 1) {
            panelParent.transform.GetChild(1).SetAsFirstSibling();
            panelParent.transform.GetChild(1).SetAsLastSibling();
        }
        if (playerPosition == 2) {
            panelParent.transform.GetChild(1).SetAsLastSibling();
            panelParent.transform.GetChild(1).SetAsFirstSibling();
        }
    }

    private void SpawnPanelInfo(GameObject playerPanel, PhotonPlayer photonPlayer, string playerNumber) {
        
        PanelControl panelControl = playerPanel.GetComponent<PanelControl>();
        panelControl.photonPlayer = photonPlayer;

        panelControl.playerNumberText.text = playerNumber;
        panelControl.nameText.text = (string)panelControl.photonPlayer.CustomProperties["name"];
        panelControl.cbody.sprite = Resources.Load<Sprite>("Faces/Bodies/" + (string)panelControl.photonPlayer.CustomProperties["body"]) as Sprite;
        panelControl.cface.sprite = Resources.Load<Sprite>("Faces/Faces/" + (string)panelControl.photonPlayer.CustomProperties["face"]) as Sprite;
        panelControl.chair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + (string)panelControl.photonPlayer.CustomProperties["hair"]) as Sprite;
        panelControl.ckit.sprite = Resources.Load<Sprite>("Faces/Kits/" + (string)panelControl.photonPlayer.CustomProperties["kit"]) as Sprite;
    }

    private void SpawnStartingCards() {

        if (!PhotonNetwork.isMasterClient) {
            return;
        }
        
        //first we must loop through the array of player panels
        for (int i = 0; i < gameControl.playerPanels.Length; i++) {

            GameObject panel = gameControl.playerPanels[i];

            //second we will spawn the cards in the deck
            for (int x = 0; x < (int)PhotonNetwork.room.CustomProperties[PhotonRooms.DeckSize()]; x++) {
                GameObject card = SpawnCard();
                photonView.RPC("DeckCardSpawned", PhotonTargets.All, card.GetComponent<PhotonView>().viewID, i);//we will place the new card where it belongs on the field for all playuers in the the game
            }

            for (int x = 0; x < panel.GetComponent<PanelControl>().handSlots.Length; x++) {
                GameObject card = SpawnCard();
                card.GetComponent<Card>().handSlot = x;
                photonView.RPC("HandCardSpawned", PhotonTargets.All, card.GetComponent<PhotonView>().viewID, i);
            }

        }
    }

    private GameObject SpawnCard() {
        GameObject card = PhotonNetwork.InstantiateSceneObject("Card", new Vector3(0, 0, 0), Quaternion.identity, 0, null);//we will create the card as a scene object
        card.GetComponent<Card>().cardNumber = gameControl.mainDeck[0];//we will add the first number from the deck of cards to the card number on the new card
        gameControl.mainDeck.Remove(gameControl.mainDeck[0]);//next we will remove the card from the deck of cards
        return card;
    }

    [PunRPC]
    private IEnumerator DeckCardSpawned(int viewID, int panelIndex) {

        while (gameControl.playerPanels[panelIndex] == null) {
            yield return new WaitForSeconds(0.5f);
        }

        Transform card = PhotonView.Find(viewID).transform;

        while (card.GetComponent<Card>().cardNumber == 0) {
            yield return new WaitForSeconds(0.5f);
        }

        GameObject panel = gameControl.playerPanels[panelIndex];
        card.SetParent(panel.GetComponent<PanelControl>().deck.transform);
        card.localPosition = new Vector3(0,0,0);
        card.localScale = new Vector3(1, 1, 1);
        if (gameControl.playerPanels[panelIndex] == gameControl.localPlayerPanel || PhotonNetwork.room.MaxPlayers == 2) {
            if (panel.GetComponent<PanelControl>().deck.transform.childCount == (int)PhotonNetwork.room.CustomProperties[PhotonRooms.DeckSize()]) {
                card.gameObject.AddComponent<CardDragHandler>();
            }
        }
        if (gameControl.playerPanels[panelIndex] == gameControl.localPlayerPanel || PhotonNetwork.room.MaxPlayers == 2) {
            card.localScale = new Vector3(2, 2, 2);
        }
        if (panel.GetComponent<PanelControl>().deck.transform.childCount == (int)PhotonNetwork.room.CustomProperties[PhotonRooms.DeckSize()]) {
            card.GetComponent<Card>().SetUpCard(card.GetComponent<Card>().cardNumber);
        }
        panel.GetComponent<PanelControl>().cardsLeftText.text = (int)PhotonNetwork.room.CustomProperties[PhotonRooms.DeckSize()] + " Cards Left";
    }

    [PunRPC]
    private IEnumerator HandCardSpawned(int viewId, int panelIndex) {

        while (gameControl.playerPanels[panelIndex] == null) {
            yield return new WaitForSeconds(0.5f);
        }

        Transform card = PhotonView.Find(viewId).transform;
        while (card.GetComponent<Card>().handSlot > 5) {
            yield return new WaitForSeconds(0.5f);
        }
        GameObject panel = gameControl.playerPanels[panelIndex];
        card.SetParent(panel.GetComponent<PanelControl>().handSlots[card.GetComponent<Card>().handSlot].transform);
        card.transform.localPosition = new Vector3(0,0,0);
        card.localScale = new Vector3(1, 1, 1);
        if (gameControl.playerPanels[panelIndex] == gameControl.localPlayerPanel) {
            card.GetComponent<Card>().SetUpCard(card.GetComponent<Card>().cardNumber);
        }
        if (gameControl.playerPanels[panelIndex] == gameControl.localPlayerPanel || PhotonNetwork.room.MaxPlayers == 2) {
            card.localScale = new Vector3(2, 2, 2);
            card.gameObject.AddComponent<CardDragHandler>();
        }
    }
}
