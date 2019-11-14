using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashPoolItem : IpoolItem
{
    public GameObject obj { get; private set; }

    public SplashPoolItem(GameObject go)
    {
        obj = go;
    }
}
