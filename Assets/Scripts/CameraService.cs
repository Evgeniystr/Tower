using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

public class CameraService
{
    public event Action OnLoseCamStop;

    //[Inject(Id = Constants.Camera)]
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

        _cameraOffset = _camTransform.position - _startLoockAt.position;
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
        var target = _towerBuilderService.LastTowerItem;
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

        _camMovement = DOTween.Sequence();

        _camMovement.Append(_camTransform.DOMove(position, _cameraSettings.cameraFolowDuration));
    }

    //failed element
    public void LoseCameraOwerviewMove()
    {
        var lastTowerItem = _towerBuilderService.LastTowerItem;

        var towerHalfHeight = lastTowerItem.transform.position.y / 2;
        var far = _cameraSettings.CameraCoofFailDistance * towerHalfHeight + _cameraSettings.CameraBaseFailDistance;

        Vector3 camMoveTo = new Vector3(Camera.main.transform.position.x,
                                        towerHalfHeight,
                                        far);

        Vector3 camLookAtPos = new Vector3(lastTowerItem.transform.position.x,
                                          towerHalfHeight,
                                          lastTowerItem.transform.position.z);

        Quaternion camRotate = Quaternion.LookRotation(camLookAtPos, Vector3.up);

        var seq = DOTween.Sequence();

        seq.Append(_camTransform.DOMove(camMoveTo, _cameraSettings.CamFailAnimDuration));
        seq.Join(_camTransform.DORotateQuaternion(camRotate, _cameraSettings.CamFailAnimDuration));
        seq.OnComplete(() => OnLoseCamStop?.Invoke());
        seq.Play();
    }
}
