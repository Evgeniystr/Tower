using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashView : MonoBehaviour
{
    [SerializeField]
    private Animation _animationComponent;
    [SerializeField]
    private Image _background;
    [SerializeField]
    private TextMeshProUGUI _text;


    public void SetAndPlay(AnimationClip animation, Sprite splashBackground, string text, ColorPair colors)
    {
        SetPositionAndRotation();

        _background.sprite = splashBackground;
        _background.color = colors.bgColor;

        _text.text = text;
        _text.color = colors.textColor;

        _animationComponent.clip = animation;

        gameObject.SetActive(true);
        _animationComponent.Play();
        StartCoroutine(OnClipEnd());
    }

    void SetPositionAndRotation()
    {
        //position------
        var rectHalfSize = transform.GetComponent<RectTransform>().sizeDelta / 2;

        var x = Random.Range(transform.position.x - rectHalfSize.x, transform.position.x - rectHalfSize.x);
        var y = Random.Range(transform.position.y - rectHalfSize.y, transform.position.y - rectHalfSize.y);
        var newPos = new Vector3(x, y);

        //rotation------
        var euler = Random.Range(-40, 40);
        //Quaternion newRot = Quaternion.Euler(0, 0, euler);
        var xRot = new Vector3(0, 0, euler);

        transform.localPosition = newPos;
        transform.localEulerAngles = xRot;
    }

    IEnumerator OnClipEnd()
    {
        while (_animationComponent.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
