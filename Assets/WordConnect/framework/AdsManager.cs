using BBG;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using WordConnect;

public class AdsManager : MonoBehaviour
{

   // ca-app-pub-9657355948721151~1949265434

    public static AdsManager instance { get; private set; }


    public bool IsNoAds => PlayerPrefs.GetInt("NoAds")==1;

#if UNITY_ANDROID
  private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-9657355948721151/7936676013";
#else
  private string _adUnitId = "unused";
#endif


#if UNITY_ANDROID
  private string _adUnitIdBanner = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string _adUnitIdBanner = "ca-app-pub-9657355948721151/8155581135";
#endif

#if UNITY_ANDROID
  private string _adUnitIdReward = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string _adUnitIdReward = "ca-app-pub-3940256099942544/1712485313";
#endif

    private RewardedAd rewardedAd;
    BannerView _bannerView;

    [SerializeField] private bool isTest = true;


    private InterstitialAd interstitialAd;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
   

  



    private void Awake()
    {

        instance = this;
        DontDestroyOnLoad(gameObject);


    }


    private void Start()
    {

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        LoadInterstitialAd();
        CreateBannerView();
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        

        // send the request to load the ad.
        RewardedAd.Load(_adUnitIdReward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }

    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
            });
    }

    public void ShowAd()
    {
        if (IsNoAds) { return; }
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    public bool isReadyReward => rewardedAd.CanShowAd();


    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                OnRewardAdGranted();
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }


    private void OnRewardAdGranted()
    {

        var coinsToReward = 10;
        // Get the current amount of coins
        int animateFromCoins = GameController.Instance.Coins;

        // Give the amount of coins
        GameController.Instance.GiveCoins(coinsToReward, false);

        // Get the amount of coins now after giving them
        int animateToCoins = GameController.Instance.Coins;

        // Show the popup to the user so they know they got the coins
        PopupManager.Instance.Show("reward_ad_granted", new object[] { coinsToReward, animateFromCoins, animateToCoins });
    }






    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        if (IsNoAds) { return; }
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitIdBanner, AdSize.Banner, AdPosition.Bottom);
        _bannerView.Show();
    }


    public void DestroyAd()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner ad.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }











}






