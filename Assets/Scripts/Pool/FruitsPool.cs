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
        var item = GameObject.Instantiate(_prefab);
        item.SetActive(false);
        return item;
    }

    public override GameObject Get()
    {
        var item = base.Get();
        item.SetActive(true);
        return item;
    }

    public override void ReleaseItem(GameObject item)
    {
        item.SetActive(false);
        base.ReleaseItem(item);
    }
}
