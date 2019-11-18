using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashPhrase : MonoBehaviour
{
    Animation animationComponent;
    Image background;
    //TextMesh textMesh;
    TextMeshProUGUI textMesh;

    bool isNeedBeSetted = true;


    void StartupSet()
    {
        if (isNeedBeSetted)
        {
            animationComponent = GetComponent<Animation>();
            background = GetComponentInChildren<Image>();
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

            isNeedBeSetted = false;
        }
    }


    public void SetAndPlay(AnimationClip animation, Sprite splashBackground, string text, ColorPair colors)
    {
        StartupSet();

        background.sprite = splashBackground;
        background.color = colors.bgColor;

        textMesh.text = text;
        textMesh.color = colors.textColor;

        animationComponent.clip = animation;

        gameObject.SetActive(true);
        animationComponent.Play();
        StartCoroutine(OnClipEnd());
    }

    IEnumerator OnClipEnd()
    {
        while (animationComponent.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
