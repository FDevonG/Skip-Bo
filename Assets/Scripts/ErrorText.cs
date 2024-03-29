﻿using UnityEngine;
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
        if(errorText == null)
            FindErrorText(gameObject);
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        Announcer.Instance.AnnouncerAnError();
    }

    public void ClearError() {
        if (errorText != null)
            errorText.gameObject.SetActive(false);
    }
}
