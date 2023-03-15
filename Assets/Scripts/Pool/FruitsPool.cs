using UnityEngine;

public class FruitsPool : APool<GameObject>
{
    private GameObject _prefab;
    private Transform _parent;


    public FruitsPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }


    protected override GameObject CreateItem()
    {
        var item = GameObject.Instantiate(_prefab, _parent);
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
