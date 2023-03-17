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
    private Vector3 _camDafaultPosition;

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

        _camDafaultPosition = _camTransform.position;

        _towerBuilderService.OnTowerItemPlaced += LookAtLastTowerItem;
        _gameService.OnGameOver += LoseCameraOwerviewMove;
    }


    public void NewGameCameraMove()
    {
        var camMoveTo = _startLoockAt.position + _camDafaultPosition;

        var camLookAtDirection = _startLoockAt.position - camMoveTo;
        var camRotate = Quaternion.LookRotation(camLookAtDirection, Vector3.up);

        var seq = DOTween.Sequence();

        seq.SetEase(Ease.InOutQuint);
        seq.Append(_camTransform.DOLocalMove(camMoveTo, _cameraSettings.CamRestartAnimDuration));
        seq.Join(_camTransform.DORotateQuaternion(camRotate, _cameraSettings.CamRestartAnimDuration));
        seq.OnComplete(() => _gameService.StartGame());
        seq.Play();
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

        var destination = position + _camDafaultPosition;

        _camMovement = DOTween.Sequence();
        _camMovement.Append(_camTransform.DOMove(destination, _cameraSettings.cameraFolowDuration));
        _camMovement.Play();
    }

    //failed element
    public void LoseCameraOwerviewMove()
    {
        var lastTowerItem = _towerBuilderService.GetLastTowerItem();

        var towerHalfHeight = lastTowerItem.transform.position.y / 2;
        var far = _cameraSettings.CameraCoofFailDistance * lastTowerItem.transform.position.y + _cameraSettings.CameraBaseFailDistance;

        var lookAtPos = new Vector3(0, towerHalfHeight, 0);
        var camMoveTo = new Vector3(0, towerHalfHeight, far);

        var camLookAtDirection = lookAtPos - camMoveTo;
        var camRotate = Quaternion.LookRotation(camLookAtDirection, Vector3.up);
       
        var seq = DOTween.Sequence();

        seq.SetEase(Ease.InOutQuint);
        seq.Append(_camTransform.DOLocalMove(camMoveTo, _cameraSettings.CamFailAnimDuration));
        seq.Join(_camTransform.DORotateQuaternion(camRotate, _cameraSettings.CamFailAnimDuration));
        seq.OnComplete(() => OnLoseCamStop?.Invoke());
        seq.Play();
    }
}
