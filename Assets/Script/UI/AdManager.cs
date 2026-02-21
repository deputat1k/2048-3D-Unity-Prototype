using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    private BannerView bannerView;

    //“≈—“ќ¬»… ID банера в≥д Google 
    //коли гра буде заливатись у плей потр≥бно зм≥нити на св≥й  ad unit id
    private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        RequestBanner();
    }

    private void RequestBanner()
    {
        // якщо банер ≥снуЇ видал€Їмо старий
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // —творюЇмо новий банер. 
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        // зарпит на рекламу
        AdRequest request = new AdRequest();

        // завантажуЇмо рекламу на банер
        bannerView.LoadAd(request);
    }
}