using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class ADSService : MonoBehaviour
{
    public event Action<string, double> OnRewardRecived;//type amount

    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            LoadRewardedAd();
        });
    }

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    //private string _adRewardedUnitId = "ca-app-pub-3940256099942544/5224354917";//test ad
    private string _adRewardedUnitId = "ca-app-pub-6811435242766154/9738732258";
#elif UNITY_IPHONE
  private string _adRewardedUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adRewardedUnitId = "unused";
#endif

    private RewardedAd rewardedAd;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    private void LoadRewardedAd()
    {
        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");

        // send the request to load the ad.
        RewardedAd.Load(_adRewardedUnitId, adRequest,
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

                RegisterEventHandlers(rewardedAd);
            });
    }

    public bool RewardReadyCheck()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (RewardReadyCheck())
        {
            rewardedAd.Show((Reward reward) =>
            {
                //TODO give reward
                OnRewardRecived?.Invoke(reward.Type, reward.Amount);
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
        else
        {
            Debug.LogError("Ad not ready");
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += AdPaid;

        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += AdImpressionRecorded;

        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += AdClicked;

        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += AdFullScreenContentOpened;

        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += AdFullScreenContentClosed;

        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += AdFullScreenContentFailed;
    }

    private void UnregisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid -= AdPaid;
        ad.OnAdImpressionRecorded -= AdImpressionRecorded;
        ad.OnAdClicked -= AdClicked;
        ad.OnAdFullScreenContentOpened -= AdFullScreenContentOpened;
        ad.OnAdFullScreenContentClosed -= AdFullScreenContentClosed;
        ad.OnAdFullScreenContentFailed -= AdFullScreenContentFailed;
    }

    private void AdPaid(AdValue adValue)
    {
        Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
    }

    private void AdImpressionRecorded()
    {
        Debug.Log("Rewarded ad recorded an impression.");
    }

    private void AdClicked()
    {
        Debug.Log("Rewarded ad was clicked.");
    }

    private void AdFullScreenContentOpened()
    {
        Debug.Log("Rewarded ad full screen content opened.");
    }

    private void AdFullScreenContentClosed()
    {
        Debug.Log("Rewarded ad full screen content closed.");
        // Clean up the old ad before loading a new one.
        UnregisterEventHandlers(rewardedAd);
        rewardedAd.Destroy();
        rewardedAd = null;

        // Reload the ad so that we can show another as soon as possible.
        LoadRewardedAd();
    }

    private void AdFullScreenContentFailed(AdError error)
    {
        Debug.LogError("Rewarded ad failed to open full screen content " +
               "with error : " + error);

        // Reload the ad so that we can show another as soon as possible.
        LoadRewardedAd();
    }
}
