using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;


    [HideInInspector] public Action RestartEvent;
    [HideInInspector] public Action GameOverEvent;



    void Awake()
    {
        ResolutionHelper();

        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }

    void ResolutionHelper()
    {
#if UNITY_STANDALONE
        Screen.fullScreen = false;
        Screen.SetResolution(540, 910, false);
#endif

    }

    public void RestartGame()
    {
        RestartEvent();
        InputControler.Instance.isInputEnabled = true;
    }

    public void GameOverResults()
    {
        GameOverEvent();
        InputControler.Instance.isInputEnabled = false;
    }
}
