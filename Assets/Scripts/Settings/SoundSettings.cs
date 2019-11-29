using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundSettings", menuName = "GameSettings/SoundSettings")]

public class SoundSettings : ScriptableObject
{
    public AudioClip mainTheme;
    public AudioClip splatter;


    public float splatterMinPitch;
    public float splatterMaxPitch;
    
    public float perfectMoveMinPitch;
    public float perfectMoveMaxPitch;
}
