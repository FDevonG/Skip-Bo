using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenPlayerInfoPanel : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) {
        GameObject infoPanel = GameObject.Find("Canvas").transform.Find("PlayerPanel").gameObject;
        if (infoPanel != null) {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>().PlayButtonClick();
            //infoPanel.SetActive(true);
            infoPanel.GetComponent<InGamePlayerInfoTab>().SetUpPanel(GetComponentInParent<PanelControl>().photonPlayer);
        }
    }
}
