using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchasing : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //Products
    private static string removeAds = "remove_ads";
    private static string twoHundredGems = "two_hundred_gems";
    private static string oneThousandGems = "one_thousand_gems";

    public static InAppPurchasing instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(removeAds, ProductType.NonConsumable);
        builder.AddProduct(twoHundredGems, ProductType.Consumable);
        builder.AddProduct(oneThousandGems, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void BuyRemoveAds()
    {
        BuyProductID(removeAds);
    }

    public void BuyTwoHundredGems()
    {
        BuyProductID(twoHundredGems);
    }

    public void BuyOneThousandGems()
    {
        BuyProductID(oneThousandGems);
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation

                if (string.Equals(productId, removeAds))
                    GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError("Item is not available for purchase");
                GameObject storePanel = GameObject.FindGameObjectWithTag("GameManagr").GetComponent<Menu>().storePanel;
                if (string.Equals(productId, twoHundredGems) || string.Equals(productId, oneThousandGems))
                    storePanel.GetComponent<ErrorText>().SetError("Item is not availible for purchase");
                
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            if(string.Equals(productId, removeAds))
                GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError("Not connected to the store");
            GameObject storePanel = GameObject.FindGameObjectWithTag("GameManagr").GetComponent<Menu>().storePanel;
            if (string.Equals(productId, twoHundredGems) || string.Equals(productId, oneThousandGems))
                storePanel.GetComponent<ErrorText>().SetError("Not Connected to the store");

            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (string.Equals(e.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
        {
            GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsRemoved();
        }
        if (string.Equals(e.purchasedProduct.definition.id, twoHundredGems, StringComparison.Ordinal))
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().storePanel.GetComponent<StorePanel>().TwoHundredGemsPurchased();
        }
        if (string.Equals(e.purchasedProduct.definition.id, oneThousandGems, StringComparison.Ordinal))
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().storePanel.GetComponent<StorePanel>().OneThousandGemsPurchased();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Initialized");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        Debug.Log(extensions);
        Debug.Log(controller);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Failed To Initialize");
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        if (GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetActive())
            GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError(p.ToString());
        else if(GameObject.FindGameObjectWithTag("GameManagr").GetComponent<Menu>().storePanel.GetActive())
            GameObject.FindGameObjectWithTag("GameManagr").GetComponent<Menu>().storePanel.GetComponent<ErrorText>().SetError(p.ToString());
    }


}
