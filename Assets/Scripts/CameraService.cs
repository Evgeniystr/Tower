using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

public class CameraService
{
    public event Action OnLoseCamStop;

    private Transform _camTransform;
    private CameraSettings _cameraSettings;

    private Transform _startLoockAt;
    private Vector3 _cameraOffset;
    private Quaternion _defaultRotation;

    private GameService _gameService;
    private TowerBuilderService _towerBuilderService;

    private Sequence _camMovement;

    public CameraService(
        [Inject(Id = Constants.Camera)] Transform camTransform,
        [Inject(Id = Constants.TowerBaseItem)] Transform towerBaseItem,
        CameraSettings cameraSettings,
        GameService gameService,
        TowerBuilderService towerBuilderService)
    {
        _camTransform = camTransform;
        _startLoockAt = towerBaseItem;
        _cameraSettings = cameraSettings;

        _gameService = gameService;
        _towerBuilderService = towerBuilderService;

        _cameraOffset = _camTransform.position;
        _defaultRotation = _camTransform.rotation;

        _towerBuilderService.OnTowerItemPlaced += LookAtLastTowerItem;
        _gameService.OnGameOver += LoseCameraOwerviewMove;
    }


    public void RestartCamera()
    {
        SetLookAt(_startLoockAt.gameObject);
        _camTransform.rotation = _defaultRotation;
    }

    private void LookAtLastTowerItem(bool isPerfect)
    {
        var target = _towerBuilderService.GetLastTowerItem();
        SetLookAt(target);
    }

    public void SetLookAt(GameObject go)
    {
        CameraMoveTo(go.transform.position);
    }

   void CameraMoveTo(Vector3 position)
    {
        if (_camMovement != null && _camMovement.active)
            _camMovement.Kill();

        var destination = position + _cameraOffset;

        _camMovement = DOTween.Sequence();
        _camMovement.Append(_camTransform.DOMove(destination, _cameraSettings.cameraFolowDuration));
    }

    //failed element
    public void LoseCameraOwerviewMove()
    {
        var lastTowerItem = _towerBuilderService.GetLastTowerItem();

        var towerHalfHeight = lastTowerItem.transform.position.y / 2;
        var far = _cameraSettings.CameraCoofFailDistance * lastTowerItem.transform.position.y + _cameraSettings.CameraBaseFailDistance;

        var lookAtPos = new Vector3(0, towerHalfHeight, 0);
        var camMoveTo = new Vector3(0, towerHalfHeight, far)/* + _cameraOffset*/;

        var camLookAtDirection = lookAtPos - camMoveTo;
        var camRotate = Quaternion.LookRotation(camLookAtDirection, Vector3.up);

        var seq = DOTween.Sequence();

        seq.Append(_camTransform.DOLocalMove(camMoveTo, _cameraSettings.CamFailAnimDuration));
        seq.Join(_camTransform.DORotateQuaternion(camRotate, _cameraSettings.CamFailAnimDuration));
        seq.OnComplete(() => OnLoseCamStop?.Invoke());
        seq.Play();
    }
}
