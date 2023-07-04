using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;
using Zenject;

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
#if (UNITY_EDITOR)
        OnStartupInitialize?.Invoke();
        StartGame();
#elif (UNITY_ANDROID)
        //PlayGamesPlatform.DebugLogEnabled = true;//
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(OnAuthenticateComplete);
#else
        throw new Exception("Unrecognized platform");
#endif
    }

    private void OnAuthenticateComplete(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            IsAuthenticated = true;

            OnStartupInitialize?.Invoke();
            StartGame();
        }
        else if(status == SignInStatus.Canceled)
        {
            IsAuthenticated = false;
            Debug.LogError("Authentication Canceled");
        }
        else if (status == SignInStatus.InternalError)
        {
            IsAuthenticated = false;
            Debug.LogError("Authentication InternalError");
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
