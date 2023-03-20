using System;
using UnityEngine;
using Zenject;

public class GameService : MonoBehaviour
{
    public event Action OnStartupInitialize;
    public event Action OnGameStart;
    public event Action OnGameOver;

    [Inject]
    private PlayerInputService _playerInputService;


    private void Start()
    {
        OnStartupInitialize?.Invoke();
        StartGame();
    }


    public void StartGame()
    {
        OnGameStart?.Invoke();
        _playerInputService.SetInputActive(true);
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        _playerInputService.SetInputActive(false);
    }

    private void Cleanup()
    {
    }
}
