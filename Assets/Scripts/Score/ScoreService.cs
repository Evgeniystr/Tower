using System;

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

        _gameService.RestartEvent += Reset;
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
    }

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
