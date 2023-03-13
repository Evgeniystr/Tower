using System;

public class GameService
{
    public event Action RestartEvent;
    public event Action OnGameOver;

    private PlayerInputService _playerInputService;
    private CameraService _cameraService;


    public GameService(PlayerInputService playerInputService, CameraService cameraService)
    {
        _playerInputService = playerInputService;
        _cameraService = cameraService;

        _cameraService.OnLoseCamStop += GameOverResults;
    }


    public void RestartGame()
    {
        RestartEvent?.Invoke();
        _playerInputService.SetInputActive(true);
    }

    public void GameOverResults()
    {
        OnGameOver?.Invoke();
        _playerInputService.SetInputActive(false);
    }

    private void Cleanup()
    {
        _cameraService.OnLoseCamStop -= GameOverResults;
    }
}
