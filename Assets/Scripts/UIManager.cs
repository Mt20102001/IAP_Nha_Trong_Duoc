using IAP;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private Dictionary<string, UIIAPElement> currentUIIAPElements = new Dictionary<string, UIIAPElement>();
    private List<Product> currentProduct = new List<Product>();

    //[Obsolete]
    void Awake()
    {
        iAPManager = controllerObj.GetComponent<IIAPManager>();
        GameEvent.ClearEvent();
        GameEvent.OnLog += ShowLog;
        GameEvent.OnLogError += ShowLogError;
        GameEvent.OnAddNewIAP += AddNewIAPs;
        GameEvent.OnUpdateIAPById += UpdateUIForIAPById;
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

    //[Obsolete]
    private void AddNewIAPs()
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
                    newIAPButtonUI.buttonIAP.onClick.AddListener(() => iAPManager.PurchaseProduct(product.ProductID));
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

    private void UpdateUIForIAPById(string productID)
    {
        if (currentUIIAPElements.TryGetValue(productID, out var uIOfIAP))
        {
            uIOfIAP.priceTxt.text = "Saled";
            uIOfIAP.buttonIAP.enabled = false;
        }
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
    public static System.Action OnAddNewIAP;
    public static System.Action<string> OnUpdateIAPById;

    public static void ShowLog(string log)
    {
        OnLog?.Invoke($"[{DateTime.Now}]: {log}");
    }

    public static void ShowLogError(string log)
    {
        OnLogError?.Invoke($"[{DateTime.Now}]: {log}");
    }

    public static void AddIAPs()
    {
        OnAddNewIAP?.Invoke();
    }

    public static void UpdateIAPByID(string IAPid)
    {
        OnUpdateIAPById?.Invoke(IAPid);
    }

    public static void ClearEvent()
    {
        OnLogError = null;
        OnLog = null;
        OnUpdateIAPById = null;
        OnAddNewIAP = null;
    }
}
