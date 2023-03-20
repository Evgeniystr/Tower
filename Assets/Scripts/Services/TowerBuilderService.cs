using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
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
    private FruitItemSettings _fruitItemSettings;
    private TowerItem _towerBaseItem;

    private List<TowerItem> _allTowerElements;
    private TowerItem _currentTowerElement;
    private TowerItem _previousTowerElement;
    private float _maxPerfectMoveBorder;

    private bool _isInitialized;

    private FruitsPool _fruitsPool;


    public TowerBuilderService(
        [Inject(Id = Constants.TowerBaseItem)] TowerItem towerBaseItem,
        [Inject(Id = Constants.TowerParent)] Transform towerParent,
        TowerItem fruitPrefab,
        SplashService splashService,
        AudioService audioService, 
        GameService gameService, 
        PlayerInputService playerInputService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab, towerParent, this, fruitItemSettings, gameSettings);

        _splashService = splashService;
        _audioService = audioService;
        _gameService = gameService;
        _playerInputService = playerInputService;

        _towerBaseItem = towerBaseItem;
        _fruitItemSettings = fruitItemSettings;
        _gameSettings = gameSettings;

        _gameService.OnStartupInitialize += Initialize;
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        _allTowerElements = new List<TowerItem>();

        _towerBaseItem.GetComponent<MeshRenderer>().material = _fruitItemSettings.GetRandomMaterial();

        _gameService.OnGameStart += OnGameStart;
        _gameService.OnGameOver += OnGameEnd;

        _maxPerfectMoveBorder = _towerBaseItem.Size * (1 - _gameSettings.PerfectMoveSizeCoef);
    }

    private void OnGameStart()
    {
        _playerInputService.OnTapEvent += StartMakeNewFruit;
        _playerInputService.OnReleaseEvent += StopMakeNewFruit;

        CleareTower();

        _previousTowerElement = null;
    }

    public async void CleareTower()
    {
        if (_allTowerElements.Count == 0)
            return;

        var duration = 1f;
        var curentDuration = duration;
        int lastTowerItemIndex = 0;

        while (curentDuration > 0)
        {
            var t = Mathf.InverseLerp(0, duration, curentDuration);

            var pointedItemIndex = (int)Mathf.Lerp(0, _allTowerElements.Count -1, t);

            if(lastTowerItemIndex != pointedItemIndex)
            {
                var towerItem = _allTowerElements[pointedItemIndex];

                //scale down
                towerItem.FadeSizeToZero(0.1f, () => _fruitsPool.ReleaseItem(towerItem));
                
                lastTowerItemIndex = pointedItemIndex;
            }

            curentDuration -= Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate();
        }
    }

    private void OnGameEnd()
    {
        _playerInputService.OnTapEvent -= StartMakeNewFruit;
        _playerInputService.OnReleaseEvent -= StopMakeNewFruit;
    }

    public TowerItem GetLastTowerItem()
    {
        return _allTowerElements.Count == 0 ?
            _towerBaseItem :
            _allTowerElements.Last();
    }

    private void StartMakeNewFruit()
    {
        SetupCurrentElement();

        _currentTowerElement.StartGrowing();
    }

    private void SetupCurrentElement()
    {
        _currentTowerElement = _fruitsPool.Get();

        var prevoiusItem = _previousTowerElement == null ? _towerBaseItem : _previousTowerElement;
        _currentTowerElement.Setup(prevoiusItem);
    }

    private void StopMakeNewFruit()
    {
        if (_currentTowerElement == null)
            return;

        _currentTowerElement.StopGrowing();

        _allTowerElements.Add(_currentTowerElement);

        if (PerfectMoveCheck())
            OnPerfectMove();
        else
            OnNormalMove();

        _previousTowerElement = _currentTowerElement;
    }

    private void OnNormalMove()
    {
        OnTowerItemPlaced?.Invoke(false);

        _audioService.DoSplatterSound();
    }

    private void OnPerfectMove()
    {
        PerfectMovePerform();

        _splashService.DoSplash();

        OnTowerItemPlaced?.Invoke(true);

        _audioService.DoPerfectSplatterSound();
    }

    private bool PerfectMoveCheck()
    {
        if (_previousTowerElement == null)
            return false;

        var currentPerfectBorder = _previousTowerElement.Size * (1 - _gameSettings.PerfectMoveSizeCoef);
        var perfectMoveBorderValue = MathF.Min(currentPerfectBorder, _maxPerfectMoveBorder);

        var isPerfectMove = 
            _currentTowerElement.Size >= perfectMoveBorderValue && 
            _currentTowerElement.Size < _previousTowerElement.Size;

        return isPerfectMove;
    }

    private void PerfectMovePerform()
    {
        var elementsLastIndex = _allTowerElements.Count - 1;

        for (int i = elementsLastIndex; i >= 0; i--)//cycle from tower top to bottom
        {
            var towerElement = _allTowerElements[i];

            var waveStartDelay = ((elementsLastIndex - i) * _gameSettings.VaweDelayStep);
            var isTopItem = i == elementsLastIndex;

            towerElement.DoWave(waveStartDelay, isTopItem);
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

        await UniTask.Delay(1000);//time for look at losing item

        _fruitsPool.ReleaseItem(failedElement);
        _allTowerElements.Remove(failedElement);

        _gameService.GameOver();
    }
}
