using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "GameSettings/Gameplay")]
public class GameSettings : ScriptableObject
{
    public float MaxTowerItemScale;
    public float ItemScaleUpSpeed;
    public float _perfectMoveOffset;
    public int PerfectMoveStreakRequired;
    public int WaveLength;

    [Header("Wave settings")]
    public float VaweScaleDuration;
    public float VaweDelayStep;
    public float LastItemMaxWaveScaleModifier;
    public float LastItemFinalScaleModifier;
    public float OtherItemMaxWaveScaleModifier;
    public float OtherItemFinalScaleModifier;

    [Header("Wave settings")]
    public float BouncePower;
    public float BounceDuration;
}
