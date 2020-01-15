using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatePanel : MonoBehaviour
{
    public void Rate() {
        string storeLink = "";
        if (DeviceType.IsDeviceAndroid()) {
            storeLink = "https://play.google.com/store/apps/details?id=com.FogBankGames.SkipBo";
        }
        if (DeviceType.IsDeviceIos()) {
            Debug.Log("Add the store link here for apple");
        }
        Application.OpenURL(storeLink);
        PlayerPrefsHandler.SetGameRated(1);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
    }

    public void DontRate() {
        PlayerPrefsHandler.SetGameRated(1);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
    }

    public void RateLater() {
        PlayerPrefsHandler.ResetGamesPlayed();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
    }
}
