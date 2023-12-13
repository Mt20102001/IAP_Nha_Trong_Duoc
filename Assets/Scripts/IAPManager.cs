using IAP;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IAPManager : MonoBehaviour, IIAPManager
{
    [SerializeField] private IAPLibrary iapLibrary;
    private Dictionary<string, Product> products = new Dictionary<string, Product>();

    private void Start()
    {
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        if (IsIAPSupported())
        {
            if (iapLibrary != null && iapLibrary.iAPElements.Length > 0)
            {
                if (!Load())
                {
                    SetupIAPProducts(iapLibrary.iAPElements);
                }
            }
            else
            {
                GameEvent.ShowLogError("Not found any product on cart");
            }
        }
        else
        {
            GameEvent.ShowLogError("IAP is not supported on this platform.");
        }
    }

    public bool TryGetIAPProducts(List<Product> products)
    {
        if (this.products != null)
        {
            products.Clear();
            products.AddRange(this.products.Values);
            return true;
        }

        return false;
    }

    public bool TryGetIAPProductByID(string productID, out Product currentProduct)
    {
        if (this.products != null)
        {
            bool isCompleted = products.TryGetValue(productID, out var product);
            currentProduct = product;
            return isCompleted;
        }

        currentProduct = null;
        return false;
    }

    public IAPElement GetIAPInformationByID(string productID)
    {
        foreach (var iAPElement in iapLibrary.iAPElements)
        {
            if (iAPElement != null && iAPElement.ProductID.Equals(productID))
            {
                return iAPElement;
            }
        }

        return null;
    }

    public bool TryGetPurchasedProducts(List<Product> products)
    {
        if (this.products != null)
        {
            products.Clear();
            products.AddRange(this.products.Values.Where(p => p.IsPurchase == true));
            return true;
        }

        return false;
    }

    public bool IsIAPSupported()
    {
        //return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        return true;
    }

    public void PurchaseProduct(string productID)
    {
        if (this.products != null)
        {
            if (TryGetIAPProductByID(productID, out var product))
            {
                if (product != null && !product.IsPurchase)
                {
                    product.IsPurchase = true;
                    GameEvent.ShowLog($"Purchase product {product.ProductID} completed.");
                    GameEvent.UpdateIAPByID(productID);
                    Save();
                }
                else
                {
                    GameEvent.ShowLogError("Product not found or not available for purchase.");
                }
            }
        }
        else
        {
            GameEvent.ShowLogError("Store controller is not initialized.");
        }
    }

    public void RestorePurchases()
    {
        //if (extensionProvider != null)
        //{
        //    extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
        //        // Xử lý kết quả phục hồi giao dịch
        //        if (result)
        //        {
        //            Debug.LogError("Restore failed.");
        //            GameEvent.ShowLogError("Restore failed.");
        //        }
        //        else
        //        {
        //            Debug.Log("Restore completed.");
        //            GameEvent.ShowLog("Restore completed.");
        //        }
        //    });
        //}
        //else
        //{
        //    Debug.LogError("Extension provider is not initialized.");
        //    GameEvent.ShowLogError("Extension provider is not initialized.");
        //}
    }

    public void SetupIAPProducts(IAPElement[] products)
    {

        for (int i = 0; i < products.Length; i++)
        {
            var product = products[i];
            if (!this.products.ContainsKey(product.ProductID))
            {
                this.products.Add(product.ProductID, new Product
                {
                    ProductID = product.ProductID,
                    IsPurchase = false,
                });
            }
        }
        GameEvent.ShowLog("Setup IAP Products completed.");
        GameEvent.AddIAPs();
    }

    //public void OnInitializeFailed(InitializationFailureReason error)
    //{
    //    Debug.LogError("Initialization failed: " + error);
    //    GameEvent.ShowLogError("Initialization failed: " + error);
    //}

    //public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    //{
    //    Debug.Log("Shopping was successful.");
    //    GameEvent.ShowLog("Shopping was successful.");

    //    return PurchaseProcessingResult.Complete;
    //}

    //public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    //{
    //    Debug.LogError("Purchase failed. Reason: " + failureReason);
    //    GameEvent.ShowLogError("Purchase failed. Reason: " + failureReason);
    //}

    private string Encode(Dictionary<string, Product> products)
    {
        string data = string.Empty;
        //foreach (var product in products)
        //{

        //}

        return string.Empty;
    }

    private Dictionary<string, Product> Decode(string dataString)
    {
        Dictionary<string, Product> datasLoaded = new Dictionary<string, Product>();

        return datasLoaded;
    }

    private const string key = "keyLoadDataIAPProducts";
    private void Save()
    {
        PlayerPrefs.SetString(key, Encode(this.products));
    }

    private bool Load()
    {
        string dataString = PlayerPrefs.GetString(key);
        if (!string.IsNullOrEmpty(dataString))
        {
            var dataLoaded = Decode(dataString);
            products = dataLoaded;
            return true;
        }

        return false;
    }
}
