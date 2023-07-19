using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreService
{
    public event Action<long, bool> OnScoreConterChange;
    public event Action<LeaderboardScoreData> OnLeaderboardDataRecive;

    private GameService _gameService;
    private TowerBuilderService _towerBuilderService;

    public long ScoreCounter { private set; get; }
    public Dictionary<string, string> UserNames { private set; get; }//iserID UserName
    public string PlayerName { private set; get; }


    private ScoreService (GameService gameService, TowerBuilderService towerBuilderService)
    {
        _gameService = gameService;
        _towerBuilderService = towerBuilderService;

        _gameService.OnGameStart += Reset;
        _gameService.OnGameOver += PublishScore;
        _towerBuilderService.OnTowerItemPlaced += AddScore;

        UserNames = new Dictionary<string, string> ();
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
        Debug.Log($"[ScoreService] GPGS Authenticate status: {_gameService.IsAuthenticated}");

        if (_gameService.IsAuthenticated)
        {
            PlayGamesPlatform.Instance.ReportScore(ScoreCounter, Constants.TowerHeightLeaderboardID, (isSucces) => 
            {
                if (isSucces)
                    GetLeaderbordData();
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
                if (string.IsNullOrEmpty(PlayerName))
                {
                    Social.LoadUsers(new string[] { data.PlayerScore.userID }, (users) =>
                    {
                        PlayerName = users[0].userName;
                    });
                }

                GetAndCatchUsernames(data);
            },
            true);
    }

    private void GetAndCatchUsernames(LeaderboardScoreData data)
    {
        var userIDsToRequest = new List<string>();

        foreach (var scoreItem in data.Scores)
        {
            if(!UserNames.ContainsKey(scoreItem.userID))
                userIDsToRequest.Add(scoreItem.userID);
        }

        if(userIDsToRequest.Count > 0)
        {
            Social.LoadUsers(userIDsToRequest.ToArray(), (users) =>
            {
                foreach (var userProfile in users)
                    UserNames.Add(userProfile.id, userProfile.userName);

                OnLeaderboardDataRecive?.Invoke(data);
            });
        }
        else
        {
            OnLeaderboardDataRecive?.Invoke(data);
        }
    }
}
