using UnityEngine;
using UnityEngine.UI;

public class ErrorText : MonoBehaviour
{
    [SerializeField] Text errorText;

    private void OnDisable() {
        ClearError();
    }

    public void SetError(string message) {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().AnnouncerAnError();
    }

    public void ClearError() {
        errorText.gameObject.SetActive(false);
    }
}
