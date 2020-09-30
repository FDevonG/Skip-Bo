using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) {
        if (GetComponent<Toggle>() || GetComponent<Dropdown>()) {
            Sounds.Instance.PlayButtonClick();
        }
    }

    private void OnEnable() {
        if (GetComponent<Button>()) {
            GetComponent<Button>().onClick.AddListener(Sounds.Instance.PlayButtonClick);
        }
    }
}
