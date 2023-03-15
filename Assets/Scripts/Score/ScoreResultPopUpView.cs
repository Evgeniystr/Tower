using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;

public class ScoreResultPopUpView : MonoBehaviour
{
    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private GameService _gameService;

    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private TMP_Text _lastScore;
    [SerializeField]
    private TMP_Text _bestScore;
    [SerializeField]
    private Button _restartButton;


    private void Start()
    {
        _gameService.OnGameOver += SetAndShowScores;
        _viewport.SetActive(false);
        _restartButton.onClick.AddListener(Restart);
    }

    private void OnDestroy()
    {
        _gameService.OnGameOver += SetAndShowScores;
        _restartButton.onClick.RemoveListener(Restart);
    }


    public void SetAndShowScores()
    {
        string scoreInfo;

        scoreInfo = $"Score {_scoreService.LastScore}";
        _lastScore.text = scoreInfo;

        scoreInfo = $"Best {_scoreService.HiScore}";
        _bestScore.text = scoreInfo;

        _viewport.SetActive(true);
    }

    public void Restart()
    {
        _gameService.StartGame();
        _viewport.SetActive(false);
    }
}
