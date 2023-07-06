using ModestTree;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitItemSettings", menuName = "GameSettings/FruitItemSettings")]
public class FruitItemSettings : ScriptableObject
{
    public GameObject FruitPrefab;
    public Material LoseElementMaterial;
    public Material[] FruitMaterials;

    private const int _matNameStripCont = 11; //" (Instance)"

    public Material GetRandomMaterial(string exceptionMatName = null)
    {
        var index = Random.Range(0, FruitMaterials.Length);

        if (!string.IsNullOrEmpty(exceptionMatName))
        {
            var exceptMatName = exceptionMatName.Substring(0, exceptionMatName.Length - _matNameStripCont);
            if (FruitMaterials[index].name == exceptMatName)
                index = index == FruitMaterials.Length - 1 ? 0 : index +1;
        }

        return FruitMaterials[index];
    }
}
