using UnityEngine;
using UnityEngine.UI;

public class ErrorText : MonoBehaviour
{
    Text errorText;

    private void Start()
    {
        FindErrorText(gameObject);
    }

    private void OnDisable() {
        ClearError();
    }

    void FindErrorText(GameObject panelToSearch)
    {
        foreach (Transform trans in panelToSearch.transform) {
            if (trans.CompareTag("ErrorText"))
            {
                errorText = trans.GetComponent<Text>();
                break;
            }
            else if(trans.childCount > 0)
            {
                FindErrorText(trans.gameObject);
            }

        }
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
