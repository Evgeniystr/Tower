using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControler : MonoBehaviour
{
    public static InputControler Instance;

    [HideInInspector] public bool isInputEnabled;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.isInputEnabled = true;
        }
        else
            Destroy(gameObject);
    }
}
