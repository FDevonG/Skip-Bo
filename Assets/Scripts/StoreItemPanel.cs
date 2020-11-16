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

    [SerializeField] Button button;

    StorePanel storePanel;

    public void SetUpPanel(StoreItem sentStoreItem, StorePanel sp)
    {
        storePanel = sp;
        storedItem = sentStoreItem;
        priceText.text = sentStoreItem.price.ToString();
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + LocalUser.locUser.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + LocalUser.locUser.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + LocalUser.locUser.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + LocalUser.locUser.kit) as Sprite;

        switch (sentStoreItem.type)
        {
            case "hair":
                hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + sentStoreItem.itemName) as Sprite;
                break;

            case "kit":
                kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + sentStoreItem.itemName) as Sprite;
                break;

            case "face":
                face.sprite = Resources.Load<Sprite>("Faces/Faces/" + sentStoreItem.itemName) as Sprite;
                break;
        }
        body.color = Color.white;
        face.color = Color.white;
        hair.color = Color.white;
        kit.color = Color.white;

        bool itemOwned = false;
        switch (sentStoreItem.type)
        {
            case "hair":
                foreach (string st in LocalUser.locUser.unlockedHair)
                {
                    if(string.Equals(st, sentStoreItem.itemName))
                    {
                        itemOwned = true;
                        break;
                    }
                }
                break;

            case "face":
                foreach (string st in LocalUser.locUser.unlockedFace)
                {
                    if (string.Equals(st, sentStoreItem.itemName))
                    {
                        itemOwned = true;
                        break;
                    }
                }
                break;

            case "cloth":
                foreach (string st in LocalUser.locUser.unlockedClothes)
                {
                    if (string.Equals(st, sentStoreItem.itemName))
                    {
                        itemOwned = true;
                        break;
                    }
                }
                break;
        }

        if (itemOwned)
        {
            TurnOffButton();
        }
        else
        {
            button.onClick.AddListener(() => StartCoroutine(storePanel.BuyItem(storedItem, this)));
        }

    }

    public void TurnOffButton()
    {
        button.GetComponentInChildren<Text>().text = "Owned";
        button.interactable = false;
    }

}
