using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class ScorePopUp : MonoBehaviour
{
    [SerializeField] ScoreHandler scoreHandler;

    [SerializeField] TextMeshProUGUI lastScore;
    [SerializeField] TextMeshProUGUI bestScore;

    private void Start()
    {
        GameManager.Instance.GameOverEvent += SetAndShowScores;
        gameObject.SetActive(false);
    }

    public void SetAndShowScores()
    {
        string scoreInfo;

        scoreInfo = "Score " + scoreHandler.lastScore;
        this.lastScore.text = scoreInfo;

        scoreInfo = "Best " + scoreHandler.hiScore;
        this.bestScore.text = scoreInfo;

        gameObject.SetActive(true);
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
        gameObject.SetActive(false);
    }
}
