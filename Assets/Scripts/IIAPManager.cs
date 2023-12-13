using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAP
{
    public interface IIAPManager
    {
        public bool IsIAPSupported();
        public void SetupIAPProducts(IAPElement[] products);
        public bool TryGetIAPProducts(List<Product> products);
        public bool TryGetIAPProductByID(string productID, out Product currentProduct);
        public IAPElement GetIAPInformationByID(string productID);
        public bool TryGetPurchasedProducts(List<Product> purchasedProducts);
        public void PurchaseProduct(string productID);
        public void RestorePurchases();
    }

    [Serializable]
    public class IAPElement
    {
        [field:SerializeField] public string ProductID { get; private set; }
        [field:SerializeField] public long ProductPrice { get; set; }
        [field:SerializeField] public string ProductTitle { get; set; }
        [field:SerializeField] public string ProductDescription { get; set; }
        [field:SerializeField] public string AppleAppStoreID { get; set; }
        [field:SerializeField] public string GooglePlayID { get; set; }
        [field:SerializeField] public string AppleSKU { get; set; }
    }

    public class Product
    {
        public string ProductID { get; set; }
        public bool IsPurchase { get; set; }
    }
}