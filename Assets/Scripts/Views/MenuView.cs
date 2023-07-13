using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuView : MonoBehaviour
{
    [SerializeField]
    private GameObject _settingsPanel;
    [SerializeField]
    private Button _openSettingsButton;
    [SerializeField]
    private Button _closeSettingsButton;
    [SerializeField]
    private Button _closeSettingsFullscreenButton;
    [SerializeField]
    private Button _soundButton;
    [SerializeField]
    private Image _soundButtonImage;
    [SerializeField]
    private Slider _soundSlider;
    [SerializeField]
    private TMP_Text _bestScore;
    [SerializeField]
    private Button _resumeButton;
    [Space]
    [SerializeField]
    private Image _sliderBG;
    [SerializeField]
    private Image _sliderFill;
    [SerializeField]
    private Color _sliderFillActive;
    [SerializeField]
    private Color _sliderFillInactive;
    [SerializeField]
    private Color _sliderBGActive;
    [SerializeField]
    private Color _sliderBGInactive;
    [Space]
    [SerializeField]
    private Sprite _volumeSprite_0;
    [SerializeField]
    private Sprite _volumeSprite_1;
    [SerializeField]
    private Sprite _volumeSprite_2;
    [SerializeField]
    private Sprite _volumeSprite_3;

    [Inject]
    private AudioService _audioService;
    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private PlayerInputService _playerInputService;

    void Start()
    {
        Initialize();
        HidePanel();
    }

    private void Initialize()
    {
        _openSettingsButton.onClick.AddListener(ShowPanel);
        _closeSettingsButton.onClick.AddListener(HidePanel);
        //_closeSettingsFullscreenButton.onClick.AddListener(HidePanel);

        _soundButton.onClick.AddListener(_audioService.SwitchMute);
        _soundSlider.onValueChanged.AddListener(_audioService.SetMasterVolume);

        _audioService.OnVolumeChange += SetVolumeView;
    }

    private void ShowPanel()
    {
        _settingsPanel.SetActive(true);
        _openSettingsButton.gameObject.SetActive(false);
        _closeSettingsFullscreenButton.gameObject.SetActive(true);

        _playerInputService.SetInputActive(false);
        //_bestScore.text = _scoreService.platy
    }
    private void HidePanel()
    {
        _settingsPanel.SetActive(false);
        _openSettingsButton.gameObject.SetActive(true);
        _closeSettingsFullscreenButton.gameObject.SetActive(false);

        _playerInputService.SetInputActive(true);
    }

    private void SetVolumeView()
    {
        if (_audioService.IsMuted)
        {
            _soundButtonImage.sprite = _volumeSprite_0;

            _sliderBG.color = _sliderBGInactive;
            _sliderFill.color = _sliderFillInactive;
        }
        else
        {
            if(_audioService.Volume > 0.7)
                _soundButtonImage.sprite = _volumeSprite_3;
            else if (_audioService.Volume > 0.4)
                _soundButtonImage.sprite = _volumeSprite_2;
            else if (_audioService.Volume > 0)
                _soundButtonImage.sprite = _volumeSprite_1;
            else
                _soundButtonImage.sprite = _volumeSprite_0;

            _sliderBG.color = _sliderBGActive;
            _sliderFill.color = _sliderFillActive;
        }

        _soundSlider.interactable = !_audioService.IsMuted;
    }
}
