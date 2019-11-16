using UnityEngine.UI;
using UnityEngine;

public class PlayerStandingPanel : MonoBehaviour
{
    [SerializeField]
    Text standingText;
    [SerializeField]
    Text nameText;
    [SerializeField]
    Image hair;
    [SerializeField]
    Image face;
    [SerializeField]
    Image kit;
    [SerializeField]
    Image body;

    public void SetUpPanel(PhotonPlayer player, int standing) {
        int standText = standing + 1;
        standingText.text = standText + ".";
        nameText.text = (string)player.CustomProperties["name"];
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + (string)player.CustomProperties["body"]) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + (string)player.CustomProperties["face"]) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + (string)player.CustomProperties["hair"]) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + (string)player.CustomProperties["kit"]) as Sprite;
    }

}
