using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) {
        Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        if (GetComponent<Toggle>() || GetComponent<Dropdown>()) {
            sounds.PlayButtonClick();
        }
    }

    private void OnEnable() {
        Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        if (GetComponent<Button>()) {
            GetComponent<Button>().onClick.AddListener(sounds.PlayButtonClick);
        }
    }
}
