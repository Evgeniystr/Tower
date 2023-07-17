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


    public void Initialize()
    {
#if (UNITY_EDITOR)
        OnStartupInitialize?.Invoke();
        StartGame();
#elif (UNITY_ANDROID)
        //GPGS
        //PlayGamesPlatform.DebugLogEnabled = true;//

        OnStartupInitialize?.Invoke();
        StartGame();

        //PlayGamesPlatform.Activate();
        //PlayGamesPlatform.Instance.Authenticate(status => {
        //    OnStartupInitialize?.Invoke();
        //    StartGame();
        //});

#else
        throw new Exception("[GameService] Unexpected platform");
#endif
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
