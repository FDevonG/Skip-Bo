using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour
{
    [SerializeField] GameObject panelParent;
    List<GameObject> storeItemPanels = new List<GameObject>();
    [SerializeField] Dropdown dropdown;

    [SerializeField] Text playerGemsText;

    //[SerializeField] Transform 

    private void OnEnable()
    {
        SetGemText();
        BuildStoreItems();
    }

    private void OnDisable()
    {
        dropdown.value = 0;
        DestroyPanelItems();
    }

    public void BuildStoreItems()
    {
        DestroyPanelItems();
        switch (dropdown.value)
        {
            //All
            case 0:
                BuildGemOptions();
                BuildHairOptions();
                BuildFaceOptions();
                BuildClothesOptions();
                break;

            //Gems
            case 1:
                BuildGemOptions();
                break;

            //Hair
            case 2:
                BuildHairOptions();
                break;

            //Faces
            case 3:
                BuildFaceOptions();
                break;

            //Clothes
            case 4:
                BuildClothesOptions();
                break;
        }
    }

    void DestroyPanelItems()
    {
        foreach (GameObject go in storeItemPanels)
        {
            Destroy(go);
        }
        storeItemPanels.Clear();
    }

    void SetGemText()
    {
        playerGemsText.text = "X " + LocalUser.locUser.gems.ToString();
    }

    void SetHeaderText(string message)
    {
        Text headerText = Instantiate(Resources.Load<Text>("StoreHeaderText"), panelParent.transform);
        headerText.text = message;
        storeItemPanels.Add(headerText.gameObject);
    }

    void BuildGemOptions()
    {
        SetHeaderText("Gems");

        if (FirebaseAuthentication.IsPlayerAnonymous())
        {
            Text messageText = Instantiate(Resources.Load<Text>("StoreGuestText"), panelParent.transform);
            storeItemPanels.Add(messageText.gameObject);
        }
        else
        {
            GameObject gridLayout = Instantiate(Resources.Load<GameObject>("StoreGridLayoutPanel"), panelParent.transform);
            storeItemPanels.Add(gridLayout);

            GameObject twoHundred = Instantiate(Resources.Load<GameObject>("StoreGemPanel"), gridLayout.transform);
            twoHundred.GetComponent<StoreGemPanel>().ammountText.text = "X 200";
            twoHundred.GetComponent<StoreGemPanel>().costText.text = "$1 CAD";
            twoHundred.GetComponent<StoreGemPanel>().button.onClick.AddListener(BuyTwoHundredGems);

            GameObject oneThousand = Instantiate(Resources.Load<GameObject>("StoreGemPanel"), gridLayout.transform);
            oneThousand.GetComponent<StoreGemPanel>().ammountText.text = "X 1000";
            oneThousand.GetComponent<StoreGemPanel>().costText.text = "$5 CAD";
            oneThousand.GetComponent<StoreGemPanel>().button.onClick.AddListener(BuyOneThousandGems);
        }
    }

    void BuildHairOptions()
    {
        SetHeaderText("Hair");

        GameObject gridLayout = Instantiate(Resources.Load<GameObject>("StoreGridLayoutPanel"), panelParent.transform);
        storeItemPanels.Add(gridLayout);

        List<StoreItem> items = JsonConvert.DeserializeObject<List<StoreItem>>(Resources.Load<TextAsset>("JSON/HairStoreItems").ToString());

        foreach (StoreItem item in items)
        {
            Debug.Log(item.itemName);
            GameObject storeItemPanel = Instantiate(Resources.Load<GameObject>("StoreItemPanel"), gridLayout.transform);
            storeItemPanel.GetComponent<StoreItemPanel>().SetUpPanel(item, this);
            storeItemPanels.Add(storeItemPanel);
        }
    }

    void BuildFaceOptions()
    {
        SetHeaderText("Faces");

        GameObject gridLayout = Instantiate(Resources.Load<GameObject>("StoreGridLayoutPanel"), panelParent.transform);
        storeItemPanels.Add(gridLayout);

        List<StoreItem> items = JsonConvert.DeserializeObject<List<StoreItem>>(Resources.Load<TextAsset>("JSON/FaceStoreItems").ToString());

        foreach (StoreItem item in items)
        {
            Debug.Log(item.itemName);
            GameObject storeItemPanel = Instantiate(Resources.Load<GameObject>("StoreItemPanel"), gridLayout.transform);
            storeItemPanel.GetComponent<StoreItemPanel>().SetUpPanel(item, this);
            storeItemPanels.Add(storeItemPanel);
        }
    }

    void BuildClothesOptions()
    {
        SetHeaderText("Clothes");

        GameObject gridLayout = Instantiate(Resources.Load<GameObject>("StoreGridLayoutPanel"), panelParent.transform);
        storeItemPanels.Add(gridLayout);

        List<StoreItem> items = JsonConvert.DeserializeObject<List<StoreItem>>(Resources.Load<TextAsset>("JSON/ClothStoreItems").ToString());

        foreach (StoreItem item in items)
        {
            GameObject storeItemPanel = Instantiate(Resources.Load<GameObject>("StoreItemPanel"), gridLayout.transform);
            storeItemPanel.GetComponent<StoreItemPanel>().SetUpPanel(item, this);
            storeItemPanels.Add(storeItemPanel);
        }
    }

    public void BuyItem(StoreItem itemToPurchase, StoreItemPanel sp)
    {
        if (LocalUser.locUser.gems >= itemToPurchase.price)
        {
            switch (itemToPurchase.type)
            {
                case "hair":
                    LocalUser.locUser.unlockedHair.Add(itemToPurchase.itemName);
                    Database.UpdateUser("unlockedHair", LocalUser.locUser.unlockedHair);
                    break;

                case "face":
                    LocalUser.locUser.unlockedFace.Add(itemToPurchase.itemName);
                    Database.UpdateUser("unlockedFace", LocalUser.locUser.unlockedFace);
                    break;

                case "cloth":
                    LocalUser.locUser.unlockedClothes.Add(itemToPurchase.itemName);
                    Database.UpdateUser("unlockedClothes", LocalUser.locUser.unlockedClothes);
                    break;
            }
            GemControl.Instance.SubtractGems(itemToPurchase.price);
            SetGemText();
            StartCoroutine(Notifications.Instance.SpawnNotification("New item availible in charcter editor"));
            sp.TurnOffButton();
        }
        else
        {
            GetComponent<ErrorText>().SetError("Not enough gems");
        }
    }

    public void BuyTwoHundredGems()
    {
        InAppPurchasing.instance.BuyTwoHundredGems();
    }

    public void TwoHundredGemsPurchased()
    {
        GemControl.Instance.AddGems(200);
        SetGemText();
    }

    public void BuyOneThousandGems()
    {
        InAppPurchasing.instance.BuyOneThousandGems();

    }

    public void OneThousandGemsPurchased()
    {
        GemControl.Instance.AddGems(1000);
        SetGemText();
    }
}
