using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class ScoreCounterView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _counter;

    [Inject]
    private ScoreService _scoreService;
    [Inject]
    private GameService _gameService;

    private Tween _perfectMoveAnim;

    void Start()
    {
        _counter.enabled = false;

        _scoreService.OnScoreConterChange += ShowScore;

        _gameService.OnGameStart += () => _counter.enabled = true;
    }

    private void OnDestroy()
    {
        _scoreService.OnScoreConterChange -= ShowScore;
    }

    private void ShowScore(long value, bool isPerfect)
    {
        if (_perfectMoveAnim != null && _perfectMoveAnim.IsPlaying())
        {
            _perfectMoveAnim.Kill();
            _counter.transform.localScale = Vector3.one;
        }

        _counter.text = value.ToString();

        if (isPerfect)
            _perfectMoveAnim = _counter.transform.DOPunchScale(Vector3.one*1.2f, 1f, 8, 0.5f);
    }
}
