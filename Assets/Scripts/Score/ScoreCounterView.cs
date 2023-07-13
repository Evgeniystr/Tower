using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class ScoreCounterView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _counter;

    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private GameService _gameService;


    void Start()
    {
        _counter.enabled = false;

        _scoreService.OnScoreConterChange += ShowScore;

        _gameService.OnStartupInitialize += () => _counter.enabled = true;
    }

    private void OnDestroy()
    {
        _scoreService.OnScoreConterChange -= ShowScore;
    }

    private void ShowScore(long value, bool isPerfect)
    {
        _counter.text = value.ToString();

        if (isPerfect)
            _counter.transform.DOPunchScale(Vector3.one*1.5f, 0.5f);
    }
}
