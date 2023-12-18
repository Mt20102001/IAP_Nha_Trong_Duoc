using IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Debug Console")]
    [SerializeField] private TextMeshProUGUI logPrefab, logErrorPrefab;
    [SerializeField] private Transform logsTransform;
    [SerializeField] private RectTransform logsRectTransform;

    [Header("MainUI")]
    [SerializeField] private UIIAPElement uiIAPElementPrefab;
    [SerializeField] private Transform uiIAPButtonsTransform;

    [SerializeField] private GameObject controllerObj;
    private IIAPManager iAPManager;

    [SerializeField] private GameObject popupPurchase, popupRestorePuchare;
    [SerializeField] private Button acceptPurchaseBtn, cancelPurchaseBtn, restorePuchaseBtn, closeRestorePopupBtn;
    [SerializeField] private TextMeshProUGUI notificationPurchaseTxt, notificationRestorePurchaseTxt;

    private Dictionary<string, UIIAPElement> currentUIIAPElements = new Dictionary<string, UIIAPElement>();
    private List<Product> currentProduct = new List<Product>();

    IEnumerator Start()
    {
        //=/Debug/============================
        GameEvent.ClearEvent();
        GameEvent.OnLog += ShowLog;
        GameEvent.OnLogError += ShowLogError;
        //====================================

        popupRestorePuchare.SetActive(false);
        iAPManager = controllerObj.GetComponent<IIAPManager>();
        cancelPurchaseBtn.onClick?.RemoveAllListeners();
        cancelPurchaseBtn.onClick?.AddListener(() => SetActivePopupPurchase(false));

        closeRestorePopupBtn.onClick?.AddListener(() => popupRestorePuchare.SetActive(false));

        restorePuchaseBtn.onClick?.RemoveAllListeners();
        restorePuchaseBtn.onClick?.AddListener(() =>
        {
            iAPManager.RestorePurchases((success) =>
            {
                if (success)
                {
                    popupRestorePuchare.SetActive(true);
                    notificationRestorePurchaseTxt.text = "Restore Successful";
                    RefrestUIs();
                }
                else
                {
                    popupRestorePuchare.SetActive(true);
                    notificationRestorePurchaseTxt.text = "Restore Failed";
                }
            });
        });

        yield return new WaitUntil(() => iAPManager.IsSetupDone == true);
        CreateIAPProductUIs();
    }

    //=/Debug/================================================================================================

    private void ShowLog(string log)
    {
        var currentLog = Instantiate<TextMeshProUGUI>(logPrefab, logsTransform);
        currentLog.text = log;
        currentLog.rectTransform.SetAsFirstSibling();
    }

    private void ShowLogError(string logError)
    {
        var currentLogError = Instantiate<TextMeshProUGUI>(logErrorPrefab, logsTransform);
        currentLogError.text = logError;
        currentLogError.rectTransform.SetAsFirstSibling();
    }

    //====================================================================================================

    private void CreateIAPProductUIs()
    {
        if (iAPManager.TryGetIAPProducts(currentProduct))
        {
            for (int i = 0; i < currentProduct.Count; i++)
            {
                var product = currentProduct[i];
                var newIAPButtonUI = GameObject.Instantiate(uiIAPElementPrefab, uiIAPButtonsTransform);
                var data = iAPManager.GetIAPInformationByID(product.ProductID);
               
                newIAPButtonUI.titleTxt.text = data.ProductTitle;
                newIAPButtonUI.descriptionTxt.text = data.ProductDescription;
                

                if (!product.IsPurchase)
                {
                    newIAPButtonUI.priceTxt.text = $"{data.ProductPrice}";
                    newIAPButtonUI.buttonIAP.onClick?.AddListener(() => SetupNotificationPurchase(data));
                }
                else
                {
                    newIAPButtonUI.priceTxt.text = "Saled";
                    newIAPButtonUI.buttonIAP.enabled = false;
                }

                currentUIIAPElements.TryAdd(product.ProductID, newIAPButtonUI);
            }
        }
    }

    private void RefrestUIs()
    {
        if (iAPManager.TryGetIAPProducts(currentProduct))
        {
            for (int i = 0; i < currentProduct.Count; i++)
            {
                var product = currentProduct[i];
                UpdateUIForIAPById(product.ProductID);
            }
        }
    }

    private void UpdateUIForIAPById(string productID)
    {
        if (currentUIIAPElements.TryGetValue(productID, out var uIOfIAP))
        {
            if (iAPManager.TryGetIAPProductByID(productID, out var product))
            {
                if (product.IsPurchase)
                {
                    uIOfIAP.priceTxt.text = "Saled";
                    uIOfIAP.buttonIAP.enabled = false;
                }
                else
                {
                    var infor = iAPManager.GetIAPInformationByID(productID);
                    uIOfIAP.priceTxt.text = $"{infor.ProductPrice}";
                    uIOfIAP.buttonIAP.enabled = true;
                }
            }
        }
    }

    private void SetActivePopupPurchase(bool isActive)
    {
        if (popupPurchase.activeSelf != isActive)
        {
            popupPurchase.SetActive(isActive);
        }
    }

    private void SetupNotificationPurchase(IAPElement iAP)
    {
        notificationPurchaseTxt.text = $"Confirming the purchase of item [{iAP.ProductTitle}].";
        acceptPurchaseBtn.onClick?.RemoveAllListeners();
        acceptPurchaseBtn.onClick?.AddListener(() =>
        {
            iAPManager.PurchaseProduct(iAP.ProductID, (success) =>
            {
                if (success)
                {
                    SetActivePopupPurchase(false);
                    UpdateUIForIAPById(iAP.ProductID);
                }
                else
                {
                    // notice
                }
            });
        });
        SetActivePopupPurchase(true);
    }



    private void OnDestroy()
    {
        GameEvent.ClearEvent();
    }
}

public class GameEvent
{
    public static System.Action<string> OnLogError;
    public static System.Action<string> OnLog;

    public static void ShowLog(string log)
    {
        OnLog?.Invoke($"[{DateTime.Now}]: {log}");
    }

    public static void ShowLogError(string log)
    {
        OnLogError?.Invoke($"[{DateTime.Now}]: {log}");
    }

    public static void ClearEvent()
    {
        OnLogError = null;
        OnLog = null;
    }
}
