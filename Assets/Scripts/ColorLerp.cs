using UnityEngine;
using UnityEngine.UI;

public class ColorLerp : MonoBehaviour {

    Color playerTurnColor = new Color(0.3179769f, 0.008944447f, 0.6320754f, 1);//thhis is used to signify that it is a players turn
    Color notPlayerTurnColor = new Color(0.3179769f, 0.008944447f, 0.6320754f, 0);

    [SerializeField]
    Image imageComponent;

    private float timeTakenDuringLerp = 1.0f;
    private float timeStartedLerping;

    private bool playerTurnEnding = false;//this is used to signify that a player turn has begun
    private bool playerTurnBegining = false;//this is used to signify that a player turn is begining

    public void StartColorLerp(bool begin, bool end) {
        if (begin && !end) {
            playerTurnBegining = true;//set the begine switch to on
        }
        if (!begin && end) {
            playerTurnEnding = true;
        }
        timeStartedLerping = Time.time;//get the time this all started
    }

    void Update() {

        if (playerTurnBegining && !playerTurnEnding) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            imageComponent.color = Color.Lerp(notPlayerTurnColor, playerTurnColor, percentageComplete);
            if (percentageComplete >= timeTakenDuringLerp) {
                playerTurnBegining = false;
            }
        }

        if (!playerTurnBegining && playerTurnEnding) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            imageComponent.color = Color.Lerp(playerTurnColor, notPlayerTurnColor, percentageComplete);

            if (percentageComplete >= timeTakenDuringLerp) {
                playerTurnEnding = false;
            }
        }

    }
}
