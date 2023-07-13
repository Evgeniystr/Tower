using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using System.Linq;

public class TowerBuilderService
{
    public event Action<bool> OnTowerItemPlaced;//bool - isPerfect

    private SplashService _splashService;
    private AudioService _audioService;
    private GameService _gameService;
    private PlayerInputService _playerInputService;
    private GameSettings _gameSettings;
    private CameraSettings _cameraSettings;
    private TowerItem _towerBaseItem;

    private List<TowerItem> _allTowerElements;
    private TowerItem _currentTowerElement;
    private TowerItem _previousTowerElement;

    private bool _isInitialized;
    private bool _fruitGrowthInProgress;

    private FruitsPool _fruitsPool;

    private int _perfectMoveCounter;


    public TowerBuilderService(
        [Inject(Id = Constants.TowerParent)] Transform towerParent,
        TowerItem fruitPrefab,
        SplashService splashService,
        AudioService audioService, 
        GameService gameService, 
        PlayerInputService playerInputService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings,
        CameraSettings cameraSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab, towerParent, this, fruitItemSettings, gameSettings);

        _splashService = splashService;
        _audioService = audioService;
        _gameService = gameService;
        _playerInputService = playerInputService;

        _gameSettings = gameSettings;
        _cameraSettings = cameraSettings;

        _gameService.OnStartupInitialize += Initialize;
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;

        _allTowerElements = new List<TowerItem>();

        _gameService.OnGameStart += OnGameStart;
        _gameService.OnGameOver += OnGameEnd;

        _isInitialized = true;
    }

    private async void OnGameStart()
    {
        await CleareTower();
        await SetBaseItem();

        _perfectMoveCounter = 0;

        _playerInputService.OnTapEvent += StartMakeNewFruit;
        _playerInputService.OnReleaseEvent += StopMakeNewFruit;
    }

    private async UniTask SetBaseItem()
    {
        _towerBaseItem = _fruitsPool.Get();
        await _towerBaseItem.SetupAsBaseItem();
        _previousTowerElement = _towerBaseItem;
        _allTowerElements.Add(_towerBaseItem);
    }

    public async UniTask CleareTower()
    {
        if (_allTowerElements.Count == 0)
            return;

        int timePerItemMS = (int)(_cameraSettings.CamFailAnimDuration / 2) * 1000 / _allTowerElements.Count;

        int defaultItemsPerSecond = 15;
        int defaultItemDelayMS = 1000 / defaultItemsPerSecond;

        int resultItemDelayMS = Mathf.Min(timePerItemMS, defaultItemDelayMS);

        float scaleDownDuration = 0.15f;

        for (int i = _allTowerElements.Count-1; i >= 0; i--)
        {
            var towerItem = _allTowerElements[i];
            towerItem.FadeSizeToZero(scaleDownDuration, () => _fruitsPool.ReleaseItem(towerItem));
            await UniTask.Delay(resultItemDelayMS);
        }

        _allTowerElements.Clear();
    }

    private void OnGameEnd()
    {
        _playerInputService.OnTapEvent -= StartMakeNewFruit;
        _playerInputService.OnReleaseEvent -= StopMakeNewFruit;
    }

    public TowerItem GetLastTowerItem()
    {
        return _allTowerElements.Count == 0 ?
            _towerBaseItem:
            _allTowerElements.Last();
    }

    private void StartMakeNewFruit()
    {
        if (_fruitGrowthInProgress)
            return;

        _fruitGrowthInProgress = true;

        SetupCurrentElement();

        var speedMultiplier = Mathf.Clamp(_previousTowerElement.Size, 1, 2);
        _currentTowerElement.StartGrowing(speedMultiplier);
    }

    private void SetupCurrentElement()
    {
        _currentTowerElement = _fruitsPool.Get();

        var prevoiusItem = _previousTowerElement == null ? _towerBaseItem : _previousTowerElement;
        _currentTowerElement.Setup(prevoiusItem);
    }

    private void StopMakeNewFruit()
    {
        if (!_fruitGrowthInProgress)
            return;
        _fruitGrowthInProgress = false;

        if (_currentTowerElement == null)
            return;

        _currentTowerElement.StopGrowing();

        _allTowerElements.Add(_currentTowerElement);

        var isPerfectMove = PerfectMoveCheck();

        if (isPerfectMove)
            OnPerfectMove();
        else
            OnNormalMove();

        OnTowerItemPlaced?.Invoke(isPerfectMove);

        _previousTowerElement = _currentTowerElement;
    }

    private void OnNormalMove()
    {
        _perfectMoveCounter = 0;
        _audioService.DoSplatterSound();
    }

    private void OnPerfectMove()
    {
        _perfectMoveCounter++;

        _splashService.DoSplash();

        _audioService.DoPerfectSplatterSound();

        if(_perfectMoveCounter > _gameSettings.PerfectMoveStreakRequired)
            DoPerfectMoveWave();
    }

    private bool PerfectMoveCheck()
    {
        if (_previousTowerElement == null)
            return false;
        
        var triggerValue = _previousTowerElement.GetPrefectMoveTriggerValue();

        var isPerfectMove =
            _currentTowerElement.Size > triggerValue &&
            _currentTowerElement.Size < _previousTowerElement.Size;

        return isPerfectMove;
    }

    private void DoPerfectMoveWave()
    {
        var elementsLastIndex = _allTowerElements.Count - 1;

        var waveLength = MathF.Min(_gameSettings.WaveLength, _allTowerElements.Count);
        var iterationLastItem = _allTowerElements.Count - waveLength;

        var powerScaleStep = 1f / _gameSettings.WaveLength;
        var powerValue = 1f;

        for (int i = elementsLastIndex; i >= iterationLastItem; i--)//cycle from tower top to bottom
        {
            var towerElement = _allTowerElements[i];

            var waveStartDelay = ((elementsLastIndex - i) * _gameSettings.VaweDelayStep);
            var isTopItem = i == elementsLastIndex;

            towerElement.DoWave(waveStartDelay, isTopItem, powerValue);
            powerValue -= powerScaleStep;
        }
    }

    public void Lose()
    {
        _currentTowerElement.StopGrowing();

        _playerInputService.SetInputActive(false);

        DestroyLoseElement();
    }

    private async UniTaskVoid DestroyLoseElement()
    {
        var failedElement = _currentTowerElement;
        _currentTowerElement = null;

        //change lose material to red
        failedElement.SetLoseView();

        await UniTask.Delay(500);//time for look at losing item

        _fruitsPool.ReleaseItem(failedElement);
        _allTowerElements.Remove(failedElement);

        _gameService.GameOver();
    }
}
