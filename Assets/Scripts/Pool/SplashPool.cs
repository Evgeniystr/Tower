using UnityEngine;

public class SplashPool : APool<SplashView>
{
    private SplashView _prefab;
    private Transform _parent;

    public SplashPool(SplashView prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    protected override SplashView CreateItem()
    {
        return GameObject.Instantiate(_prefab, _parent);
    }
}
