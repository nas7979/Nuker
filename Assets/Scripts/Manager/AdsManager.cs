using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using BackEnd;

public class AdsManager : MonoSingleton<AdsManager>
{
	private InterstitialAd mAd_Interstitial;
	private RewardedAd mAd_Reward;
	private string m_AdInterID = "ca-app-pub-3940256099942544/1033173712";
	private string m_AdRewardID = "ca-app-pub-3940256099942544/5224354917";
	private int mInterstitialAdsCount = 0;

	private void Awake()
	{
		mAd_Interstitial = new InterstitialAd(m_AdInterID);
		MobileAds.Initialize(_InitStatus => { });

		OnAdClosed(null, null);

		DontDestroyOnLoad(gameObject);
	}

	public void OnAdOpening(object _Sender, EventArgs _Args)
	{
		Time.timeScale = 0;
		SoundManager.Instance.PauseAll();
		mInterstitialAdsCount = 0;
	}

	public void OnAdClosed(object _Sender, EventArgs _Args)
	{
		Time.timeScale = 1;
		Debug.Log("Loading");
		mAd_Reward = new RewardedAd(m_AdRewardID);
		mAd_Reward.OnAdOpening += OnAdOpening;
		mAd_Reward.OnAdClosed += OnAdClosed;
		mAd_Reward.OnUserEarnedReward += OnAdRewarded;
		AdRequest Req = new AdRequest.Builder().Build();
		mAd_Reward.LoadAd(Req);
		mAd_Interstitial.Destroy();
		mAd_Interstitial = new InterstitialAd(m_AdInterID);
		mAd_Interstitial.OnAdOpening += OnAdOpening;
		mAd_Interstitial.OnAdClosed += OnAdClosed;
		Req = new AdRequest.Builder().Build();
		mAd_Interstitial.LoadAd(Req);
	}

	public void OnAdRewarded(object _Sender, Reward _Reward)
	{

	}

	public void ShowInterstitialAd()
	{
		if (SaveSystem.Instance.Data.IsAdRemoved == true)
			return;
		mInterstitialAdsCount++;
		if (mInterstitialAdsCount != 2)
			return;
		Debug.Log(mAd_Interstitial.IsLoaded());
		if (mAd_Interstitial.IsLoaded())
		{
			mAd_Interstitial.Show();
		}
	}

	public void ShowRewardAd()
	{
		AdRequest Request = new AdRequest.Builder().Build();
		mAd_Reward.LoadAd(Request);
		if (mAd_Reward.IsLoaded())
		{
			mAd_Reward.Show();
		}
	}
}
