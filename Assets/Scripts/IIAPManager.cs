using System;
using System.Collections.Generic;

namespace IAP
{
    public interface IIAPManager
    {
        public bool IsSetupDone { get; }
        public bool IsIAPSupported();
        public void SetupIAPProducts(IAPElement[] products);
        public int GetIAPProducts(List<Product> products);
        public bool TryGetIAPProductByID(string productID, out Product currentProduct);
        public IAPElement GetIAPInformationByID(string productID);
        public int GetPurchasedProducts(List<Product> purchasedProducts);
        public void PurchaseProduct(string productID, Action<bool> PurchaseCallback);
        public void RestorePurchases(Action<bool> RestorePurchaseCallback);
    }

    public interface IAPElement
    {
        public string ProductID { get; }
        public long ProductPrice { get; }
        public string ProductTitle { get; }
        public string ProductDescription { get; }
    }

    public class Product
    {
        public string ProductID { get; set; }
        public bool IsPurchase { get; set; }
        public IAPElement IAPElement { get; set; }
    }
}