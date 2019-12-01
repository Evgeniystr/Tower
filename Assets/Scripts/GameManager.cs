using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Set screen size for Standalone
#if UNITY_STANDALONE
                Screen.SetResolution(540, 910, false);
                Screen.fullScreen = false;
#endif


public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance;


    [HideInInspector] public Action RestartEvent;
    [HideInInspector] public Action GameOverEvent;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
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
