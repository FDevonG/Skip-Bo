using UnityEngine;

public class CardScaleLerp : MonoBehaviour
{
    private bool firstScaleing = false;//a boolean to turn on and off the lerp
    private bool secondScale = false;
    private Vector3 startScale = new Vector3();//this is the start position of the transform
    private Vector3 endScale = new Vector3();//this is where we want the transform to end at

    private float timeTakenDuringLerp = 1.0f;
    private float timeStartedLerping;

    private int newCardValue;

    // Start is called before the first frame update
    public void StartScale(int newCardVal) {
        firstScaleing = true;//turn on the boolean
        timeStartedLerping = Time.time;
        startScale = transform.localScale;//set the start position to the current position when we call the start lerp method

        newCardValue = newCardVal;

        if (transform.parent.GetComponent<RectTransform>().sizeDelta == new Vector2(144, 200)) {
            endScale = new Vector3(0, 2, 1);
        } else {
            endScale = new Vector3(0, 1, 1);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (firstScaleing) {
            float timeSinceStarted = (Time.time - timeStartedLerping) * 2;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            transform.localScale = Vector3.Lerp(startScale, endScale, percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                firstScaleing = false;

                if (transform.localScale == new Vector3(0, 2, 1)) {
                    endScale = new Vector3(2, 2, 1);
                } else {
                    endScale = new Vector3(1, 1, 1);
                }

                timeStartedLerping = Time.time;
                startScale = transform.localScale;//set the start position to the current position when we call the start lerp method
                GameControl gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();

                GetComponent<Card>().SetUpCard(newCardValue);

                if (newCardValue == gameControl.wildNumber) {
                    for (int i = 0; i < gameControl.playedCardPanels.Count; i++) {
                        if (transform.parent == gameControl.playedCardPanels[i].transform) {
                            GetComponent<Card>().SetUpWildCard(gameControl.playedCardPanels[i].GetComponent<PlayedCardStack>().currentCardValue);
                        }
                    }
                }

                secondScale = true;
                    
            } 
        }
        if (secondScale) {
            float timeSinceStarted = (Time.time - timeStartedLerping) * 2;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            transform.localScale = Vector3.Lerp(startScale, endScale, percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                secondScale = false;
                GameControl gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
                if (gameControl.playerPanels[gameControl.turnIndex] == gameControl.localPlayerPanel) {
                    this.gameObject.AddComponent<CardDragHandler>();
                }
            }
        }
    }
}
