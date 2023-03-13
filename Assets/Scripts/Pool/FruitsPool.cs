using UnityEngine;

public class FruitsPool : APool<GameObject>
{
    private GameObject _prefab;


    public FruitsPool(GameObject prefab)
    {
        _prefab = prefab;
    }


    protected override GameObject CreateItem()
    {
        return GameObject.Instantiate(_prefab);
    }
}
