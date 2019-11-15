using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
using Firebase.Messaging;
//using System;

public class FirebaseNotificationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
        Debug.Log("Recieved a new message from: " + e.Message.From);
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e) {
        Debug.Log("Recieved Registration Token: " + e.Token);
    }

}
