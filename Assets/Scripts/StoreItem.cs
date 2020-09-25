using UnityEngine;

[System.Serializable]
public class StoreItem : MonoBehaviour
{

    public string itemName { get; set; }
    public string type { get; set; }
    public int price { get; set; }


    [Newtonsoft.Json.JsonConstructor]
    public StoreItem(string ItemName, string Type, int Price)
    {
        itemName = ItemName;
        type = Type;
        price = Price;
    }
}
