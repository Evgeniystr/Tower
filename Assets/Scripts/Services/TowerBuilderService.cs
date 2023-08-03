using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using System.Linq;

public class TowerBuilderService
{
    public event Action<bool> OnTowerItemPlaced;//bool - isPerfect
    public event Action OnSecondChancePropoposal;

    private SplashService _splashService;
    private AudioService _audioService;
    private GameService _gameService;
    private PlayerInputService _playerInputService;
    private ADSService _adService;
    private GameSettings _gameSettings;
    private CameraSettings _cameraSettings;
    private TowerItem _towerBaseItem;

    private List<TowerItem> _allTowerElements;
    private TowerItem _currentTowerElement;
    private TowerItem _previousTowerElement;

    private bool _isInitialized;
    private bool _fruitGrowthInProgress;
    private bool _adRewadrRecived;

    private FruitsPool _fruitsPool;

    private int _perfectMoveCounter;
    private int _totalPerfectMoveCounter;
    private int _bestPerfectStreak;

    private const int _minTowerHeightIncrementor = 5;
    public const int AdPerTowerLevel = 30;
    private int _minTowerHeightForAd;
    public int UsedAdCounter { get; private set; }


    public TowerBuilderService(
        [Inject(Id = Constants.TowerParent)] Transform towerParent,
        TowerItem fruitPrefab,
        SplashService splashService,
        AudioService audioService,
        GameService gameService,
        PlayerInputService playerInputService,
        ADSService adService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings,
        CameraSettings cameraSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab, towerParent, this, fruitItemSettings, gameSettings);

        _splashService = splashService;
        _audioService = audioService;
        _gameService = gameService;
        _playerInputService = playerInputService;
        _adService = adService;

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
        _adService.OnRewardRecived += (type, amount) => TakeSecondChance();
        _adService.OnAdClosed += OnAdClosed;

        _isInitialized = true;
    }

    private async void OnGameStart()
    {
        await CleareTower();
        await SetBaseItem();

        UsedAdCounter = 0;
        _perfectMoveCounter = 0;
        _totalPerfectMoveCounter = 0;
        _bestPerfectStreak = 0;
        _minTowerHeightForAd = _minTowerHeightIncrementor;

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
        AnaliticsTool.LogGameResult(_allTowerElements.Count, UsedAdCounter);
        AnaliticsTool.LogPerfectMovesStatistic(_totalPerfectMoveCounter, _allTowerElements.Count, _bestPerfectStreak);

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
        if (_perfectMoveCounter > _bestPerfectStreak)
            _bestPerfectStreak = _perfectMoveCounter;

        _perfectMoveCounter = 0;
        _audioService.DoSplatterSound();
    }

    private void OnPerfectMove()
    {
        _perfectMoveCounter++;
        _totalPerfectMoveCounter++;

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

    public async UniTaskVoid Lose()
    {
        _fruitGrowthInProgress = false;

        _currentTowerElement.StopGrowing();

        _playerInputService.SetInputActive(false);


        await DestroyLoseElement();


        if (CanTakeSecondChanceCheck())
        {
            OnSecondChancePropoposal?.Invoke();
        }
        else
        {
            _gameService.GameOver();
        }
    }

    //ad succes
    private void TakeSecondChance()
    {
        _adRewadrRecived = true;
        UsedAdCounter++;
        _minTowerHeightForAd = _allTowerElements.Count + _minTowerHeightIncrementor;
        DoPerfectMoveWave();
        _playerInputService.SetInputActive(true);
    }

    private void OnAdClosed()
    {
        if(!_adRewadrRecived)
            _gameService.GameOver();

        _adRewadrRecived = false;
    }

    private async UniTask DestroyLoseElement()
    {
        var failedElement = _currentTowerElement;

        //change lose material to red
        failedElement.SetLoseView();

        await UniTask.Delay(500);//time for look at losing item

        _fruitsPool.ReleaseItem(failedElement);
        _allTowerElements.Remove(failedElement);

        _currentTowerElement = _allTowerElements.Last();
    }

    private bool CanTakeSecondChanceCheck()
    {
        if(!_adService.RewardReadyCheck())
            return false;

        var towerLevel = _allTowerElements.Count - 1; //substract base level
        var chansesForCurrentLevel = towerLevel / AdPerTowerLevel + 1;
        var isCanTakeChance =
            UsedAdCounter < chansesForCurrentLevel && //is has unused chances
            towerLevel >= _minTowerHeightForAd; //is tower meet minimal height

        return isCanTakeChance;
    }
}
