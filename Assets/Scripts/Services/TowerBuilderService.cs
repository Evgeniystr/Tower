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
    private Transform _towerBaseItem;

    private List<GameObject> _allTowerElements;
    private GameObject _currentTowerElement;
    private GameObject _previousTowerElement;

    private FruitsPool _fruitsPool;

    private float _maxPerfectMoveOffset;
    private bool _isGrowing;

    public TowerBuilderService(
        [Inject(Id = Constants.FruitPrefab)] GameObject fruitPrefab,
        [Inject(Id = Constants.TowerBaseItem)] Transform towerBaseItem,
        [Inject(Id = Constants.TowerParent)] Transform towerParent,
        SplashService splashService,
        AudioService audioService, 
        GameService gameService, 
        PlayerInputService playerInputService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab, towerParent);

        _splashService = splashService;
        _audioService = audioService;
        _gameService = gameService;
        _playerInputService = playerInputService;

        _towerBaseItem = towerBaseItem;
        _fruitItemSettings = fruitItemSettings;
        _gameSettings = gameSettings;

        Initialize();
    }

    private void Initialize()
    {
        _allTowerElements = new List<GameObject>();

        _towerBaseItem.GetComponent<MeshRenderer>().material = _fruitItemSettings.GetRandomMaterial();

        _gameService.OngameStart += OnGameStart;
        _gameService.OnGameOver += OnGameEnd;

        var maxPerfectMoveOffset =
            _towerBaseItem.GetComponent<MeshFilter>().mesh.bounds.size.x *
            _towerBaseItem.localScale.x *
            _gameSettings.PerfectMoveSizeCoef;
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
                towerItem.transform.
                    DOScale(new Vector3(0, towerItem.transform.localScale.y, 0), 0.1f).
                    OnComplete(() => _fruitsPool.ReleaseItem(towerItem));
                
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

    public GameObject GetLastTowerItem()
    {
        return _allTowerElements.Count == 0 ?
            _towerBaseItem.gameObject :
            _allTowerElements.Last();
    }

    private void StartMakeNewFruit()
    {
        SetupCurrentElement();

        ScaleUpProcess();
    }

    private async UniTaskVoid ScaleUpProcess()
    {
        _isGrowing = true;

        var failSize = _previousTowerElement == null ?
            _towerBaseItem.localScale.x :
            _previousTowerElement.transform.localScale.x;

        while (_isGrowing)
        {
            var scaleUpValue = _gameSettings.ItemScaleUpSpeed * Time.fixedDeltaTime;

            var newScale = new Vector3(
                _currentTowerElement.transform.localScale.x + scaleUpValue,
                _currentTowerElement.transform.localScale.y,
                _currentTowerElement.transform.localScale.z + scaleUpValue);

            _currentTowerElement.transform.localScale = newScale;

            await UniTask.WaitForFixedUpdate();

            if (IsLoseCheck(failSize))
                Lose();
        }
    }

    private void SetupCurrentElement()
    {
        _currentTowerElement = _fruitsPool.Get();

        _currentTowerElement.transform.localScale = new Vector3(0, _currentTowerElement.transform.localScale.y, 0);

        //set position
        float baseYpos = _previousTowerElement == null ?
            _towerBaseItem.position.y : 
            _previousTowerElement.transform.position.y;

        float newYpos = _currentTowerElement.GetComponent<Renderer>().bounds.size.y + baseYpos;
        _currentTowerElement.transform.position = new Vector3(0, newYpos, 0);

        //set material
        _currentTowerElement.GetComponent<MeshRenderer>().material = _fruitItemSettings.GetRandomMaterial();
    }

    private void StopMakeNewFruit()
    {
        if (!_isGrowing)
            return;

        _isGrowing = false;

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

        //per item calculations
        var currentItemSize = 
            _currentTowerElement.GetComponent<MeshFilter>().mesh.bounds.size.x * 
            _currentTowerElement.transform.localScale.x;

        var previousItemSize = 
            _previousTowerElement.GetComponent<MeshFilter>().mesh.bounds.size.x * 
            _previousTowerElement.transform.localScale.x;

        var prevItemOffsetSize = previousItemSize * _gameSettings.PerfectMoveSizeCoef;
        var offset = MathF.Min(prevItemOffsetSize, _maxPerfectMoveOffset);

        var perfectMoveBorderValue = previousItemSize - offset;

        var isPerfectMove = currentItemSize >= perfectMoveBorderValue;

        return isPerfectMove;
    }

    private void PerfectMovePerform()
    {
        var elementsLastIndex = _allTowerElements.Count - 1;

        for (int i = elementsLastIndex; i >= 0; i--)//cycle from tower top to bottom
        {
            var towerElement = _allTowerElements[i];

            var waveItemDelay = ((elementsLastIndex - i) * _gameSettings.VaweDelayStep);

            var maxWaveScaleModifier = i == elementsLastIndex ? 
                _gameSettings.LastItemMaxWaveScaleModifier : 
                _gameSettings.OtherItemMaxWaveScaleModifier;
            var finalScaleModifier = i == elementsLastIndex ? 
                _gameSettings.LastItemFinalScaleModifier : 
                _gameSettings.OtherItemFinalScaleModifier;

            Vector3 waveMaxScale = new Vector3(
                Mathf.Clamp(towerElement.transform.localScale.x * maxWaveScaleModifier, 0, _gameSettings.MaxTowerItemScale),
                towerElement.transform.localScale.y,
                Mathf.Clamp(towerElement.transform.localScale.z * maxWaveScaleModifier, 0, _gameSettings.MaxTowerItemScale));

            Vector3 finalScale = new Vector3(
                Mathf.Clamp(towerElement.transform.localScale.x * finalScaleModifier, 0, _gameSettings.MaxTowerItemScale),
                towerElement.transform.localScale.y,
                Mathf.Clamp(towerElement.transform.localScale.z * finalScaleModifier, 0, _gameSettings.MaxTowerItemScale));

            DoWave(waveItemDelay, towerElement.transform, waveMaxScale, finalScale);
        }
    }

    private void DoWave(float startDelay, Transform towerElement, Vector3 waveMaxScale, Vector3 finalScale)
    {
        var seq = DOTween.Sequence();
        
        seq.AppendInterval(startDelay);
        seq.Append(towerElement.DOScale(waveMaxScale, _gameSettings.VaweScaleDuration));
        seq.Append(towerElement.DOScale(finalScale, _gameSettings.VaweScaleDuration));

        seq.Play();
    }

    //lose
    private bool IsLoseCheck(float failSize)
    {
        var currScale = _currentTowerElement.transform.localScale;

        var isLose = currScale.x >= failSize;

        return isLose;
    }

    private void Lose()
    {
        _isGrowing = false;

        _playerInputService.SetInputActive(false);

        DestroyLoseElement();
    }

    private async UniTaskVoid DestroyLoseElement()
    {
        var failedElement = _currentTowerElement;
        _currentTowerElement = null;

        //change lose material to red
        failedElement.GetComponent<MeshRenderer>().material = _fruitItemSettings.LoseElementMaterial;

        await UniTask.Delay(1000);

        _fruitsPool.ReleaseItem(failedElement);
        _allTowerElements.Remove(failedElement);

        _gameService.GameOver();
    }
}
