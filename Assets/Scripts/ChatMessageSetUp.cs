using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageSetUp : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text messageText;

    public void SetUpMessage(string name, string message)
    {
        nameText.text = name;
        messageText.text = message;
    }
}
