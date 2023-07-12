using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HintPanelView : MonoBehaviour
{
    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private Button _button;

    [Inject]
    private GameService _gameService;

    void Start()
    {
        _button.onClick.AddListener(HideHint);
        _button.onClick.AddListener(_gameService.Initialize);

        ShowHint();
    }

    public void HideHint()
    {
        _button.onClick.RemoveAllListeners();

        _viewport.SetActive(false);
    }

    public void ShowHint()
    {
        _viewport.SetActive(true);
    }
}
