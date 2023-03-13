using UnityEngine;

[CreateAssetMenu(fileName = "FruitItemSettings", menuName = "GameSettings/FruitItemSettings")]
public class FruitItemSettings : ScriptableObject
{
    public GameObject FruitPrefab;
    public Material LoseElementMaterial;
    public Material[] FruitMaterials;

    public Material GetRandomMaterial()
    {
        var index = Random.Range(0, FruitMaterials.Length);
        return FruitMaterials[index];
    }
}
