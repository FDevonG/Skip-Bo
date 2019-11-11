using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        SaveSystem.DeletePlayer();
        FireBaseScript.SignOut();
    }
}
