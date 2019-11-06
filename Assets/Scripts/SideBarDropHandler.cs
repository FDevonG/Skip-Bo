using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SideBarDropHandler : MonoBehaviour, IDropHandler {

    PhotonView photonView;
    Sounds sounds;
    GameControl gameControl;

    private void Start() {
        photonView = GetComponent<PhotonView>();
        sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
    }

    public void OnDrop(PointerEventData eventData) {
        if (!CardDragHandler.playerDeck && !CardDragHandler.playerSideBar)//check to make sure the card is not from the player deck or sidebar
        {
            sounds.PlayCardFlip(CardDragHandler.itemBeingDragged.GetComponent<AudioSource>());
            CardDragHandler.itemBeingDragged.transform.SetParent(transform);//set the new transform

            PanelControl panelControl = gameControl.localPlayerPanel.GetComponent<PanelControl>();

            int numberOfChildren = transform.childCount;
            if (numberOfChildren > 0) {
                CardDragHandler.itemBeingDragged.transform.localPosition = new Vector3(0, (numberOfChildren - 1) * -60, 0);
            }
            if (numberOfChildren > 3) {
                CardDragHandler.itemBeingDragged.transform.localPosition = new Vector3(0, 3 * -60, 0);
            }

            CardDragHandler.itemBeingDragged.AddComponent<SideBoardDoubleClick>();

            //if we get to many cards in the same sytack showing they are going to get to be to much and take up to much screen real estate
            panelControl.HideLowerSideBarCards(transform);//we are going to hide the lower cards beneath newer cards so we do not run out of screen real estate

            TurnOffCardInteractions();

            int panelIndex = 0;
            for (int i = 0; i < panelControl.sideBarSlots.Length; i++) {
                if (panelControl.sideBarSlots[i].transform == transform) {
                    panelIndex = i;
                    break;
                }
            }
            photonView.RPC("RequestSideBoadPlay", PhotonTargets.Others, CardDragHandler.itemBeingDragged.GetComponent<PhotonView>().viewID, panelIndex, gameControl.turnIndex);
            gameControl.ChangeTurn();
        }
    }

    //this is called from the clients to the paster client
    [PunRPC]
    private IEnumerator RequestSideBoadPlay(int viewID, int sideBoardIndex, int turnIndex) {
        //we need to start the card lerps
        //remove the card from the players hand
        //change the turn
        PanelControl panelControl =  gameControl.playerPanels[turnIndex].GetComponent<PanelControl>();
        GameObject card = PhotonView.Find(viewID).gameObject;

        Vector3 endPos = new Vector3(0, 0, 0);

        int numberOfChildren = panelControl.sideBarSlots[sideBoardIndex].transform.childCount;
        if (numberOfChildren > 0) {
            endPos = new Vector3(0, numberOfChildren * -30, 0);
        }
        if (numberOfChildren > 3) {
            endPos = new Vector3(0, 3 * -30, 0);
        }
        card.GetComponent<CardLerp>().StartLerping(panelControl.sideBarSlots[sideBoardIndex].transform, endPos);
        card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);

        gameControl.ChangeTurn();

        yield return new WaitForSeconds(1.0f);
        panelControl.HideLowerSideBarCards(panelControl.sideBarSlots[sideBoardIndex].transform);
    }

    //this function turns the interections with the cards beneath the top card so we cant move them
    public void TurnOffCardInteractions() {
        if (transform.childCount > 1) {
            transform.GetChild(transform.childCount - 2).GetComponent<CanvasGroup>().blocksRaycasts = false;
        } 
    }
    public void TurnOnCardInteraction() {
        if (transform.childCount != 0) {
            transform.GetChild(transform.childCount - 1).GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
