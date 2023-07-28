using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;
using DG.Tweening;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class ScoreResultPopUpView : MonoBehaviour
{
    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private CameraService _cameraService;
    [Inject]
    private GameService _gameService;

    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private CanvasGroup _viewportCanvasGroup;
    [SerializeField]
    private TMP_Text _lastScore;
    [SerializeField]
    private TMP_Text _bestScore;
    [SerializeField]
    private TMP_Text _waitText;
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _showLeaderboardButton;
    [SerializeField]
    private Button _hideLeaderboardButton;
    [SerializeField]
    private GameObject _privateRecordGO;
    [SerializeField]
    private GameObject _resultPanelGO;
    [SerializeField]
    private GameObject _laederboardPanelGO;
    [SerializeField]
    private GameObject _waitPanelGO;

    [SerializeField]
    private Transform _leaderbordListRoot;
    [SerializeField]
    private LeaderboardEntryView _leadenboardEntriePrefab;

    private LeadenboardEntriesPool _leadenboardEntriesPool;
    private List<LeaderboardEntryView> _spawnedScoreItems;

    private const string _waitTextValue = "wait";
    private const int _maxDotsCounter = 5;
    private bool _isScoresRecived;

    private void Start()
    {
        _leadenboardEntriesPool = new LeadenboardEntriesPool(_leadenboardEntriePrefab, _leaderbordListRoot);
        _spawnedScoreItems = new List<LeaderboardEntryView>();

        _cameraService.OnLoseCamStop += ShowPopup;
        _scoreService.OnLeaderboardDataRecive += ShowSocialScores;

        _restartButton.onClick.AddListener(Restart);
        _showLeaderboardButton.onClick.AddListener(ShowLeaderboardPanel);
        _hideLeaderboardButton.onClick.AddListener(HideLeaderboardPanel);

        _viewport.SetActive(false);
        StartupLeaderbordCleanup();
    }

    private void OnDestroy()
    {
        _cameraService.OnLoseCamStop -= ShowPopup;
        _scoreService.OnLeaderboardDataRecive -= ShowSocialScores;
        _restartButton.onClick.RemoveListener(Restart);
    }

    private void StartupLeaderbordCleanup()
    {
        for (int i = 0; i < _leaderbordListRoot.childCount; i++)
            Destroy(_leaderbordListRoot.GetChild(i).gameObject);
    }

    public void ShowPopup()
    {
        _lastScore.text = $"Score {_scoreService.ScoreCounter}";
        _bestScore.text = $"best {_scoreService.BestScore}";

        _privateRecordGO.SetActive(_scoreService.ScoreCounter == _scoreService.BestScore);
        _showLeaderboardButton.gameObject.SetActive(_gameService.IsAuthenticated);

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

        HideLeaderboardPanel();
    }

    private async void ShowLeaderboardPanel()
    {
        AnaliticsTool.LogLeaderboardOpen();

        _scoreService.RequestLeaderbordData(10, LeaderboardTimeSpan.AllTime);

        _resultPanelGO.SetActive(false);
        _laederboardPanelGO.SetActive(_isScoresRecived);
        _waitPanelGO.SetActive(!_isScoresRecived);

        var dotsCounter = 0;
        while (!_isScoresRecived)
        {
            await UniTask.Delay(300);

            if (dotsCounter <= _maxDotsCounter)
            {
                _waitText.text = _waitText.text + ".";
                dotsCounter++;
            }
            else
            {
                dotsCounter = 0;
                _waitText.text = _waitTextValue;
            }
        }
    }
    private void HideLeaderboardPanel()
    {
        _resultPanelGO.SetActive(true);
        _laederboardPanelGO.SetActive(false);
    }

    public void ShowSocialScores(LeaderboardScoreData leaderboardData)
    {
        if (!leaderboardData.Valid)
            throw new Exception($"[ScoreResultPopUpView] Leaderboard recived status: {leaderboardData.Status}");

        _isScoresRecived = true;
        _laederboardPanelGO.SetActive(true);
        _waitPanelGO.SetActive(false);

        foreach (var scoreItem in leaderboardData.Scores)
        {
            var itemView = _leadenboardEntriesPool.Get();
            var userName = _scoreService.UserNames[scoreItem.userID];
            var currentUsrScoreEntry = _scoreService.PlayerName == userName;
            itemView.Setup(scoreItem.rank, scoreItem.value, userName, currentUsrScoreEntry);
            _spawnedScoreItems.Add(itemView);
        }
    }

    public void Restart()
    {
        _isScoresRecived = false;

        _cameraService.NewGameCameraMove();
        _gameService.StartGame();
        _viewport.SetActive(false);

        foreach (var spawnedItem in _spawnedScoreItems)
            _leadenboardEntriesPool.ReleaseItem(spawnedItem);
        _spawnedScoreItems.Clear();
    }
}
