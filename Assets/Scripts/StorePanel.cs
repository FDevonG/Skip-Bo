using System.Collections;
using System.Collections.Generic;
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
        switch (dropdown.value)
        {
            //All
            case 0:

                break;

            //Gems
            case 1:
                BuildGemOptions();
                break;

            //Hair
            case 2:
                break;

            //Faces
            case 3:
                break;

            //Clothes
            case 4:
                break;
        }
        //JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("JSON/Items").ToString());
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

    void BuildGemOptions()
    {
        Text headerText = Instantiate(Resources.Load<Text>("StoreHeaderText"), panelParent.transform);
        headerText.text = "Gems";

        GameObject gridLayout = Instantiate(Resources.Load<GameObject>("StoreGridLayoutPanel"), panelParent.transform);

    }

    public void BuyTwoHundredGems()
    {
        InAppPurchasing.instance.BuyTwoHundredGems();
    }

    public void TwoHundredGemsPurchased()
    {
        LocalUser.locUser.gems += 200;
        Database.UpdateUser("gems", LocalUser.locUser.gems);
        SetGemText();
    }

    public void BuyOneThousandGems()
    {
        InAppPurchasing.instance.BuyOneThousandGems();

    }

    public void OneThousandGemsPurchased()
    {
        LocalUser.locUser.gems += 1000;
        Database.UpdateUser("gems", LocalUser.locUser.gems);
        SetGemText();
    }
}
