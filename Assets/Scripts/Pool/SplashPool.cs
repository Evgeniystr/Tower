using UnityEngine;

public class SplashPool : APool<SplashView>
{
    private Transform _parent;
    private SplashSettings _splashSettings;

    public SplashPool(Transform parent, SplashSettings splashSettings)
    {
        _parent = parent;
        _splashSettings = splashSettings;
    }

    protected override SplashView CreateItem()
    {
        var item = GameObject.Instantiate(_splashSettings.Prefab, _parent);
        item.Initialize(_splashSettings);
        item.gameObject.SetActive(false);
        return item;
    }

    public override SplashView Get()
    {
        var item = base.Get();
        item.gameObject.SetActive(true);
        return item;
    }

    public override void ReleaseItem(SplashView item)
    {
        item.gameObject.SetActive(false);
        base.ReleaseItem(item);
    }
}
