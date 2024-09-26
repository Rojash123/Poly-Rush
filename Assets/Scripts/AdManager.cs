using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AdManager : MonoBehaviour
{
    private static AdManager instance;
    public static AdManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }
    public string appId = "ca-app-pub-7231474520345903~7644202989";


#if UNITY_ANDROID
    string interstitialId = "ca-app-pub-7231474520345903/8685200109";
    string rewardedId = "ca-app-pub-7231474520345903/8663452136";

#elif UNITY_IPHONE
    string appOpenId ="";
    string interId = "";
    string rewardedId = "";

#endif

    public AppOpenAd appOpenAd;
    public InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;

    private void Start()
    {
        LoadAllAds();
    }
    public void LoadAllAds()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    #region IntersitialAd
    public bool isInterstitialAdAvailable
    {
        get
        {
            return interstitialAd != null;
        }
    }
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(interstitialId, adRequest,
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
                RegisterEventHandlers(interstitialAd);

            });
    }
    public void ShowInterstitialAd()
    {
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
    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
        };
    }
    #endregion

    #region RewardedAd
    public bool isRewardAdAvailable
    {
        get
        {
            return interstitialAd != null;
        }
    }
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardedId, adRequest,
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
                RegisterEventHandlersRewardedAds(rewardedAd);
            });
    }
    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                UIManager.Instance.OnRewardAdWatched();
                SoundManager.Instance.bgMusic.Play();
            });
        }
    }
    public GameObject loadingAd,PleaseCheckInternetConnection;
    public void ShowRewardedAdLevelUnlock()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            while (!rewardedAd.CanShowAd())
            {
                loadingAd.SetActive(true);
            }
            loadingAd.SetActive(false);
        }
       
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                int temp = PlayerPrefs.GetInt("AdWatchLeft");
                PlayerPrefs.SetInt("AdWatchLeft", temp - 1);
                GameLoadingFirst.Instance.SetDynamicText();
            });
        }
        else
        {
            PleaseCheckInternetConnection.SetActive(true);
            return;
        }
    }

    public void ShowRewardedAStore()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                GameLoadState.coinAmt += 50;
                SaveAndLoadData.SaveData();
            });
        }
        else
        {
            PleaseCheckInternetConnection.SetActive(true);
        }
    }

    float adTimer = 0;
    public void ShowAdOnAppropriateCondition()
    {
        if (Time.time - adTimer > 200)
        {
            AdManager.Instance.ShowInterstitialAd();
            adTimer += 200;
        }
    }
    private void RegisterEventHandlersRewardedAds(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
        };
    }
    #endregion
}
