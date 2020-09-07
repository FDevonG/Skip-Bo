using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;

public class InAppPurchasing : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //Products
    private static string removeAds = "remove_ads";

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
                GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError("Item is not available for purchase");
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError("Not connected to the store");
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (string.Equals(e.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
        {
            GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsRemoved();
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
        GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<ErrorText>().SetError(p.ToString());
    }


}
