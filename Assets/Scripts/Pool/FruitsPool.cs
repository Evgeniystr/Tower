using UnityEngine;

public class FruitsPool : APool<TowerItem>
{
    private TowerItem _prefab;
    private Transform _parent;
    private TowerBuilderService _towerBuilderService;
    private FruitItemSettings _fruitItemSettings;
    private GameSettings _gameSettings;

    public FruitsPool(
        TowerItem prefab, 
        Transform parent, 
        TowerBuilderService towerBuilderService, 
        FruitItemSettings fruitItemSettings, 
        GameSettings gameSettings)
    {
        _prefab = prefab;
        _parent = parent;
        _towerBuilderService = towerBuilderService;
        _fruitItemSettings = fruitItemSettings;
        _gameSettings = gameSettings;
    }


    protected override TowerItem CreateItem()
    {
        var item = GameObject.Instantiate(_prefab, _parent);
        item.Initialize(_towerBuilderService, _fruitItemSettings, _gameSettings);
        item.SetActive(false);
        return item;
    }

    public override TowerItem Get()
    {
        var item = base.Get();
        item.SetActive(true);
        return item;
    }

    public override void ReleaseItem(TowerItem item)
    {
        item.SetActive(false);
        base.ReleaseItem(item);
    }
}
