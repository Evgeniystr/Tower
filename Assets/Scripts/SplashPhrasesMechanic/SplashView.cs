using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;

public class SplashView : MonoBehaviour
{
    [SerializeField]
    private Animation _animationComponent;
    [SerializeField]
    private Image _backgroundShadow;
    [SerializeField]
    private Image _background;
    [SerializeField]
    private TextMeshProUGUI _textShadow;
    [SerializeField]
    private TextMeshProUGUI _text;


    private SplashSettings _splashSettings;
    private Color _shadowColorShift;

    public void Initialize(SplashSettings splashSettings)
    {
        _splashSettings = splashSettings;
        _shadowColorShift = new Color(30, 30, 30, 30);
    }

    public async void SetAndPlay(Vector3 position, Action animEndCallback)
    {
        SetPositionAndRotation(position);

        var colors = _splashSettings.GetRandomColorsPreset();

        _background.sprite = _splashSettings.GetRandomBackground();
        _background.color = colors.bgColor;
        _backgroundShadow.color = colors.bgColor - _shadowColorShift;

        var phraseText = _splashSettings.GetRandomPhrase();
        _textShadow.text = phraseText;
        _text.text = phraseText;
        _textShadow.color = colors.textColor - _shadowColorShift;
        _text.color = colors.textColor;

        _animationComponent.clip = _splashSettings.GetRandomAnimation();

        _animationComponent.Play();

        //wait animation end
        await UniTask.WaitUntil(() => !_animationComponent.isPlaying);

        animEndCallback.Invoke();
    }

    void SetPositionAndRotation(Vector3 position)
    {
        //rotation------
        var euler = UnityEngine.Random.Range(-40, 40);
        var xRot = new Vector3(0, 0, euler);

        transform.localPosition = position;
        transform.localEulerAngles = xRot;
    }
}
