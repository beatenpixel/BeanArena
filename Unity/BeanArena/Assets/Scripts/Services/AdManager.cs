using MicroCrew.Utils;
using UnityEngine;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

[CreateAssetMenu(fileName = "AdManager", menuName = "MicroCrew/Services/AdManager")]
public class AdManager : SingletonScriptableObject<AdManager>, IInterstitialAdListener, IRewardedVideoAdListener {

    private const int AD_TYPES = Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO | Appodeal.BANNER;

    public string androidAppKey = "65fc515e932121b5b37f7d93a72dbd683e87b011983fd040";

    private Action<bool, double> rewardedCallback;

    private bool didFinishRewarded;
    private double cachedRewardedAmount;

    private int noAdsFromFreshInstallTime = 300;
    private float showRate = 120f;
    private float lastShowTime;

    public override void Init() {
        Appodeal.setAutoCache(AD_TYPES, true);

        Appodeal.setRewardedVideoCallbacks(this);
        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setTesting(false);

        Appodeal.initialize(androidAppKey, AD_TYPES, false);
    }

    public bool TryShowInterstitial() {
        if((Game.data.timeInGame + Time.realtimeSinceStartup) < noAdsFromFreshInstallTime) {
            return false;
        }

        if (Time.realtimeSinceStartup > lastShowTime + showRate) {
            if (Appodeal.isLoaded(Appodeal.INTERSTITIAL)) {
                lastShowTime = Time.realtimeSinceStartup;

                Appodeal.show(Appodeal.INTERSTITIAL);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public bool TryShowRewarded(Action<bool, double> reward) {
        rewardedCallback = reward;
        didFinishRewarded = false;

        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO)) {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
            return true;
        } else {
            rewardedCallback = null;
            UIWindowManager.CreateWindow(new UIWData_Message(MLocalization.Get("ERROR_UNABLE_TO_SHOW_AD"),
                        new UIW_ButtonConfig(MLocalization.OK, MAssets.inst.colors[MAssets.COLOR_BUTTON_GRAY], (x) => {

                        }, null)
                ));
            return false;
        }
    }

    private void FireRewardedCallback() {
        if(didFinishRewarded && rewardedCallback != null) {
            rewardedCallback?.Invoke(didFinishRewarded, cachedRewardedAmount);
            rewardedCallback = null;
            didFinishRewarded = false;
        }
    }

    #region Rewarded Callbacks

    public void onRewardedVideoLoaded(bool precache) {
        Debug.Log("onRewardedVideoLoaded");
    }

    public void onRewardedVideoFailedToLoad() {
        Debug.Log("onRewardedVideoFailedToLoad");
    }

    public void onRewardedVideoShowFailed() {
        Debug.Log("onRewardedVideoShowFailed");
    }

    public void onRewardedVideoShown() {
        Debug.Log("onRewardedVideoShowFailed");
    }

    public void onRewardedVideoFinished(double amount, string name) {
        cachedRewardedAmount = amount;
        didFinishRewarded = true;

        MFunc.Wait(() => {
            FireRewardedCallback();
        }, 0.1f);        

        Debug.Log("onRewardedVideoFinished");
    }

    public void onRewardedVideoClosed(bool finished) {
        didFinishRewarded = finished;

        Debug.Log("onRewardedVideoClosed");
    }

    public void onRewardedVideoExpired() {
        Debug.Log("onRewardedVideoExpired");
    }

    public void onRewardedVideoClicked() {
        Debug.Log("onRewardedVideoClicked");
    }

    #endregion

    #region Interstitial Callbacks

    public void onInterstitialLoaded(bool isPrecache) {
        Debug.Log("onInterstitialLoaded");
    }

    public void onInterstitialFailedToLoad() {
        Debug.Log("onInterstitialFailedToLoad");
    }

    public void onInterstitialShowFailed() {
        Debug.Log("onInterstitialShowFailed");
    }

    public void onInterstitialShown() {
        Debug.Log("onInterstitialShown");
    }

    public void onInterstitialClosed() {
        Debug.Log("onInterstitialClosed");
    }

    public void onInterstitialClicked() {
        Debug.Log("onInterstitialClicked");
    }

    public void onInterstitialExpired() {
        Debug.Log("onInterstitialExpired");
    }

    #endregion

}
