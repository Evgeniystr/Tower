using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [HideInInspector] public bool readyToRestart;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);

        //Set screen size for Standalone
        #if UNITY_STANDALONE
                Screen.SetResolution(540, 910, false);
                Screen.fullScreen = false;
        #endif
    }
}
