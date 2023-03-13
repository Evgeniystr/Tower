using UnityEngine;
using Zenject;

public class SplashService
{
    private Transform _splashParent;
    private SplashPool _spalshPool;
    private AnimationClip[] _animations;
    private Sprite[] _backgrounds;
    private SplashColorPalette _splashColorPalette;

    private string[] phrases = 
        { "Yummie", "Splash", "Mmmm", "Yaay!", "Juicy!", "Sabroso", "Om nom nom", "Oh Lord!", "Perfect!", "So GOOD!!", "Nice!"};


    public SplashService(
        [Inject(Id = Constants.SplashParent)] Transform _splashParent,
        SplashView splashPrefab)
    {
        _splashParent = _splashParent;
        _spalshPool = new SplashPool(splashPrefab, _splashParent);
    }


    public void DoSplash()
    {
        var splashView = _spalshPool.Get();

        //set background sprite
        int index;

        index = Random.Range(0, _backgrounds.Length);
        var sprite = _backgrounds[index];

        //set text
        index = Random.Range(0, phrases.Length);
        var text = phrases[index];

        //set animation
        index = Random.Range(0, _animations.Length);
        var animation = _animations[index];

        //set color
        index = Random.Range(0, _splashColorPalette.ColorPalette.Length);
        var colors = _splashColorPalette.ColorPalette[index];

        splashView.SetAndPlay(animation, sprite, text, colors);
    }
}
