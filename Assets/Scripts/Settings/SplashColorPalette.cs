using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewSplashColorPalette", menuName = "GameSettings/SplashColorPalette")]
public class SplashColorPalette : ScriptableObject
{
    public ColorPair[] ColorPalette;
}

[Serializable]
public class ColorPair
{
    public Color32 bgColor;
    public Color32 textColor;
}
