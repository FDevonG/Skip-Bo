using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayedCardsDropHandler : MonoBehaviour, IDropHandler {

    PhotonView photonView;
    Sounds sounds;
    GameControl gameControl;
    Announcer announcer;
    LevelSystem levelSystem;

    private void Start() {
        photonView = GetComponent<PhotonView>();
        sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
        announcer = GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>();
        levelSystem = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelSystem>();
    }

    public void OnDrop(PointerEventData eventData) {
        
        //we check to see if we are able to place this card on the stack
        if (CardDragHandler.cardNumber == GetComponent<PlayedCardStack>().currentCardValue + 1 || CardDragHandler.cardNumber == gameControl.wildNumber) {

            sounds.PlayCardFlip(CardDragHandler.itemBeingDragged.GetComponent<AudioSource>());//play the sound of the card being played

            CardDragHandler.itemBeingDragged.transform.SetParent(transform);//set the new transform parent
            CardDragHandler.itemBeingDragged.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);//zero out the dragged card in its new transform

            CardDragHandler.itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;//make the card so we cant interact with it again

            this.GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the stack

            PanelControl panelControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().localPlayerPanel.GetComponent<PanelControl>();

            if (CardDragHandler.playerDeck) {
                gameControl.CardRemovedOffDeck(panelControl);
                announcer.PayCompliment();
                StartCoroutine(levelSystem.AddExperience(15));
            }

            if (CardDragHandler.playerHand) {
                gameControl.CheckRedeal(panelControl);
                StartCoroutine(levelSystem.AddExperience(5));
            }
            if (CardDragHandler.itemBeingDragged.GetComponent<Card>().cardNumber == gameControl.wildNumber) {
                if (PhotonNetwork.isMasterClient) {
                    CardDragHandler.itemBeingDragged.GetComponent<Card>().SetUpWildCard(transform.GetComponent<PlayedCardStack>().currentCardValue);
                } else {
                    CardDragHandler.itemBeingDragged.GetComponent<Card>().SetUpWildCard(transform.GetComponent<PlayedCardStack>().currentCardValue + 1);
                }
            }

            if (CardDragHandler.playerSideBar) {
                CardDragHandler.startParent.GetComponent<SideBarDropHandler>().TurnOnCardInteraction();//turn the interaction of the next item on sidebar so we can move that one
                panelControl.ShowLowerSideBarCards(CardDragHandler.startParent);
            }

            //List<Transform> panels = transform.parent.transform.Cast<Transform>().ToList();
            int panelIndex = 0;
            for (int i = 0; i < gameControl.playedCardPanels.Count; i++) {
                if (gameControl.playedCardPanels[i].transform == transform) {
                    panelIndex = i;
                    break;
                }
            }
            if (CardDragHandler.playerDeck) {
                photonView.RPC("PlayCardFromDeck", PhotonTargets.AllViaServer, CardDragHandler.itemBeingDragged.GetComponent<PhotonView>().viewID, panelIndex, PhotonNetwork.player);
            }
            if (CardDragHandler.playerHand) {
                photonView.RPC("PlayCardFromHand", PhotonTargets.AllViaServer, CardDragHandler.itemBeingDragged.GetComponent<PhotonView>().viewID, panelIndex, PhotonNetwork.player);
            }
            if (CardDragHandler.playerSideBar) {
                photonView.RPC("PlayCardFromSideboard", PhotonTargets.AllViaServer, CardDragHandler.itemBeingDragged.GetComponent<PhotonView>().viewID, panelIndex, PhotonNetwork.player);
            }

        } else {
            announcer.Cant();
        }

    }

    [PunRPC]
    private void PlayCardFromDeck(int viewID, int panelIndex, PhotonPlayer photonPlayer) {
        if (photonPlayer != PhotonNetwork.player) {

            if (PhotonNetwork.isMasterClient) {
                GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the stack
            }

            GameObject card = PhotonView.Find(viewID).gameObject;
            PanelControl panelControl = card.transform.GetComponentInParent<PanelControl>();

            card.GetComponent<CardLerp>().StartLerping(gameControl.playedCardPanels[panelIndex].transform, new Vector3(0, 0, 0));
            card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);

            gameControl.CardRemovedOffDeck(panelControl);
        }
        if (PhotonNetwork.isMasterClient) {
            if (GetComponent<PlayedCardStack>().StackFull()) {
                photonView.RPC("ResetStack", PhotonTargets.AllViaServer);
            }
        }
    }

    [PunRPC]
    private void PlayCardFromHand(int viewID, int panelIndex, PhotonPlayer photonPlayer) {
        if (photonPlayer != PhotonNetwork.player) {

            if (PhotonNetwork.isMasterClient) {
                GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the stack
            }

            GameObject card = PhotonView.Find(viewID).gameObject;
            Transform startParent = card.transform.parent.transform;
            card.GetComponent<CardLerp>().StartLerping(gameControl.playedCardPanels[panelIndex].transform, new Vector3(0, 0, 0));
            card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);

            gameControl.CheckRedeal(startParent.GetComponentInParent<PanelControl>());
        }
        if (PhotonNetwork.isMasterClient) {
            if (GetComponent<PlayedCardStack>().StackFull()) {
                photonView.RPC("ResetStack", PhotonTargets.AllViaServer);
            }
        }
    }

    [PunRPC]
    private IEnumerator PlayCardFromSideboard(int viewID, int panelIndex, PhotonPlayer photonPlayer) {
        if (photonPlayer != PhotonNetwork.player) {

            if (PhotonNetwork.isMasterClient) {
                GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the stack
            }

            GameObject card = PhotonView.Find(viewID).gameObject;
            PanelControl panelControl = card.GetComponentInParent<PanelControl>();
            Transform startParentPanel = card.transform.parent;

            card.GetComponent<CardLerp>().StartLerping(gameControl.playedCardPanels[panelIndex].transform, new Vector3(0, 0, 0));

            yield return new WaitForSeconds(0.5f);
            panelControl.ShowLowerSideBarCards(startParentPanel);
        }

        if (PhotonNetwork.isMasterClient) {
            if (GetComponent<PlayedCardStack>().StackFull()) {
                photonView.RPC("ResetStack", PhotonTargets.AllViaServer);
            }
        }
    }

    [PunRPC]
    private IEnumerator ResetStack() {
        GameObject playerPanel = gameControl.localPlayerPanel;
        playerPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        yield return StartCoroutine(GetComponent<PlayedCardStack>().ResetStack());
        if (gameControl.playerPanels[gameControl.turnIndex] == playerPanel) {
            playerPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void DoubleClickedSideboard(int viewID, int panelIndex) {
        photonView.RPC("PlayCardFromSideboard", PhotonTargets.AllViaServer, viewID, panelIndex, PhotonNetwork.player);
    }

}
