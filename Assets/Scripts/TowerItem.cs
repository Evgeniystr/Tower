using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class TowerItem : MonoBehaviour
{
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private MeshFilter _meshFilter;

    public Transform Transform { get { return _transform; } }
    public MeshFilter MeshFilter { get { return _meshFilter; } }
    public float Size { get; private set; } = 1;
    public string CurrentMatName { get; private set; }

    private TowerBuilderService _towerBuilderService;
    private FruitItemSettings _fruitItemSettings;
    private GameSettings _gameSettings;

    private float _dafaultYscale;
    private bool _isGrowing;
    private float _failSize;

    public void Initialize(TowerBuilderService towerBuilderService, FruitItemSettings fruitItemSettings, GameSettings gameSettings)
    {
        _towerBuilderService = towerBuilderService;
        _fruitItemSettings = fruitItemSettings;
        _gameSettings = gameSettings;

        _dafaultYscale = _transform.localScale.y;
    }


    public void Setup(TowerItem previousItem)
    {
        Size = 0;

        _transform.localScale = new Vector3(Size, _dafaultYscale, Size);

        //set position
        float baseYpos = previousItem.Transform.position.y;

        float newYpos = _meshRenderer.bounds.size.y + baseYpos;
        _transform.position = new Vector3(Size, newYpos, Size);

        //set fail size
        _failSize = previousItem.Size;

        //set material
        SetRandomMaterial(previousItem.CurrentMatName);
    }

    public void SetupAsBaseItem()
    {
        Size = 1;

        _transform.localScale = new Vector3(Size, _dafaultYscale, Size);
        _transform.position = Vector3.zero;
    }

    public float GetPrefectMoveTriggerValue()
    {
        var perfectMoveOffset = Size * _gameSettings.PerfectMoveSizeCoef;

        var perfectMoveTriggerValue = Size - perfectMoveOffset;

        return perfectMoveTriggerValue;
    }

    public void SetRandomMaterial(string previousMatName)
    {
        _meshRenderer.material = _fruitItemSettings.GetRandomMaterial(previousMatName);
        CurrentMatName = _meshRenderer.material.name;
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public async void StartGrowing()
    {
        _isGrowing = true;

        while (_isGrowing)
        {
            var growingValue = _gameSettings.ItemScaleUpSpeed * Time.fixedDeltaTime;

            Size += growingValue;

            _transform.localScale = new Vector3(Size, _dafaultYscale, Size);

            if (Size >= _failSize)
                _towerBuilderService.Lose();

            await UniTask.WaitForFixedUpdate();
        }
    }

    public void StopGrowing()
    {
        if (!_isGrowing)
            return;

        _isGrowing = false;
    }

    public void DoWave(float startDelay, bool isTopItem, float powerScale = 1)
    {
        var maxWaveScaleModifier = isTopItem ?
            _gameSettings.LastItemMaxWaveScaleModifier :
            _gameSettings.OtherItemMaxWaveScaleModifier;
        var finalScaleModifier = isTopItem ?
            _gameSettings.LastItemFinalScaleModifier :
            _gameSettings.OtherItemFinalScaleModifier;

        Size += Size * (finalScaleModifier * powerScale);
        var maxWaveItemSize = Size + Size * (maxWaveScaleModifier * powerScale);


        Vector3 waveMaxScale = new Vector3(
            Mathf.Clamp(maxWaveItemSize, 0, _gameSettings.MaxTowerItemScale),
            _dafaultYscale,
            Mathf.Clamp(maxWaveItemSize, 0, _gameSettings.MaxTowerItemScale));

        Vector3 finalScale = new Vector3(
            Mathf.Clamp(Size, 0, _gameSettings.MaxTowerItemScale),
            _dafaultYscale,
            Mathf.Clamp(Size, 0, _gameSettings.MaxTowerItemScale));


        var seq = DOTween.Sequence();

        seq.AppendInterval(startDelay);
        seq.Append(_transform.DOScale(waveMaxScale, _gameSettings.VaweScaleDuration));
        seq.Append(_transform.DOScale(finalScale, _gameSettings.VaweScaleDuration));

        seq.Play();
    }

    public void SetLoseView()
    {
        _meshRenderer.material = _fruitItemSettings.LoseElementMaterial;
    }

    public void FadeSizeToZero(float duration, Action callback)
    {
        Size = 0;

        _transform.
            DOScale(new Vector3(Size, _dafaultYscale, Size), duration).
            OnComplete(() => callback?.Invoke());
    }
}