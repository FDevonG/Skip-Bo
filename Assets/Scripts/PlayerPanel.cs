using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] Image body;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;

    private void OnEnable() {
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + LocalUser.locUser.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + LocalUser.locUser.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + LocalUser.locUser.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + LocalUser.locUser.kit) as Sprite;
    }
}
