using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCTurn : MonoBehaviour {
    GameControl gameControl;
    private PhotonView photonView;
    public PanelControl currentPlayerPanel;
    public delegate void npcTurnDelegate();

    string npcNetworkDeck = "CardPlayedFromDeck";
    string npcNetworkHand = "PlayedCardFromHand";
    string npcNetworkToSideboard = "PlayedCardToSideboard";

    private void OnEnable() {
        gameControl = GetComponent<GameControl>();
        photonView = GetComponent<PhotonView>();
    }

    public void StartNPCTurn() {
        if (PhotonNetwork.isMasterClient) {
            currentPlayerPanel = gameControl.playerPanels[gameControl.turnIndex].GetComponent<PanelControl>();
            npcTurnDelegate checkPlayerDeck = CheckPLayerDeck;
            StartCoroutine(Wait(checkPlayerDeck, 1));
        }
    }

    private void CheckPLayerDeck() {
        if (!gameControl.playerWon) {
            bool cardPlayed = false;
            if (currentPlayerPanel.deck.transform.childCount == 0) {
                return;
            }
            int deckCardNumber = currentPlayerPanel.deck.transform.GetChild(currentPlayerPanel.deck.transform.childCount - 1).GetComponent<Card>().cardNumber;

            if (deckCardNumber != gameControl.wildNumber) {//if it isnt a wild card
                for (int i = 0; i < gameControl.playedCardPanels.Count; i++) {//loop through the played card panels
                    if (deckCardNumber == gameControl.playedCardPanels[i].GetComponent<PlayedCardStack>().currentCardValue + 1) {//if the values line up
                        if (PhotonNetwork.isMasterClient) {
                            photonView.RPC(npcNetworkDeck, PhotonTargets.All, i);
                        } 
                        cardPlayed = true;//set the card played to true
                        break;
                    }
                }
            } else if (deckCardNumber == gameControl.wildNumber) {

                //we will check the player hand to see if we can play cards from the hand before playing the wild
                for (int y = 0; y < currentPlayerPanel.handSlots.Length; y++) {//loop through thr player hand
                    for (int u = 0; u < gameControl.playedCardPanels.Count; u++) {
                        if (currentPlayerPanel.handSlots[y].transform.childCount != 0) {
                            if (currentPlayerPanel.handSlots[y].transform.GetChild(0).GetComponent<Card>().cardNumber == gameControl.playedCardPanels[u].GetComponent<PlayedCardStack>().currentCardValue + 1) {
                                if (PhotonNetwork.isMasterClient) {
                                    photonView.RPC(npcNetworkHand, PhotonTargets.All, u, currentPlayerPanel.handSlots[y].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                                }
                                cardPlayed = true;
                                break;
                            }
                        }
                    }
                    if (cardPlayed) {
                        break;
                    }
                }

                //we will lopp throught the players hand looking for cards we can play after placing the wild
                if (!cardPlayed) {
                    for (int x = 0; x < currentPlayerPanel.handSlots.Length; x++) {//loop through the curent players cards in hand
                        for (int t = 0; t < gameControl.playedCardPanels.Count; t++) {
                            if (currentPlayerPanel.handSlots[x].transform.childCount != 0) {
                                if (currentPlayerPanel.handSlots[x].transform.GetChild(0).GetComponent<Card>().cardNumber == gameControl.playedCardPanels[t].GetComponent<PlayedCardStack>().currentCardValue + 2) {//if you have a card in your hand that is 2 above the current value of the stack then lay the wild card there
                                    if (PhotonNetwork.isMasterClient) {
                                        photonView.RPC(npcNetworkDeck, PhotonTargets.All, t);
                                    } 
                                    cardPlayed = true;
                                    break;
                                }
                            }
                        }
                        if (cardPlayed) {
                            break;
                        }
                    }
                }
                if (!cardPlayed) {
                    int randomPanelIndex = RandomInt(0, gameControl.playedCardPanels.Count - 1);//if the card is a wild card select a random panel to play the card to
                    while (gameControl.playedCardPanels[randomPanelIndex].GetComponent<PlayedCardStack>().currentCardValue == gameControl.playedCardPanels[randomPanelIndex].GetComponent<PlayedCardStack>().maxStackSize) {//make sure we are not selecting one that is being reset
                        randomPanelIndex = RandomInt(0, gameControl.playedCardPanels.Count - 1);//if the card is a wild card select a random panel to play the card to
                    }
                    if (PhotonNetwork.isMasterClient) {
                        photonView.RPC(npcNetworkDeck, PhotonTargets.All, randomPanelIndex);
                    } 
                    cardPlayed = true;
                }
            }
            if (!cardPlayed) {
                npcTurnDelegate checkPlayerHand = CheckPlayerHand;
                StartCoroutine(Wait(checkPlayerHand, 0.5f));
            }
        }
    }

    [PunRPC]
    private IEnumerator CardPlayedFromDeck(int panelIndex) {
        currentPlayerPanel = gameControl.playerPanels[gameControl.turnIndex].GetComponent<PanelControl>();
        Transform panelTransform = gameControl.playedCardPanels[panelIndex].transform;
        GameObject deckCard = currentPlayerPanel.deck.transform.GetChild(currentPlayerPanel.deck.transform.childCount - 1).gameObject;

        panelTransform.GetComponent<PlayedCardStack>().AddCardToStack();//add the played card to the stack
        deckCard.GetComponent<CardLerp>().StartLerping(panelTransform, new Vector3(0, 0, 0));

        if (deckCard.GetComponent<Card>().cardNumber == gameControl.wildNumber) {
            deckCard.GetComponent<Card>().SetUpWildCard(panelTransform.GetComponent<PlayedCardStack>().currentCardValue);
        }

        gameControl.CardRemovedOffDeck(currentPlayerPanel);

        if (panelTransform.GetComponent<PlayedCardStack>().StackFull()) {
            yield return StartCoroutine(panelTransform.GetComponent<PlayedCardStack>().ResetStack());
        }
        if (PhotonNetwork.isMasterClient) {
            npcTurnDelegate checkPlayerDeck = CheckPLayerDeck;
            StartCoroutine(Wait(checkPlayerDeck, 0.5f));
        }
    }

    private void CheckPlayerHand() {
        if (!gameControl.playerWon) {
            bool cardPlayed = false;
            bool wildCardInHand = false;
            int wildCardIndex = 0;

            for (int i = 0; i < currentPlayerPanel.handSlots.Length; i++) {//loop through the cards in the player hand
                if (currentPlayerPanel.handSlots[i].transform.childCount != 0) {
                    if (currentPlayerPanel.handSlots[i].transform.GetChild(0).GetComponent<Card>().cardNumber == gameControl.wildNumber) {//if the card is a wild card
                        wildCardIndex = i;//set varible to the index of the wild card
                        wildCardInHand = true;//set the wildcard boolean to true signifying that there is a wild card in the hand
                    }
                }
                for (int x = 0; x < gameControl.playedCardPanels.Count; x++) {//loop through the played card panels
                    if (currentPlayerPanel.handSlots[i].transform.childCount != 0) {
                        if (currentPlayerPanel.handSlots[i].transform.GetChild(0).GetComponent<Card>().cardNumber == gameControl.playedCardPanels[x].GetComponent<PlayedCardStack>().currentCardValue + 1) {//if the current card in the hand is one number higher than the current card stack, play the card
                            if (!cardPlayed) {
                                cardPlayed = true;//set the cardPlayed bool to true so we can skip code
                                if (PhotonNetwork.isMasterClient) {
                                    photonView.RPC(npcNetworkHand, PhotonTargets.All, x, currentPlayerPanel.handSlots[i].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                                }
                                break;
                            }
                        }
                    }
                }
                if (cardPlayed) {
                    break;
                }
                if (!cardPlayed && wildCardInHand) {//if they havent played a card yet and there is a wild card
                    //we will check the player sidebar before playing the wild card
                    for (int v = 0; v < currentPlayerPanel.sideBarSlots.Length; v++) {//loop through the players side bar panels
                        if (!cardPlayed) {
                            if (currentPlayerPanel.sideBarSlots[v].transform.childCount > 0) {//if the sidebar has children
                                int sideBoardCardValue = currentPlayerPanel.sideBarSlots[v].transform.GetChild(currentPlayerPanel.sideBarSlots[v].transform.childCount - 1).GetComponent<Card>().cardNumber;//get the last card in the panel and store the value of it
                                for (int x = 0; x < gameControl.playedCardPanels.Count; x++) {//loop through the played card stacks
                                    if (sideBoardCardValue == gameControl.playedCardPanels[x].GetComponent<PlayedCardStack>().currentCardValue + 1) {//if thesideboard card value is 1 higher then then current played stack play the card off the sideboard panel to the stack
                                        if (!cardPlayed) {
                                            cardPlayed = true;
                                            if (PhotonNetwork.isMasterClient) {
                                                photonView.RPC("PlayCardFromSideboard", PhotonTargets.All, x, currentPlayerPanel.sideBarSlots[v].transform.GetChild(currentPlayerPanel.sideBarSlots[v].transform.childCount - 1).GetComponent<PhotonView>().viewID);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!cardPlayed) {
                        //first we will loop through the stacks and see if we could play the card on top of stock pile if we played a wild card on that stack
                        int stockPileNumber = currentPlayerPanel.deck.transform.GetChild(currentPlayerPanel.deck.transform.childCount - 1).GetComponent<Card>().cardNumber;
                        for (int t = 0; t < gameControl.playedCardPanels.Count; t++) {
                            if (gameControl.playedCardPanels[t].GetComponent<PlayedCardStack>().currentCardValue + 2 == stockPileNumber) {
                                if (!cardPlayed) {
                                    cardPlayed = true;
                                    if (PhotonNetwork.isMasterClient) {
                                        photonView.RPC(npcNetworkHand, PhotonTargets.All, t, currentPlayerPanel.handSlots[wildCardIndex].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (!cardPlayed) {
                        //second we will lopp through the players hand and the stacks seeing if we can use the wild card to then place a second card from the hand
                        for (int t = 0; t < currentPlayerPanel.handSlots.Length; t++) {
                            for (int g = 0; g < gameControl.playedCardPanels.Count; g++) {
                                if (currentPlayerPanel.handSlots[t].transform.childCount != 0) {
                                    if (currentPlayerPanel.handSlots[t].transform.GetChild(0).GetComponent<Card>().cardNumber == gameControl.playedCardPanels[g].GetComponent<PlayedCardStack>().currentCardValue + 2) {
                                        if (!cardPlayed) {
                                            cardPlayed = true;
                                            if (PhotonNetwork.isMasterClient) {
                                                photonView.RPC(npcNetworkHand, PhotonTargets.All, g, currentPlayerPanel.handSlots[wildCardIndex].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            if (cardPlayed) {
                                break;
                            }
                        }
                    }
                    if (cardPlayed) {
                        break;
                    }
                    if (!cardPlayed) {
                        //if we couldnt find a card in the hand to play after the wild card then we will find the stack closest to the value of the card on top of the player deck and play it there
                        int deckCardNumber = currentPlayerPanel.deck.transform.GetChild(currentPlayerPanel.deck.transform.childCount - 1).GetComponent<Card>().cardNumber;//used to store the value of the players top card in deck
                        int highestStackValue = 0;//used to keep track of the highest stack
                        int panelIndex = 0;//used to track the panel index with the highest value
                        for (int d = 0; d < gameControl.playedCardPanels.Count; d++) {
                            if (gameControl.playedCardPanels[d].GetComponent<PlayedCardStack>().currentCardValue < deckCardNumber && highestStackValue < gameControl.playedCardPanels[d].GetComponent<PlayedCardStack>().currentCardValue) {
                                highestStackValue = gameControl.playedCardPanels[d].GetComponent<PlayedCardStack>().currentCardValue;
                                panelIndex = d;
                            }
                        }
                        cardPlayed = true;
                        if (PhotonNetwork.isMasterClient) {
                            photonView.RPC(npcNetworkHand, PhotonTargets.All, panelIndex, currentPlayerPanel.handSlots[wildCardIndex].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                        }
                    }
                }
            }
            if (!cardPlayed) {
                npcTurnDelegate checkPlayerSideboard = CheckPlayerSideboard;
                StartCoroutine(Wait(checkPlayerSideboard, 0.5f));
            }
        }
    }

    [PunRPC]
    private IEnumerator PlayedCardFromHand(int panelIndex, int cardViewID) {
        currentPlayerPanel = gameControl.playerPanels[gameControl.turnIndex].GetComponent<PanelControl>();
        Transform newParent = gameControl.playedCardPanels[panelIndex].transform;
        newParent.GetComponent<PlayedCardStack>().AddCardToStack();//add the played card to the stack

        GameObject card = PhotonView.Find(cardViewID).gameObject;

        card.GetComponent<CardLerp>().StartLerping(newParent, new Vector3(0, 0, 0));//set the card from the hand to start lerping towards the panel
        card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);//start the card scaling lerp

        gameControl.CheckRedeal(currentPlayerPanel);//check to see if we need to redeal the player hand

        if (newParent.GetComponent<PlayedCardStack>().StackFull()) {
            yield return StartCoroutine(newParent.GetComponent<PlayedCardStack>().ResetStack());
        }
        if (PhotonNetwork.isMasterClient) {
            npcTurnDelegate checkPlayerDeck = CheckPLayerDeck;
            StartCoroutine(Wait(checkPlayerDeck, 0.5f));
        }
    }

    private void CheckPlayerSideboard() {

        if (!gameControl.playerWon) {
            bool cardPlayed = false;

            for (int i = 0; i < currentPlayerPanel.sideBarSlots.Length; i++) {//loop through the players side bar panels
                if (!cardPlayed) {
                    if (currentPlayerPanel.sideBarSlots[i].transform.childCount > 0) {//if the sidebar has children
                        int sideBoardCardValue = currentPlayerPanel.sideBarSlots[i].transform.GetChild(currentPlayerPanel.sideBarSlots[i].transform.childCount - 1).GetComponent<Card>().cardNumber;//get the last card in the panel and store the value of it
                        for (int x = 0; x < gameControl.playedCardPanels.Count; x++) {//loop through the played card stacks
                            if (sideBoardCardValue == gameControl.playedCardPanels[x].GetComponent<PlayedCardStack>().currentCardValue + 1) {//if thesideboard card value is 1 higher then then current played stack play the card off the sideboard panel to the stack
                                if (!cardPlayed) {
                                    cardPlayed = true;
                                    if (PhotonNetwork.isMasterClient) {
                                        photonView.RPC("PlayCardFromSideboard", PhotonTargets.All, x, currentPlayerPanel.sideBarSlots[i].transform.GetChild(currentPlayerPanel.sideBarSlots[i].transform.childCount - 1).GetComponent<PhotonView>().viewID);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (!cardPlayed) {
                npcTurnDelegate playCardToSideboard = PlayCardToSideboard;
                StartCoroutine(Wait(playCardToSideboard, 0.5f));
            }
        }
    }

    [PunRPC]
    private IEnumerator PlayCardFromSideboard(int panelIndex, int cardViewID) {
        currentPlayerPanel = gameControl.playerPanels[gameControl.turnIndex].GetComponent<PanelControl>();
        GameObject cardPlayed = PhotonView.Find(cardViewID).gameObject;
        Transform newParent = gameControl.playedCardPanels[panelIndex].transform;
        Transform startParent = cardPlayed.transform.parent.transform;

        newParent.GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the played stack
        cardPlayed.GetComponent<CardLerp>().StartLerping(newParent, new Vector3(0, 0, 0));

        yield return new WaitForSeconds(0.2f);
        currentPlayerPanel.ShowLowerSideBarCards(startParent);

        if (newParent.GetComponent<PlayedCardStack>().StackFull()) {
            yield return StartCoroutine(newParent.GetComponent<PlayedCardStack>().ResetStack());
        }

        if (PhotonNetwork.inRoom) {
            if (PhotonNetwork.isMasterClient) {
                npcTurnDelegate checkPlayerDeck = CheckPLayerDeck;
                StartCoroutine(Wait(checkPlayerDeck, 0.5f));
            }
        } else {
            npcTurnDelegate checkPlayerDeck = CheckPLayerDeck;
            StartCoroutine(Wait(checkPlayerDeck, 0.5f));
        }
    }

    private void PlayCardToSideboard() {
        if (!gameControl.playerWon) {
            bool cardPlayed = false;

            //first we will loop through the players hand and the sideboard panels seeing if the last card played on the pile matches the card in hand, if they do we will play that card on that panel
            for (int i = 0; i < currentPlayerPanel.handSlots.Length; i++) {
                //here we loop through the players side bar panel looking for matching cards, if we find a matching card we will place it there
                for (int x = 0; x < currentPlayerPanel.sideBarSlots.Length; x++) {
                    if (currentPlayerPanel.sideBarSlots[x].transform.childCount > 0) {
                        if (currentPlayerPanel.handSlots[i].transform.childCount != 0) {
                            if (currentPlayerPanel.handSlots[i].transform.GetChild(0).GetComponent<Card>().cardNumber == currentPlayerPanel.sideBarSlots[x].transform.GetChild(currentPlayerPanel.sideBarSlots[x].transform.childCount - 1).GetComponent<Card>().cardNumber) {
                                if (!cardPlayed) {
                                    cardPlayed = true;
                                    if (PhotonNetwork.isMasterClient) {
                                        photonView.RPC(npcNetworkToSideboard, PhotonTargets.All, x, currentPlayerPanel.handSlots[i].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //if we didnt find a matching card to play on then we will look for an empty slot and play a random card
            if (!cardPlayed) {
                for (int t = 0; t < currentPlayerPanel.sideBarSlots.Length; t++) {
                    if (currentPlayerPanel.sideBarSlots[t].transform.childCount == 0) {
                        if (!cardPlayed) {
                            cardPlayed = true;
                            int randomHandSlot = RandomInt(0, currentPlayerPanel.handSlots.Length);
                            while (currentPlayerPanel.handSlots[randomHandSlot].transform.childCount == 0) {
                                randomHandSlot = RandomInt(0, currentPlayerPanel.handSlots.Length);
                            }
                            if (PhotonNetwork.isMasterClient) {
                                photonView.RPC(npcNetworkToSideboard, PhotonTargets.All, t, currentPlayerPanel.handSlots[randomHandSlot].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                            }
                            break;
                        }
                    }
                }
            }
            //if we didnt find an empty slot to play the card to we will jus select a random slot and a random card to play
            if (!cardPlayed) {
                int randomHandSlot = RandomInt(0, currentPlayerPanel.handSlots.Length);
                while (currentPlayerPanel.handSlots[randomHandSlot].transform.childCount == 0) {
                    randomHandSlot = RandomInt(0, currentPlayerPanel.handSlots.Length);
                }
                int randomPanelindex = Random.Range(0, currentPlayerPanel.sideBarSlots.Length);
                if (PhotonNetwork.isMasterClient) {
                    photonView.RPC(npcNetworkToSideboard, PhotonTargets.All, randomPanelindex, currentPlayerPanel.handSlots[randomHandSlot].transform.GetChild(0).GetComponent<PhotonView>().viewID);
                }
            }
        }
    }

    private int RandomInt(int min, int max) {
        int randomIndex = Random.Range(min, max);
        return randomIndex;
    }

    [PunRPC]
    private IEnumerator PlayedCardToSideboard(int panelIndex, int cardViewID) {
        currentPlayerPanel = gameControl.playerPanels[gameControl.turnIndex].GetComponent<PanelControl>();
        Transform newParent = currentPlayerPanel.sideBarSlots[panelIndex].transform;
        int numberOfChildren = newParent.childCount;
        Vector3 endPos = new Vector3(0, 0, 0);
        if (numberOfChildren > 0) {
            endPos = new Vector3(0, numberOfChildren * -30, 0);
        }
        if (numberOfChildren > 3) {
            endPos = new Vector3(0, 3 * -30, 0);
        }

        GameObject card = PhotonView.Find(cardViewID).gameObject;
        card.GetComponent<CardLerp>().StartLerping(newParent, endPos);//start lerping the card to the sidebar
        card.GetComponent<CardScaleLerp>().StartScale(card.GetComponent<Card>().cardNumber);//start flipping the card over to reveal it

        StartCoroutine(HideSideboardCards(newParent, currentPlayerPanel));

        yield return new WaitForSeconds(0.5f);
        gameControl.ChangeTurn();
    }

    private IEnumerator HideSideboardCards(Transform panel, PanelControl panelControl) {
        yield return new WaitForSeconds(1f);
        panelControl.HideLowerSideBarCards(panel);
    }

    IEnumerator Wait(npcTurnDelegate func, float seconds) {
        yield return new WaitForSeconds(seconds);
        func();
    }

}
