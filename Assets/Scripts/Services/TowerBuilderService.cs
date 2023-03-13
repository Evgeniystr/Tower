using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DG.Tweening;
using System.Threading;
using System;

public class TowerBuilderService
{
    public event Action<bool> OnTowerItemPlaced;//bool - isPerfect
    
    private SplashService _splashService;
    private AudioService _audioService;
    private GameService _gameService;
    private PlayerInputService _playerInputService;
    private CameraService _cameraService;
    private GameSettings _gameSettings;
    private FruitItemSettings _fruitItemSettings;
    private Transform _towerBaseItem;

    private List<GameObject> _allTowerElements;
    private GameObject _currentTowerElement;
    private GameObject _previousTowerElement;

    private FruitsPool _fruitsPool;
    private CancellationTokenSource _scaleUpCancellationTokenSource;

    public TowerBuilderService(
        [Inject(Id = Constants.FruitPrefab)] GameObject fruitPrefab,
        [Inject(Id = Constants.TowerBaseItem)] Transform towerBaseItem,
        SplashService splashService,
        AudioService audioService, 
        GameService gameService, 
        PlayerInputService playerInputService,
        CameraService cameraService,
        FruitItemSettings fruitItemSettings,
        GameSettings gameSettings)
    {
        _fruitsPool = new FruitsPool(fruitPrefab);

        _splashService = splashService;
        _audioService = audioService;
        _gameService = gameService;
        _playerInputService = playerInputService;
        _cameraService = cameraService;

        _towerBaseItem = towerBaseItem;
        _fruitItemSettings = fruitItemSettings;
        _gameSettings = gameSettings;

        Initialize();
    }

    private void Initialize()
    {
        _scaleUpCancellationTokenSource = new CancellationTokenSource();

        _towerBaseItem.GetComponent<MeshRenderer>().material = _fruitItemSettings.GetRandomMaterial();

        _gameService.RestartEvent += Restart;
        _playerInputService.OnTapEvent += StartMakeNewFruit;
        _playerInputService.OnReleaseEvent += StopMakeNewFruit;
    }

    private void StartMakeNewFruit()
    {
        SetupCurrentElement();
        _cameraService.SetLookAt(_currentTowerElement);

        ScaleUpProcess();
    }

    private async UniTaskVoid ScaleUpProcess()
    {
        var token = _scaleUpCancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            var scaleUpValue = _gameSettings.scaleUpSpeed * Time.deltaTime;

            var newScale = new Vector3(
                _currentTowerElement.transform.localScale.x + scaleUpValue,
                _currentTowerElement.transform.localScale.y,
                _currentTowerElement.transform.localScale.z + scaleUpValue);

            _currentTowerElement.transform.localScale = newScale;

            await UniTask.WaitForFixedUpdate(token);

            if (!LoseCheck())
                Lose();
        }
    }

    void SetupCurrentElement()
    {
        _currentTowerElement = _fruitsPool.Get();

        _currentTowerElement.transform.localScale = new Vector3(0, _currentTowerElement.transform.localScale.y, 0);

        //set position
        float baseY;

        if (_previousTowerElement != null)
            baseY = _previousTowerElement.transform.position.y;
        else if (_towerBaseItem)
            baseY = _towerBaseItem.position.y;
        else
            baseY = 0;

        float newYpos = _currentTowerElement.GetComponent<Renderer>().bounds.size.y + baseY;
        _currentTowerElement.transform.position = new Vector3(0, newYpos, 0);

        //set material
        _currentTowerElement.GetComponent<MeshRenderer>().material = _fruitItemSettings.GetRandomMaterial();

        _currentTowerElement.SetActive(true);
    }

    void StopMakeNewFruit()
    {
        _scaleUpCancellationTokenSource.Cancel();

        if (PerfectMoveCheck())
            OnPerfectMove();
        else
            OnNormalMove();

        _previousTowerElement = _currentTowerElement;
        _currentTowerElement = null;
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
        _scaleUpCancellationTokenSource.Cancel();

        _playerInputService.SetInputActive(false);
        var failedElement = _currentTowerElement;
        _currentTowerElement = null;

        //change lose material to red
        //loseElementMaterial.SetTexture("_MainTex",
        //    failedElement.GetComponent<MeshRenderer>().material.mainTexture);
        failedElement.GetComponent<MeshRenderer>().material = _fruitItemSettings.LoseElementMaterial;

        DestroyLoseElement(failedElement);
    }

    private async UniTaskVoid DestroyLoseElement(GameObject failedElement)
    {
        await UniTask.Delay(2000);
        _cameraService.LoseCameraOwerviewMove(failedElement);
        await UniTask.Delay(100);
        failedElement.SetActive(false);
    }

    void Restart()
    {
        _previousTowerElement = null;
    }
}
