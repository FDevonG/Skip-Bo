using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) {
        if (GetComponent<Toggle>()) {
            Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
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
