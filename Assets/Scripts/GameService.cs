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

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        OnStartupInitialize?.Invoke();

#if (UNITY_EDITOR)
#elif (UNITY_ANDROID)
        //GPG
        //PlayGamesPlatform.DebugLogEnabled = true;//

        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(status => 
        {
            Debug.Log($"[GameService] GPGS Authenticate status: {status}");

            if (status == SignInStatus.Success)
                IsAuthenticated = true;

            AnaliticsTool.LogGPGSAuthenticateStatus(status.ToString());
        });

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
