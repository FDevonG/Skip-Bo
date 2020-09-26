using System.Collections;
using UnityEngine;

public class Notifications : MonoBehaviour
{

    public static Notifications Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator SpawnNotification(string message)
    {
        while (GameObject.FindGameObjectWithTag("NotificationPanel") != null && CardDragHandler.itemBeingDragged != null)
        {
            yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("NotificationPanel") == null && CardDragHandler.itemBeingDragged == null);
        }
        GameObject notificationPanel = Instantiate(Resources.Load<GameObject>("NotificationPanel"));
        notificationPanel.transform.localScale = new Vector3(1, 1, 1);
        notificationPanel.GetComponent<NotificationPanel>().SetText(message);
    }
}
