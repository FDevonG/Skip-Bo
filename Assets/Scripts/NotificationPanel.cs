using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NotificationPanel : MonoBehaviour
{
    int currCountdownValue;
    int storedTime = 6;
    [SerializeField] Text notificationtext;

    private void Start() {
        StartCoroutine(StartCountdown());
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>().PlayerNotification();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StartCoroutine(StartCountdown());
    }

    public IEnumerator StartCountdown(int countdownValue = 5) {
        if (storedTime == 6) {
            currCountdownValue = countdownValue;
        } else {
            currCountdownValue = storedTime;
        }
        
        while (currCountdownValue > 0) {
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
            storedTime = currCountdownValue;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }

    public void SetText(string message) {
        notificationtext.text = message;
    }

}
