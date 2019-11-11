using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        SaveSystem.DeletePlayer();
        if (DeviceType.IsDeviceAndroid()) {
            if (GooglePlayServices.IsGooglePlayLoggedIn()) {
                GooglePlayServices.SignOut();
            }
        }
        if (DeviceType.IsDeviceIos()) {
            Debug.Log("Write the code to sign out of the apple service here");
        }
    }
}
