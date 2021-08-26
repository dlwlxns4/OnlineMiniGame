using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using Photon.Pun;
using Photon.Realtime;

public class AdmobManager : MonoBehaviourPunCallbacks
{
    public bool isTestMode;
    public Text LogText;
    public Button FrontAdsBtn, RewardAdsBtn;
    

    void Start()
    {
         var requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() { "324B5A29D2CFC85B" }) // test Device ID
            .build();

        MobileAds.SetRequestConfiguration(requestConfiguration);

        LoadBannerAd();
        // LoadFrontAd();
    }

    void Update()
    {
        // FrontAdsBtn.interactable = frontAd.IsLoaded();
    }

    AdRequest GetAdRequest() 
    {
        return new AdRequest.Builder().Build();
    }



    #region 배너 광고
    const string bannerTestID = "ca-app-pub-3940256099942544/6300978111";
    const string bannerID = "ca-app-pub-1220123488916055/8761662127";
    BannerView bannerAd;


    void LoadBannerAd()
    {
        bannerAd = new BannerView(isTestMode ? bannerTestID : bannerID,
            AdSize.SmartBanner, AdPosition.Top);
        bannerAd.LoadAd(GetAdRequest());
        ToggleBannerAd(false);
        bannerAd.Show();
    }

    public void ToggleBannerAd(bool b)
    {
        if (b) bannerAd.Show();
        else bannerAd.Hide();
    }
    #endregion

    public void HideBannderAd(){
        bannerAd.Hide();
    }
    
    public void ShowBannderAd(){
        bannerAd.Show();
    }

    #region 전면 광고
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = "ca-app-pub-1220123488916055/5570174962";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdClosed += (sender, e) => 
        {
            // LogText.text = "전면광고 성공";
        };
    }

    public void ShowFrontAd()
    {
        frontAd.Show();
        LoadFrontAd();
    }
    #endregion
}
