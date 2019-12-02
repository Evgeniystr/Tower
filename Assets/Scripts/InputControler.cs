using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputControler : MonoBehaviour
{
    public static InputControler Instance;

    [HideInInspector] public bool isInputEnabled;
    [HideInInspector] public Action onTapEvent;
    [HideInInspector] public Action onReleaseEvent;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.isInputEnabled = true;
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isInputEnabled)
        {
            if(Input.GetMouseButtonDown(0))
                onTapEvent();
            
            if(Input.GetMouseButtonUp(0))
                onReleaseEvent();
        }
    }
}
