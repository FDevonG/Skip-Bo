using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "V" + GameGlobalSettings.Version();
    }
}
