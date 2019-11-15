using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "GameSettings/Gameplay")]
public class GameSettings : ScriptableObject
{
    public float maxScale;
    public float scaleUpSpeed;
    public float vaweScaleSpeed;
    public float vaweDelayStep;
}
