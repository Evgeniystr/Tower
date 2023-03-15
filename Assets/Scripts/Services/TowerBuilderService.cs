using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
using System.Threading;
using System;
using System.Linq;

public class TowerBuilderService
{
    public event Action<bool> OnTowerItemPlaced;//bool - isPerfect

    public GameObject LastTowerItem => _allTowerElements.Last();

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

    private bool _isGrowing;

    public TowerBuilderService(
        [Inject(Id = Constants.FruitPrefab)] GameObject fruitPrefab,
        [Inject(Id = Constants.TowerBaseItem)] Transform towerBaseItem,
        SplashService splashService,
        AudioService audioService, 
        GameService gameService, 
        PlayerInputService playerInputService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab);

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
    }

    private void OnGameStart()
    {
        _playerInputService.OnTapEvent += StartMakeNewFruit;
        _playerInputService.OnReleaseEvent += StopMakeNewFruit;

        foreach (var item in _allTowerElements)
            _fruitsPool.ReleaseItem(item);

        _previousTowerElement = null;
    }

    private void OnGameEnd()
    {
        _playerInputService.OnTapEvent -= StartMakeNewFruit;
        _playerInputService.OnReleaseEvent -= StopMakeNewFruit;
    }

    private void StartMakeNewFruit()
    {
        SetupCurrentElement();

        ScaleUpProcess();
    }

    private async UniTaskVoid ScaleUpProcess()
    {
        _isGrowing = true;

        while (_isGrowing)
        {
            var scaleUpValue = _gameSettings.scaleUpSpeed * Time.fixedDeltaTime;

            var newScale = new Vector3(
                _currentTowerElement.transform.localScale.x + scaleUpValue,
                _currentTowerElement.transform.localScale.y,
                _currentTowerElement.transform.localScale.z + scaleUpValue);

            _currentTowerElement.transform.localScale = newScale;

            await UniTask.WaitForFixedUpdate();

            if (LoseCheck())
                Lose();
        }
    }

    void SetupCurrentElement()
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

    void StopMakeNewFruit()
    {
        _isGrowing = false;

        if (PerfectMoveCheck())
            OnPerfectMove();
        else
            OnNormalMove();

        _previousTowerElement = _currentTowerElement;
    }

    void OnNormalMove()
    {
        OnTowerItemPlaced?.Invoke(false);

        _audioService.DoSplatterSound();
    }

    void OnPerfectMove()
    {
        PerfectMovePerform();

        _splashService.DoSplash();

        OnTowerItemPlaced?.Invoke(true);

        _audioService.DoPerfectSplatterSound();
    }


    bool PerfectMoveCheck()
    {
        if (_previousTowerElement == null)
            return false;

        var minPerfScale = _previousTowerElement.transform.localScale * 0.95f;

        if (_currentTowerElement.transform.localScale.x >= minPerfScale.x && _currentTowerElement.transform.localScale.z >= minPerfScale.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PerfectMovePerform()
    {
        float lastItemFinalScaleModifier = 0.2f;
        float lastItemMaxWaveScaleModifier = 0.4f;
        float otherItemFinalScaleModifier = 0.3f;
        float otherItemMaxWaveScaleModifier = otherItemFinalScaleModifier * 0.8f;

        var elementsLastIndex = _allTowerElements.Count - 1;

        for (int i = elementsLastIndex; i >= 0; i--)//cycle from tower top to bottom
        {
            var towerElement = _allTowerElements[i];

            var waveDelayMS = (int)((elementsLastIndex - i) * _gameSettings.vaweDelayStep * 1000);

            var maxWaveScaleModifier = i == _allTowerElements.Count ? lastItemMaxWaveScaleModifier : otherItemMaxWaveScaleModifier;
            var finalScaleModifier = i == _allTowerElements.Count ? lastItemFinalScaleModifier : otherItemFinalScaleModifier;


            Vector3 waveMaxScale = new Vector3(
                Mathf.Clamp(towerElement.transform.localScale.x + maxWaveScaleModifier, 0, _gameSettings.maxScale),
                towerElement.transform.localScale.y,
                Mathf.Clamp(towerElement.transform.localScale.x + maxWaveScaleModifier, 0, _gameSettings.maxScale));

            Vector3 finalScale = new Vector3(
                Mathf.Clamp(towerElement.transform.localScale.x + finalScaleModifier, 0, _gameSettings.maxScale),
                towerElement.transform.localScale.y,
                Mathf.Clamp(towerElement.transform.localScale.x + finalScaleModifier, 0, _gameSettings.maxScale));

            DoWave(waveDelayMS, towerElement.transform, waveMaxScale, finalScale);
        }
    }

    private void DoWave(int startDelay, Transform towerElement, Vector3 waveMaxScale, Vector3 finalScale)
    {
        var seq = DOTween.Sequence();

        seq.AppendInterval(startDelay);
        seq.Append(towerElement.DOScale(waveMaxScale, _gameSettings.vaweScaleSpeed));
        seq.Append(towerElement.DOScale(finalScale, _gameSettings.vaweScaleSpeed));

        seq.Play();
    }

    //lose
    private bool LoseCheck()
    {
        var currScale = _currentTowerElement.transform.localScale;

        Vector3 loseScale = _previousTowerElement == null ? 
            _towerBaseItem.localScale * 1.1f :
            _previousTowerElement.transform.localScale * 1.1f;

        //false - lose, true - go on
        var isLose = currScale.x >= loseScale.x || currScale.z >= loseScale.z;

        return isLose;
    }

    private void Lose()
    {
        Debug.LogError("Lose");//
        _isGrowing = false;

        _playerInputService.SetInputActive(false);
        var failedElement = _currentTowerElement;
        _currentTowerElement = null;

        //change lose material to red
        //loseElementMaterial.SetTexture("_MainTex",
        //    failedElement.GetComponent<MeshRenderer>().material.mainTexture);
        failedElement.GetComponent<MeshRenderer>().material = _fruitItemSettings.LoseElementMaterial;

        _gameService.GameOver();
        _fruitsPool.ReleaseItem(failedElement);
    }

    private async UniTaskVoid DestroyLoseElement(GameObject failedElement)
    {
        await UniTask.Delay(2000);
        //_cameraService.LoseCameraOwerviewMove(failedElement);
        //await UniTask.Delay(100);
        _fruitsPool.ReleaseItem(failedElement);
    }
}
