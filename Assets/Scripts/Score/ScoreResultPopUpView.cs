using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreResultPopUpView : MonoBehaviour
{
    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private CameraService _cameraService;
    [Inject]
    private TowerBuilderService _towerBuilderService;

    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private CanvasGroup _viewportCanvasGroup;
    [SerializeField]
    private TMP_Text _lastScore;
    [SerializeField]
    private TMP_Text _bestScore;
    [SerializeField]
    private Button _restartButton;


    private void Start()
    {
        _cameraService.OnLoseCamStop += ShowPopup;

        _restartButton.onClick.AddListener(Restart);

        _viewport.SetActive(false);
    }

    private void OnDestroy()
    {
        _cameraService.OnLoseCamStop -= ShowPopup;
        _restartButton.onClick.RemoveListener(Restart);
    }


    public void ShowPopup()
    {
        string scoreInfo;

        scoreInfo = $"Score {_scoreService.LastScore}";
        _lastScore.text = scoreInfo;

        scoreInfo = $"Best {_scoreService.HiScore}";
        _bestScore.text = scoreInfo;

        var seq = DOTween.Sequence();
        seq.AppendInterval(1);//add some time for builded tower lookup
        seq.AppendCallback(() => {
            _viewportCanvasGroup.alpha = 0;
            _viewport.SetActive(true);
            });
        seq.Append(DOTween.To(
            () => _viewportCanvasGroup.alpha, 
            (value) => _viewportCanvasGroup.alpha = value, 
            1, 0.6f).SetEase(Ease.OutExpo));
    }

    public void Restart()
    {
        _cameraService.NewGameCameraMove();
        _towerBuilderService.CleareTower();
        _viewport.SetActive(false);
    }
}
