using UnityEngine;
using UnityEngine.UI;

public class LoginPanelSetup : MonoBehaviour
{
    public Button loginButton;
    public Text loginButtonText;

    private void OnEnable() {
        if (DeviceType.IsDeviceAndroid()) {
            loginButtonText.text = "Google Play";
            loginButton.GetComponent<Button>().onClick.AddListener(GameObject.FindGameObjectWithTag("GooglePlayServices").GetComponent<GooglePlayServices>().SignIn); 
        }
        if (DeviceType.IsDeviceIos()) {
            Debug.Log("Write the code to set up the login button on ios here");
        }
    }
}
