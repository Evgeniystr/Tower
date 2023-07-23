using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SecondChanceView : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private TMP_Text _continueText;

    [Inject]
    private ADSService _adService;
    [Inject]
    private GameService _gameService;
    [Inject]
    private TowerBuilderService _towerBuilderService;

    private Sequence _popupSequence;
    private float _chanceTimerDuration = 4f;

    private Tween _continurTextPolsing;



    void Start()
    {
        _towerBuilderService.OnSecondChancePropoposal += ShowPopup;

        _yesButton.onClick.AddListener(Contirm);

        HidePopup();
    }

    private void ShowPopup()
    {
        _progressBar.fillAmount = 0;
        _viewport.SetActive(true);

        _popupSequence = DOTween.Sequence();
        _popupSequence.Append(_progressBar.DOFillAmount(1, _chanceTimerDuration));
        _popupSequence.AppendCallback(() => Decline());

        _continurTextPolsing = _continueText.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
    }

    private void HidePopup()
    {
        _viewport.SetActive(false);
    }

    private void Contirm()
    {
        _popupSequence.Kill();
        _continurTextPolsing.Kill();
        _adService.ShowRewardedAd();
        HidePopup();
    }

    private void Decline()
    {
        _popupSequence.Kill();
        _continurTextPolsing.Kill();
        _gameService.GameOver();
        HidePopup();
    }
}
