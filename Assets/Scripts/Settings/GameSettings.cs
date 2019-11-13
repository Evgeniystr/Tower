using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "GameSettings/Gameplay")]
public class GameSettings : ScriptableObject
{
    public float heightElementRatio;
    public float heightStep;
    public int sturtupPoolSize;
    public int poolExpandStep;
    public float scaleUpSpeed;
    public float vaweScaleSpeed;
    public float vaweDelayStep;
}
