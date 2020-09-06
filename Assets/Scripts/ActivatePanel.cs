using UnityEngine;

public class ActivatePanel : MonoBehaviour
{
    public GameObject activePanel;
    public GameObject previousPanel;

    public void SwitchPanel(GameObject panel) {
        if (activePanel != null) {
            previousPanel = activePanel;
            activePanel.SetActive(false);
        }
        panel.SetActive(true);
        activePanel = panel;
    }

    public void PreviousPanel()
    {
        if (previousPanel != null)
        {
            GameObject panel = activePanel;
            activePanel.SetActive(false);
            activePanel = previousPanel;
            activePanel.SetActive(true);
            previousPanel = panel;
        }
    }
    
}
