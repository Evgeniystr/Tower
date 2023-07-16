using GooglePlayGames.BasicApi;
using GoogleMobileAds.Api;
using System;
using UnityEngine;
using Zenject;
using GooglePlayGames;
using Cysharp.Threading.Tasks;

public class GameService : MonoBehaviour
{
    public event Action OnStartupInitialize;
    public event Action OnGameStart;
    public event Action OnGameOver;

    public bool IsAuthenticated { get; private set; }

    [Inject]
    private PlayerInputService _playerInputService;

    private bool _GPGSInited;
    private bool _ADSInited;

    private const int _attemptsIntervalMS = 2000;
    private const int _maxAttemptsCount = 5;

    private int _initAttempsCounter;


    public void Initialize()
    {
#if (UNITY_EDITOR)
        OnStartupInitialize?.Invoke();
        StartGame();
#elif (UNITY_ANDROID)
        //GPGS
        //PlayGamesPlatform.DebugLogEnabled = true;//
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(OnGPGSInited);

        //ADMob
        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        //MobileAds.RaiseAdEventsOnUnityMainThread = true;
        //MobileAds.Initialize(OnADSInited);

        //Main initializing awaiter
        OnAuthenticateComplete();
#else
        throw new Exception("Unrecognized platform");
#endif
    }

    private void OnGPGSInited(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            _GPGSInited = true;
        }
        else if (status == SignInStatus.Canceled)
        {
            _GPGSInited = false;
            Debug.LogError("Authentication Canceled");
        }
        else if (status == SignInStatus.InternalError)
        {
            _GPGSInited = false;
            Debug.LogError("Authentication InternalError");
        }
    }

    private void OnADSInited(InitializationStatus status)
    {
        Debug.LogError("ADS initialisation callback recived");

        //var mediationServisesStatus = status.getAdapterStatusMap();

        //foreach (var item in mediationServisesStatus)
        //{
        //    Debug.Log($"Name: {item.Key}, Status: {item.Value.InitializationState}");

        //    if(item.Value.InitializationState == AdapterState.NotReady)
        //    {
        //        Debug.LogError("ADS initialisation fail");
        //        return;
        //    }
        //}

        //test add ---
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";//android test

        // Create a 320x50 banner at the top of the screen.
        var bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest();
        // Load the banner with the request.
        bannerView.LoadAd(request);
        //---

        _ADSInited = true;
    }


    private async UniTaskVoid OnAuthenticateComplete()
    {
        _initAttempsCounter = 0;

        while (!_GPGSInited/* || !_ADSInited*/)
        {
            _initAttempsCounter++;

            if (_GPGSInited/* && _ADSInited*/)
            {
                IsAuthenticated = true;
                Debug.Log("[GameService] Third party services initialize SUCCES");

                OnStartupInitialize?.Invoke();
                StartGame();
            }
            else
            {
                Debug.Log("Initialize status");
                Debug.Log($"GPGS status: {_GPGSInited}");
                Debug.Log($"ADS status: {_ADSInited}");

                if(_initAttempsCounter < _maxAttemptsCount)
                {
                    Debug.Log($"Wait {_ADSInited}ms for new attempt");
                    await UniTask.Delay(_attemptsIntervalMS);
                }
                else
                {
                    Debug.LogFormat("Initialize FAIL");
                }
            }
        }
    }


    public void StartGame()
    {
        _playerInputService.SetInputActive(true);
        OnGameStart?.Invoke();
    }

    public void GameOver()
    {
        _playerInputService.SetInputActive(false);
        OnGameOver?.Invoke();
    }

    private void Cleanup()
    {
    }
}
