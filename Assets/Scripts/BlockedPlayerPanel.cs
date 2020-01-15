using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockedPlayerPanel : MonoBehaviour
{
    [SerializeField] Text emailText;
    [SerializeField] Button unBlockButton;

    public void SetUpPanel(User blockedUser) {
        emailText.text = blockedUser.email;
        unBlockButton.onClick.AddListener(() => {
            Friends.UnblockPlayer(blockedUser.userID);
            BlockedPanel.blockedPlayers.Remove(gameObject);
            Destroy(gameObject);
        });
    }
}
