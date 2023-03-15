using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SplashSettings", menuName = "GameSettings/SplashSettings")]
public class SplashSettings : ScriptableObject
{
    public SplashView Prefab;
    public string[] Phrases;
    public ColorPair[] ColorPalette;
    public AnimationClip[] Animations;
    public Sprite[] Backgrounds;


    public string GetRandomPhrase()
    {
        var index = UnityEngine.Random.Range(0, Phrases.Length);
        return Phrases[index];
    }

    public ColorPair GetRandomColorsPreset()
    {
        var index = UnityEngine.Random.Range(0, ColorPalette.Length);
        return ColorPalette[index];
    }

    public AnimationClip GetRandomAnimation()
    {
        var index = UnityEngine.Random.Range(0, Animations.Length);
        return Animations[index];
    }

    public Sprite GetRandomBackground()
    {
        var index = UnityEngine.Random.Range(0, Backgrounds.Length);
        return Backgrounds[index];
    }
}

[Serializable]
public class ColorPair
{
    public Color32 bgColor;
    public Color32 textColor;
}
