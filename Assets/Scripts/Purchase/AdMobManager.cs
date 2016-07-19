using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdMobManager : SingletonMonoBehaviourFast<AdMobManager> {
	public string Android_Banner		= "ca-app-pub-2805884795874004/8649677347";
	public string Android_Interstitial	= "ca-app-pub-2805884795874004/8649677347";

	public string Android_RewardedVide	= "ca-app-pub-2805884795874004/6970807743";

	public string ios_Banner			= "ca-app-pub-2805884795874004/8649677347";
	public string ios_Interstitial		= "ca-app-pub-2805884795874004/8649677347";

	private InterstitialAd interstitial;
	private AdRequest request;
	private AdRequest requestReward;

	private RewardBasedVideoAd rewardedAd;

	bool is_close_interstitial = false; 

	// Use this for initialization
	void Awake () {
		// 起動時にインタースティシャル広告をロードしておく
		RequestInterstitial ();
		// バナー広告を表示
		RequestBanner ();

		//動画広告をロード
		RequestRewardAds();
	}

	public void RequestBanner() {
		#if UNITY_ANDROID
		string adUnitId = Android_Banner;
		#elif UNITY_IPHONE
		string adUnitId = ios_Banner;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the top of the screen.
		BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
			.AddTestDevice("F7D9F6DAF5E046F7028A525895AEAAD7")
			.AddTestDevice("065A88DF674C8C0B527647FFF568EB06") //nexus 5
		.Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}

	//****************** Interstitial Ads ******************//

	public void RequestInterstitial() {
		#if UNITY_ANDROID
		string adUnitId = Android_Interstitial;
		#elif UNITY_IPHONE
		string adUnitId = ios_Interstitial;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		if (is_close_interstitial == true) {
			interstitial.Destroy ();
		}

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd (adUnitId);
		// Create an empty ad request.
		request = new AdRequest.Builder ()
		.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
		.AddTestDevice("F7D9F6DAF5E046F7028A525895AEAAD7")
		.AddTestDevice("065A88DF674C8C0B527647FFF568EB06") //nexus 5
		.Build ();
		// Load the interstitial with the request.
		interstitial.LoadAd (request);

		interstitial.OnAdClosed += HandleAdClosed;

		is_close_interstitial = false;
	}

	public void ShowInterstitialAds() {
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		}
	}

	// インタースティシャル広告を閉じた時に走る
	void HandleAdClosed (object sender, System.EventArgs e) {
		is_close_interstitial = true;
	}

	//****************** Video Reward Ads ******************//

	public void RequestRewardAds() {
		#if UNITY_ANDROID
		string adUnitId = Android_RewardedVide;
		#elif UNITY_IPHONE
		string adUnitId = ios_Interstitial;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		rewardedAd = RewardBasedVideoAd.Instance;
		requestReward = new AdRequest.Builder ()
		.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
		.AddTestDevice("F7D9F6DAF5E046F7028A525895AEAAD7")
		.AddTestDevice("065A88DF674C8C0B527647FFF568EB06") //nexus 5
		.Build ();
		rewardedAd.LoadAd (requestReward, adUnitId);

		rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
	}

	private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) {
		Debug.LogError ("Error on loading reward video ads: " + e.Message);
	}

	public void ShowRewardAds() {
		if (rewardedAd.IsLoaded ()) {
			rewardedAd.Show ();
		}
	}
}