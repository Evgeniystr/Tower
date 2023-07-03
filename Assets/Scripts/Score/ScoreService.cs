using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ScoreService
{
    public event Action<int, bool> OnScoreConterChange;

    private GameService _gameService;
    private TowerBuilderService _towerBuilderService;

    public int LastScore { private set; get; }
    public int HiScore { private set; get; }


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
        LastScore++;

        OnScoreConterChange?.Invoke(LastScore, isPerfect);
    }

    public void Reset()
    {
        LastScore = 0;

        OnScoreConterChange?.Invoke(LastScore, false);
    }

    private void PublishScore()
    {
        if (_gameService.IsAuthenticated)
        {
            Social.ReportScore(LastScore, Constants.TowerHeightLeaderboardID, (isSucces) => Debug.Log("GPGS ReportScore " + isSucces));//
        }

        GetLeaderbordData();//test
    }

    private void GetLeaderbordData()
    {
        PlayGamesPlatform.Instance.LoadScores(
            Constants.TowerHeightLeaderboardID,
            LeaderboardStart.PlayerCentered,
            10,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                var mStatus = "Leaderboard data valid: " + data.Valid;
                mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
            });
    }

    //на старті гри отримати лідербор щоб дізнатись поточний рекорд по поінтах
    //кешуєм поточний рекорд щоб візначати новий
    //при закінченні гри посилається реквесть на лідерборд
    //показується поточні бали і рекорд
    //якшо у нас новий персональний рекорд то яскраво пишем шо рекорд
    //виводимо в вікні результатів список результатів гравця та ближайших до нього місць
    //це імиена, позиція, поінти

    //get score
    //Social.LoadScores(allTimeTopScoreLeaderboardID, (scores) => Debug.LogError("ReportScore " + scores.Rank));
    //Social.ShowLeaderboardUI();


    //hi score ever
    //hi score day
    //hi score week

    //perfect move percent
    //hi perfect move percent ever/day/week

    //also need count minimal fruit count fo eachachivment
    //15% not bad, good
    //30% nice, good
    //50% perfect master
    //70%+ maestro! grend bellissimo!
    //90% godlike!!! unbelivable! incredible! immposible
}
