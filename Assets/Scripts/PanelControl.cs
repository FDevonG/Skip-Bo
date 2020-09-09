using UnityEngine;
using UnityEngine.UI;

public class PanelControl : MonoBehaviour
{
    public PhotonPlayer photonPlayer;

    public Text nameText;
    public Text cardsLeftText;
    public Text playerNumberText;

    public GameObject avatarPanel;
    public Image cbody;//used to display the body
    public Image cface;//used to display the face
    public Image chair;//used to display the hair
    public Image ckit;//used to display the clothes

    public GameObject deck;

    public GameObject[] handSlots;
    public GameObject[] sideBarSlots;

    //this function hides lower cards in the stack so we dont run out of screen real estate
    public void HideLowerSideBarCards(Transform panel) {
        int numberOfChildren = panel.childCount;
        if (numberOfChildren > 3) {
            for (int i = 0; i < panel.childCount; i++) {
                if (i > panel.childCount - 3)
                {
                    int ammount = 0;
                    if (panel.parent.parent.parent.gameObject == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().localPlayerPanel)
                    {
                        ammount = 60;
                    }
                    else
                    {
                        ammount = 30;
                    }
                    panel.GetChild(i).GetComponent<CardLerp>().StartLerping(panel, new Vector3(0, panel.GetChild(i).localPosition.y + ammount, 0));
                }
                else
                {
                    panel.GetChild(i).GetComponent<CardLerp>().StartLerping(panel, new Vector3(0, 0, 0));
                }
            }
        }
    }

    //this shows lower cards on the sidebar after top ones have been removed
    public void ShowLowerSideBarCards(Transform panel) {
        int numberOfChildren = panel.childCount;
        if (numberOfChildren > 2) {
            for (int i = 0; i < panel.childCount; i++) {
                if (i > panel.childCount - 3) {
                    int ammount = 0;
                    if (panel.parent.parent.parent.gameObject == GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().localPlayerPanel) {
                        ammount = 60;
                    } else {
                        ammount = 30;
                    }
                    panel.GetChild(i).GetComponent<CardLerp>().StartLerping(panel, new Vector3(0, panel.GetChild(i).localPosition.y - ammount, 0));
                } else {
                    panel.GetChild(i).GetComponent<CardLerp>().StartLerping(panel, new Vector3(0, 0, 0));
                }
            }
        }
    }
}
