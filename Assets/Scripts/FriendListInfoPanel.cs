using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListInfoPanel : MonoBehaviour {

    User friend;
    [SerializeField] Text nameText;
    [SerializeField] GameObject statusPanel;
    [SerializeField] Button friendSettingsButton;

    public void SetUpFriendPanel(User user, bool status) {
        friend = user;
        nameText.text = user.userName;
        if (status) {
            statusPanel.GetComponent<Image>().color = Color.green;
        } else {
            statusPanel.GetComponent<Image>().color = Color.red;
        }
        friendSettingsButton.onClick.AddListener(() => GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().friendsSettingsPanel.GetComponent<FriendSettingsPanel>().SetUpFriendPanel(friend));
        friendSettingsButton.onClick.AddListener(() => GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().friendsSettingsPanel));
    }
}
