using UnityEngine;
using UnityEngine.UI;

public class StoreItemPanel : MonoBehaviour
{
    public StoreItem storedItem;
    [SerializeField] Text priceText;

    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Image body;

    public void SetUpPanel(StoreItem sentStoreItem)
    {
        storedItem = sentStoreItem;
        priceText.text = sentStoreItem.price.ToString();
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + LocalUser.locUser.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + LocalUser.locUser.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + LocalUser.locUser.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + LocalUser.locUser.kit) as Sprite;

        switch (sentStoreItem.type)
        {
            case "hair":
                hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + sentStoreItem.name) as Sprite;
                break;

            case "kit":
                kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + sentStoreItem.name) as Sprite;
                break;

            case "face":
                face.sprite = Resources.Load<Sprite>("Faces/Faces/" + sentStoreItem.name) as Sprite;
                break;
        }
        body.color = Color.white;
        face.color = Color.white;
        hair.color = Color.white;
        kit.color = Color.white;
    }

}
