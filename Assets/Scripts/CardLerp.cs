using UnityEngine;
using UnityEngine.UI;

public class CardLerp : MonoBehaviour { 
    private bool isLerping = false;
    private float timeStartedLerping;
    private Vector3 startPosition = new Vector3();
    private Vector3 endPosition = new Vector3(0, 0, 0);
    private float timeTakenDuringLerp = 1f;
    private Vector3 startScale = new Vector3();
    private Vector3 newScale = new Vector3(1,1,1);
    private Transform startParent;

    public void StartLerping(Transform newParent, Vector3 endPoint) {
        isLerping = true;
        timeStartedLerping = Time.time;
        startParent = transform.parent;
        transform.SetParent(newParent);
       
        //We set the start position to the current position, and the finish to 10 spaces in the 'forward' direction
        startPosition = transform.localPosition;
        startScale = transform.localScale;

        endPosition = endPoint;

        Vector2 gridSize = new Vector2();
        if (transform.parent.GetComponent<GridLayoutGroup>()) {
            gridSize = transform.parent.transform.parent.GetComponent<GridLayoutGroup>().cellSize;
        }
        if (transform.parent.GetComponent<RectTransform>().sizeDelta == new Vector2(144, 200) || gridSize == new Vector2(144, 200)) {
            newScale = new Vector3(2, 2, 2);
        }

    }

    void Update() {
        if (isLerping) {

            //We want percentage = 0.0 when Time.time = _timeStartedLerping
            //and percentage = 1.0 when Time.time = _timeStartedLerping + timeTakenDuringLerp
            //In other words, we want to know what percentage of "timeTakenDuringLerp" the value
            //"Time.time - _timeStartedLerping" is.
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            //Perform the actual lerping.  Notice that the first two parameters will always be the same
            //throughout a single lerp-processs (ie. they won't change until we hit the space-bar again
            //to start another lerp)
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, percentageComplete);
            if (newScale != new Vector3(1, 1, 1)) {
                transform.localScale = Vector3.Lerp(startScale, newScale, percentageComplete);
            }
            
            //When we've completed the lerp, we set _isLerping to false
            if (percentageComplete >= timeTakenDuringLerp) {
                isLerping = false;
                if (transform.parent != startParent) {
                    Sounds.Instance.PlayCardFlip(transform.GetComponent<AudioSource>());
                }
            }
        }
    }
}
