using IAP;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class IAPManager : MonoBehaviour, IIAPManager
{
    [SerializeField] private IAPLibrary iapLibrary;
    private Dictionary<string, Product> products = new Dictionary<string, Product>();

    public bool IsSetupDone { get; private set; }

    private void Start()
    {
        InitializePurchasing();
    }

    private void InitializePurchasing()
    {
        if (IsSetupDone)
            return;

        if (IsIAPSupported())
        {
            if (iapLibrary != null && iapLibrary.iAPElements.Length > 0)
            {
                SetupIAPProducts(iapLibrary.iAPElements);
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

    public int GetIAPProducts(List<Product> products)
    {
        if (this.products != null)
        {
            products.Clear();
            products.AddRange(this.products.Values);
            return products.Count;
        }

        return 0;
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
        foreach (var product in products.Values)
        {
            if (product != null && product.ProductID.Equals(productID))
            {
                return product.IAPElement;
            }
        }

        return null;
    }

    public int GetPurchasedProducts(List<Product> products)
    {
        if (this.products != null)
        {
            products.Clear();
            products.AddRange(this.products.Values.Where(p => p.IsPurchase == true));
            return products.Count;
        }

        return 0;
    }

    public bool IsIAPSupported()
    {
        //return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        return true;
    }

    public void PurchaseProduct(string productID, Action<bool> PurchaseCallback)
    {
        if (this.products != null)
        {
            if (TryGetIAPProductByID(productID, out var product))
            {
                if (product != null && !product.IsPurchase)
                {
                    product.IsPurchase = true;
                    PurchaseCallback?.Invoke(true);
                    OnPurchaseSuccessful(productID);
                }
                else
                {
                    PurchaseCallback?.Invoke(false);
                    OnPurchaseFailed(productID);
                }
            }
        }
        else
        {
            PurchaseCallback?.Invoke(false);
            GameEvent.ShowLogError("Store controller is not initialized.");
        }
    }

    public void RestorePurchases(Action<bool> PurchaseCallback)
    {
        try
        {
            PurchaseCallback?.Invoke(true);
            OnRestorePurchasesSuccessful();
        }
        catch (Exception ex)
        {
            PurchaseCallback?.Invoke(false);
            OnRestorePurchasesFailed();
            GameEvent.ShowLogError($"{ex}");
        }
    }

    public void SetupIAPProducts(IAPElement[] products)
    {
        IsSetupDone = false;
        for (int i = 0; i < products.Length; i++)
        {
            var product = products[i];
            if (!this.products.ContainsKey(product.ProductID))
            {
                this.products.Add(product.ProductID, new Product
                {
                    ProductID = product.ProductID,
                    IAPElement = product,
                    IsPurchase = false,
                });
            }
        }
        IsSetupDone = true;
        GameEvent.ShowLog("Setup IAP Products completed.");
    }

    public void OnPurchaseFailed(string productID)
    {
        GameEvent.ShowLogError($"Purchase failed product [{productID}]");
    }

    public void OnPurchaseSuccessful(string productID)
    {
        GameEvent.ShowLog($"Purchase successful [{productID}]");
    }

    public void OnRestorePurchasesSuccessful()
    {
        GameEvent.ShowLog($"Restore purchase successful");
    }

    public void OnRestorePurchasesFailed()
    {
        GameEvent.ShowLogError($"Restore purchase failed");
    }
}

[Serializable]
public class IAPElementData : IAPElement
{
    [field : SerializeField] public string ProductID { get; set; }

    [field: SerializeField] public long ProductPrice { get; set; }

    [field: SerializeField] public string ProductTitle { get; set; }

    [field: SerializeField] public string ProductDescription { get; set; }
}
