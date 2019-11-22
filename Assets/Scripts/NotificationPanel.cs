using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    float currCountdownValue;
    [SerializeField] Text notificationtext;

    private void Start() {
        StartCoroutine(StartCountdown());
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator StartCountdown(float countdownValue = 5) {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0) {
            //Debug.Log("Countdown: " + currCountdownValue);
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        Destroy(gameObject);
    }

    public void SetText(string message) {
        notificationtext.text = message;
    }

    private void Update() {
        if (transform.parent == null) {
            if (GameObject.Find("Canvas") != null) {
                transform.SetParent(GameObject.Find("Canvas").transform);
            }
        }
    }

}
