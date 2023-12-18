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
