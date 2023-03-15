using System;
using UnityEngine;
using Zenject;

public class GameService : MonoBehaviour
{
    public event Action OngameStart;
    public event Action OnGameOver;

    [Inject]
    private PlayerInputService _playerInputService;


    private void Start()
    {
        StartGame();
    }


    public void StartGame()
    {
        OngameStart?.Invoke();
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
