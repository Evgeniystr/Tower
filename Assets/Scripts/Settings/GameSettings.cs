using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "GameSettings/Gameplay")]
public class GameSettings : ScriptableObject
{
    public float MaxTowerItemScale;
    public float ItemScaleUpSpeed;

    [Header("Wave settings")]
    public float VaweScaleDuration;
    public float VaweDelayStep;
    public float LastItemMaxWaveScaleModifier;
    public float LastItemFinalScaleModifier;
    public float OtherItemMaxWaveScaleModifier;
    public float OtherItemFinalScaleModifier;
}
