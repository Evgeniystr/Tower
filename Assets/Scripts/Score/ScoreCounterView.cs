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

    void Start()
    {
        _scoreService.OnScoreConterChange += ShowScore;
    }

    private void OnDestroy()
    {
        _scoreService.OnScoreConterChange -= ShowScore;
    }

    private void ShowScore(int value, bool isPerfect)
    {
        _counter.text = value.ToString();

        if (isPerfect)
            _counter.transform.DOPunchScale(Vector3.one*1.5f, 0.5f);
    }
}
