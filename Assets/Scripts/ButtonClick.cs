using UnityEngine.UI;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    private void OnEnable() {
        Sounds sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        GetComponent<Button>().onClick.AddListener(sounds.PlayButtonClick);
    }
}
