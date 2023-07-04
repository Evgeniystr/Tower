using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public class ScoreService
{
    public event Action<long, bool> OnScoreConterChange;
    public event Action<LeaderboardScoreData> OnLeaderboardDataRecive;

    private GameService _gameService;
    private TowerBuilderService _towerBuilderService;

    public long ScoreCounter { private set; get; }

    private ScoreService (GameService gameService, TowerBuilderService towerBuilderService)
    {
        _gameService = gameService;
        _towerBuilderService = towerBuilderService;

        _gameService.OnGameStart += Reset;
        _gameService.OnGameOver += PublishScore;
        _towerBuilderService.OnTowerItemPlaced += AddScore;
    }


    private void AddScore(bool isPerfect)
    {
        ScoreCounter++;

        OnScoreConterChange?.Invoke(ScoreCounter, isPerfect);
    }

    public void Reset()
    {
        ScoreCounter = 0;

        OnScoreConterChange?.Invoke(ScoreCounter, false);
    }

    private void PublishScore()
    {
        if (_gameService.IsAuthenticated)
        {
            Social.ReportScore(ScoreCounter, Constants.TowerHeightLeaderboardID, (isSucces) => 
            {
                if (isSucces)
                {
                    Social.ShowLeaderboardUI();
                    GetLeaderbordData();
                }
                else
                    throw new Exception($"[ScoreService] GPGS ReportScore: {isSucces}");
            });
        }
        else
        {
#if UNITY_EDITOR
#elif PLATFORM_ANDROID
        Debug.LogError("[ScoreService] GPGS NOT Authenticated while runing on Android device");
#endif
        }
    }

    public void GetLeaderbordData()
    {
        PlayGamesPlatform.Instance.LoadScores(
            Constants.TowerHeightLeaderboardID,
            LeaderboardStart.PlayerCentered,
            10,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                OnLeaderboardDataRecive?.Invoke(data);
            },
            true);
    }
}
