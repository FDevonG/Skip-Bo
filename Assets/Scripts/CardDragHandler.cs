using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public static GameObject itemBeingDragged;
    private Vector3 startPosition = new Vector3();//used to store the position we started in
    public static bool playerDeck = false;//a boolean to see if the card is coming form the player deck, if so we can place it on the sidebar
    public static bool playerHand = false;
    public static int cardNumber;
    public static bool playerSideBar = false;
    public static Transform startParent;

    GameSetup gameSetup;

    private void Start() {
        gameSetup = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameSetup>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (itemBeingDragged != null) {
            return;
        }
        itemBeingDragged = this.gameObject;
        itemBeingDragged.GetComponent<CanvasGroup>().blocksRaycasts = false;
        startPosition = this.transform.localPosition;
        cardNumber = this.GetComponent<Card>().cardNumber;
        startParent = transform.parent.transform;

        if (transform.parent.gameObject == transform.GetComponentInParent<PanelControl>().deck) {
            playerDeck = true;
        }
        for (int i = 0; i < transform.GetComponentInParent<PanelControl>().handSlots.Length; i++) {
            if (transform.parent.gameObject == transform.GetComponentInParent<PanelControl>().handSlots[i]) {
                playerHand = true;
                break;
            }
        }
        if (transform.parent.GetComponent<SideBarDropHandler>()) {
            playerSideBar = true;
            transform.parent.GetComponent<SideBarDropHandler>().TurnOffCardInteractions();
        }

        transform.SetParent(gameSetup.canvas.transform);
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {

        if (transform.parent == gameSetup.canvas.transform) {
            transform.SetParent(startParent);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.transform.localPosition = startPosition;//return it to the starting position
        }
        if (transform.parent.GetComponent<SideBarDropHandler>()) {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.parent.GetComponent<SideBarDropHandler>().TurnOffCardInteractions();
        }

        itemBeingDragged = null;
        playerDeck = false;
        playerHand = false;
        playerSideBar = false;
    }
}
