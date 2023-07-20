using DG.Tweening;
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
    private Button _noButton;
    [SerializeField]
    private Text _notReadyText;
    [SerializeField]
    private Color _notReadyColor;

    [Inject]
    private ADSService _adService;
    [Inject]
    private GameService _gameService;
    [Inject]
    private TowerBuilderService _towerBuilderService;


    private Sequence _fadeOut;
    private Color _notReadyTranspatentColor;

    void Start()
    {
        _towerBuilderService.OnSecondChancePropoposal += ShowPopup;

        _yesButton.onClick.AddListener(Contirm);
        _noButton.onClick.AddListener(Decline);

        _notReadyTranspatentColor = new Color(_notReadyColor.r, _notReadyColor.g, _notReadyColor.b, 0);
        _notReadyText.color = _notReadyTranspatentColor;

        HidePopup();
    }

    private void ShowPopup()
    {
        _viewport.SetActive(true);
    }

    private void HidePopup()
    {
        _viewport.SetActive(false);
    }

    private void Contirm()
    {
        if (_adService.RewardReadyCheck())
        {
            _adService.ShowRewardedAd();
            HidePopup();
        }
        else
        {
            if(_fadeOut != null && _fadeOut.IsPlaying())
                _fadeOut.Kill();

            _fadeOut = DOTween.Sequence();
            _fadeOut.Append(_notReadyText.DOColor(_notReadyColor, 0));
            _fadeOut.AppendInterval(1.5f);
            _fadeOut.Append(_notReadyText.DOColor(_notReadyTranspatentColor, 1.5f));

            _fadeOut.Play();
        }
    }

    private void Decline()
    {
        _gameService.GameOver();
        HidePopup();
    }
}
