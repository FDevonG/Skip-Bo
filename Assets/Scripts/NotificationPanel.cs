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
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        Destroy(gameObject);
    }

    public void SetText(string message) {
        notificationtext.text = message;
    }

}
