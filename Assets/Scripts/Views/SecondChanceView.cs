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

    [Inject]
    private ADSService _adService;
    [Inject]
    private GameService _gameService;
    [Inject]
    private TowerBuilderService _towerBuilderService;


    void Start()
    {
        _towerBuilderService.OnSecondChancePropoposal += ShowPopup;

        _yesButton.onClick.AddListener(() => 
        {
            _adService.ShowRewardedAd();
            HidePopup();
        });
        _noButton.onClick.AddListener(() =>
        {
            _gameService.GameOver();
            HidePopup();
        });

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
}
