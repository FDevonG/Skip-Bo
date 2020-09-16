using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    PhotonView photonView;

    AdManager adManager;

    [SerializeField] GameSetup gameSetup;
    NPCTurn npcTurn;
    Announcer announcer;

    public List<int> mainDeck;
    public int wildNumber = 100;

    public int turnIndex;

    public PhotonPlayer[] connectedPlayers = new PhotonPlayer[0];
    public GameObject[] playerPanels;

    public List<GameObject> playedCardPanels = new List<GameObject>();

    public GameObject localPlayerPanel;

    public bool playerWon = false;

    public Vector3 cardSpawnLocation = new Vector3(-600, 0, 0);

    public int deckCardsPlayed = 0;

    private void Start() {
        npcTurn = GetComponent<NPCTurn>();
        announcer = GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>();
        photonView = GetComponent<PhotonView>();
        adManager = GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>();
    }

    public void PlayedDeckCardCount()
    {
        if (!PhotonNetwork.offlineMode)
            return;

        deckCardsPlayed++;
        if (deckCardsPlayed == 10 && !playerWon)
        {
            StartCoroutine(adManager.ShowRegularAd());
            deckCardsPlayed = 0;
        }
    }

    [PunRPC]
    public IEnumerator StartGame(int turnIndex) {
        while (playerPanels[turnIndex] == null) {
            yield return new WaitForSeconds(0.1f);
        }

        if (PhotonNetwork.isMasterClient) {
            turnIndex = Random.Range(0, PhotonNetwork.room.MaxPlayers);
            photonView.RPC("SetupStartingAvatar", PhotonTargets.All, turnIndex);
            DetermineNPCTurn();
        }
    }

    [PunRPC]
    private IEnumerator SetupStartingAvatar(int turn) {
        turnIndex = turn;
        while (playerPanels[turnIndex] == null) {
            yield return new WaitForSeconds(0.5f);
        }
        announcer.AnnouncePlayerTurn(turnIndex);
        if (playerPanels[turnIndex] == localPlayerPanel) {
            localPlayerPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        } 
        playerPanels[turnIndex].GetComponent<PanelControl>().avatarPanel.GetComponent<ColorLerp>().StartColorLerp(true, false);
    }

    //this checks to see if we need to redeal the player hand
    public void CheckRedeal(PanelControl panelControl) {
        if (!PhotonNetwork.isMasterClient) {
            return;
        }
        if (PhotonNetwork.isMasterClient) {
            bool redeal = true;
            for (int i = 0; i < panelControl.handSlots.Length; i++) {
                if (panelControl.handSlots[i].transform.childCount != 0) {
                    redeal = false;
                    break;
                }
            }
            if (redeal) {
                StartCoroutine(DealPlayerHand(panelControl));
            }
        }
    }

    public IEnumerator DealPlayerHand(PanelControl panelControl) {

        if (!PhotonNetwork.isMasterClient) {
            yield return null;
        }

        if (PhotonNetwork.isMasterClient) {
            for (int i = 0; i < panelControl.handSlots.Length; i++) {
                if (mainDeck.Count == 0) {
                    if (PhotonNetwork.inRoom) {
                        if (PhotonNetwork.isMasterClient) {
                            mainDeck = gameSetup.BuildDeck();//if the main deck is out of cards shuffle the discard deck
                        }
                    } else {
                        mainDeck = gameSetup.BuildDeck();//if the main deck is out of cards shuffle the discard deck
                    }
                }

                if (panelControl.handSlots[i].transform.childCount == 0) {

                    GameObject dealtCard = PhotonNetwork.InstantiateSceneObject("Card", cardSpawnLocation, Quaternion.identity, 0, null);
                    dealtCard.GetComponent<Card>().cardNumber = mainDeck[0];
                    mainDeck.Remove(mainDeck[0]);//remove card from main deck
                    dealtCard.GetComponent<Card>().handSlot = i;
                    //we tell the players to move the spawned card to the appropriate hand slot for that proper player
                    photonView.RPC("SpawnedPlayerHand", PhotonTargets.All, dealtCard.GetComponent<PhotonView>().viewID, turnIndex);
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }
    }

    [PunRPC]
    private IEnumerator SpawnedPlayerHand(int viewID, int turnIn) {
        GameObject card = PhotonView.Find(viewID).gameObject;
        PanelControl panelControl = playerPanels[turnIn].GetComponent<PanelControl>();
        while (card.GetComponent<Card>().handSlot > 5) {
            yield return new WaitForSeconds(0.1f);
        }
        card.transform.SetParent(gameSetup.canvas.transform);
        card.transform.localPosition = cardSpawnLocation;
        card.transform.localScale = new Vector3(1, 1, 1);
        card.GetComponent<CardLerp>().StartLerping(panelControl.handSlots[card.GetComponent<Card>().handSlot].transform, new Vector3(0, 0, 0));//start the cards animating out to the players
        if (playerPanels[turnIn] == localPlayerPanel) {
            card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);
            card.AddComponent<CardDragHandler>();
        }
    }

    public void ChangeTurn() {

        localPlayerPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        playerPanels[turnIndex].GetComponent<PanelControl>().avatarPanel.GetComponent<ColorLerp>().StartColorLerp(false, true);

        if (PhotonNetwork.isMasterClient) {
            if (turnIndex == playerPanels.Length - 1) {//make sure the turn index doesnt go out of range and set to proper number
                turnIndex = 0;
            } else {
                turnIndex += 1;
            }
            StartCoroutine(DealPlayerHand(playerPanels[turnIndex].GetComponent<PanelControl>()));
            photonView.RPC("ChangePlayerPanel", PhotonTargets.All, turnIndex);
            DetermineNPCTurn();
        }
    }

    [PunRPC]
    private void ChangePlayerPanel (int newTurn) {

        announcer.AnnouncePlayerTurn(newTurn);
        if (playerPanels[newTurn] == localPlayerPanel) {
            localPlayerPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        playerPanels[newTurn].GetComponent<PanelControl>().avatarPanel.GetComponent<ColorLerp>().StartColorLerp(true, false);
    }

    public void DetermineNPCTurn() {
        if (PhotonNetwork.offlineMode) {
            if (playerPanels[turnIndex] != localPlayerPanel) {
                npcTurn.StartNPCTurn();
            }
        }

        //here we are going to check to see if the current players turn is still connected, if not we will run the npc turn script
        if (!PhotonNetwork.offlineMode) {
            bool playerConnected = false;
            //first we loop through the player panels
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
                if (playerPanels[turnIndex].GetComponent<PanelControl>().photonPlayer == PhotonNetwork.playerList[i]) {
                    playerConnected = true;
                    break;
                }
            }
            if (!playerConnected) {
                npcTurn.StartNPCTurn();
            }
        }
    }

    public void CardRemovedOffDeck(PanelControl panelControl) {
        panelControl.cardsLeftText.text = panelControl.deck.transform.childCount + " Cards";
        if (panelControl.deck.transform.childCount == 0) {
            if (PhotonNetwork.isMasterClient) {
                playerWon = true;
                photonView.RPC("PlayerWonGame", PhotonTargets.All);
            }
        } else {
            GameObject nextDeckCard = panelControl.deck.transform.GetChild(panelControl.deck.transform.childCount - 1).gameObject;
            nextDeckCard.GetComponent<CardScaleLerp>().StartScale(nextDeckCard.GetComponent<Card>().cardNumber);
        }
    }

    [PunRPC]
    public void PlayerWonGame() {
        localPlayerPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        playerWon = true;
        StartCoroutine(GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().Victory());
        if (PhotonNetwork.isMasterClient) {
            GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().playerStandings = GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().GetPlayerStandings();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {

            //sync the main deck
            int[] deckToSend = new int[mainDeck.Count];
            for (int i = 0; i < mainDeck.Count; i++) {
                deckToSend[i] = mainDeck[i];
            }
            stream.SendNext(deckToSend);

            //stream.SendNext(connectedPlayers);

            stream.SendNext(playerWon);

            stream.SendNext(turnIndex);

        } else {

            //sync the main deck
            int[] tempDeck = (int[])stream.ReceiveNext();
            mainDeck.Clear();
            for (int i = 0; i < tempDeck.Length; i++) {
                mainDeck.Add(tempDeck[i]);
            }

            //connectedPlayers = (PhotonPlayer[])stream.ReceiveNext();

            playerWon = (bool)stream.ReceiveNext();

            turnIndex = (int)stream.ReceiveNext();
        }
    }

}
