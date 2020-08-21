using UnityEngine;

public class ActivatePanel : MonoBehaviour
{
    public GameObject activePanel;

    public void SwitchPanel(GameObject panel) {
        if (activePanel != null) {
            activePanel.SetActive(false);
        }
        panel.SetActive(true);
        activePanel = panel;
    }

}
