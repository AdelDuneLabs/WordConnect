using BBG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using WordConnect;

public class IAPManager : MonoBehaviour
{

    private const string noads = "com.dunelabs.arabicwords.noads";
    private const string coins240 = "com.dunelabs.arabicwords.coins240";
    private const string coins720 = "com.dunelabs.arabicwords.coins720";
    private const string coins1340 = "com.dunelabs.arabicwords.coins1340";

    [SerializeField] private GameObject restoreButton;



    private void Awake()
    {

        restoreButton.SetActive(false);

#if UNITY_IOS
        restoreButton.SetActive(true);
#endif


    }





    public void OnPurchaseComplete(Product product)
    {

        if(product.definition.id== noads)
        {
           
            PlayerPrefs.SetInt("NoAds", 1);
            AdsManager.instance.DestroyAd();
        }
        if (product.definition.id == coins240)
        {
            GameController.Instance.GiveCoinsIAP(240);
        }
        if (product.definition.id == coins720)
        {
            GameController.Instance.GiveCoinsIAP(720);
        }
        if (product.definition.id == coins1340)
        {
            GameController.Instance.GiveCoinsIAP(1340);
        }

        PopupManager.Instance.Show("product_purchased");

    }


    public void OnPurchaseFailed(Product product ,PurchaseFailureDescription purchaseFailureDescription)
    {

    }


}
