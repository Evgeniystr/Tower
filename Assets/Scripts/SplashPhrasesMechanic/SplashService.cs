using UnityEngine;
using Zenject;

public class SplashService
{
    private SplashPool _spalshPool;
    private Transform _splashParent;

    public SplashService(
        [Inject(Id = Constants.SplashParent)] Transform splashParent,
        SplashSettings splashSettings)
    {
        _splashParent = splashParent;
        _spalshPool = new SplashPool(_splashParent, splashSettings);
    }


    public void DoSplash()
    {
        var splashView = _spalshPool.Get();

        var splashPosition = DefineSplashPosition();

        splashView.SetAndPlay(splashPosition , () =>_spalshPool.ReleaseItem(splashView));
    }

    private Vector3 DefineSplashPosition()
    {
        var rect = _splashParent.GetComponent<RectTransform>().rect;
        var x = Random.Range(rect.min.x, rect.max.x);
        var y = Random.Range(rect.min.y, rect.max.y);
        var newPos = new Vector3(x, y);

        return newPos;
    }
}
