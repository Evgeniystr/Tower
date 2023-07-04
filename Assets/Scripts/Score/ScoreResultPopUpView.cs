using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;
using DG.Tweening;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;

public class ScoreResultPopUpView : MonoBehaviour
{
    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private CameraService _cameraService;
    [Inject]
    private TowerBuilderService _towerBuilderService;
    [Inject]
    private GameService _gameService;

    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private CanvasGroup _viewportCanvasGroup;
    [SerializeField]
    private TMP_Text _lastScore;
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private GameObject _privateRecordGO;

    [SerializeField]
    private Transform _leaderbordListRoot;
    [SerializeField]
    private LeaderboardEntryView _leadenboardEntriePrefab;

    private LeadenboardEntriesPool _leadenboardEntriesPool;
    private List<LeaderboardEntryView> _spawnedScoreItems;

    private void Start()
    {
        _leadenboardEntriesPool = new LeadenboardEntriesPool(_leadenboardEntriePrefab, _leaderbordListRoot);
        _spawnedScoreItems = new List<LeaderboardEntryView>();

        _cameraService.OnLoseCamStop += ShowPopup;
        _scoreService.OnLeaderboardDataRecive += ShowSocialScores;

        _restartButton.onClick.AddListener(Restart);

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

    public void ShowSocialScores(LeaderboardScoreData leaderboardData)
    {
        Debug.Log($"LeaderboardScoreData STATUS {leaderboardData.Status}");//

        if (!leaderboardData.Valid)
            throw new Exception($"[ScoreResultPopUpView] Leaderboard recived status: {leaderboardData.Status}");

        Debug.Log($"LeaderboardScoreData STATUS {leaderboardData.Status}");//
        _privateRecordGO.SetActive(_scoreService.ScoreCounter == leaderboardData.PlayerScore.value);
        Debug.Log($"LeaderboardScoreData ScoreCounter {_scoreService.ScoreCounter}");//
        Debug.Log($"LeaderboardScoreData PlayerScore {leaderboardData.PlayerScore.value}");//
        Debug.Log($"LeaderboardScoreData leaderboard entries count {leaderboardData.Scores.Length}");//
        foreach (var scoreItem in leaderboardData.Scores)
        {
            Debug.Log($"LeaderboardScoreData rank value userid {scoreItem.rank} {scoreItem.value} {scoreItem.userID}");//
            Debug.Log("-----");//

            var itemView = _leadenboardEntriesPool.Get();
            itemView.Setup(scoreItem.rank, scoreItem.value, scoreItem.userID);
            _spawnedScoreItems.Add(itemView);
        }
    }

    public void Restart()
    {
        _cameraService.NewGameCameraMove();
        _gameService.StartGame();
        _towerBuilderService.CleareTower();
        _viewport.SetActive(false);

        foreach (var spawnedItem in _spawnedScoreItems)
            _leadenboardEntriesPool.ReleaseItem(spawnedItem);
        _spawnedScoreItems.Clear();
    }
}
