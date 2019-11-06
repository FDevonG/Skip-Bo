using UnityEngine;

public class SideBoardDoubleClick : MonoBehaviour
{

    Sounds sounds;
    GameControl gameControl;

    private void Start() {
        sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
    }

    private void CheckPlayedPanels() {
        int cardNumber = GetComponent<Card>().cardNumber;
        for (int i = 0; i < gameControl.playedCardPanels.Count; i++) {
            if (gameControl.playedCardPanels[i].GetComponent<PlayedCardStack>().currentCardValue + 1 == cardNumber) {
                Transform startParent = transform.parent;
                sounds.PlayCardFlip(gameObject.GetComponent<AudioSource>());//play the sound of the card being played

                transform.SetParent(gameControl.playedCardPanels[i].transform);//set the new transform parent
                transform.localPosition = new Vector3(0, 0, 0);//zero out the dragged card in its new transform

                gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;//make the card so we cant interact with it again

                gameControl.playedCardPanels[i].GetComponent<PlayedCardStack>().AddCardToStack();//add the card to the stack

                PanelControl panelControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().localPlayerPanel.GetComponent<PanelControl>();
                startParent.GetComponent<SideBarDropHandler>().TurnOnCardInteraction();//turn the interaction of the next item on sidebar so we can move that one
                panelControl.ShowLowerSideBarCards(CardDragHandler.startParent);

                int panelIndex = 0;
                for (int x = 0; x < gameControl.playedCardPanels.Count; x++) {
                    if (gameControl.playedCardPanels[x].transform == transform) {
                        panelIndex = x;
                        break;
                    }
                }
                gameControl.playedCardPanels[i].GetComponent<PlayedCardsDropHandler>().DoubleClickedSideboard(gameObject.GetComponent<PhotonView>().viewID, panelIndex);
            }
        }
    }

    public static bool IsDoubleTap() {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }

    private void Update() {
        if (IsDoubleTap()) {
            CheckPlayedPanels();
        }
    }

}
