using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SplashPhrase : MonoBehaviour, Isplash
{
    Animation splashAnimation;
    Image background;
    TextMesh textMesh;

    private void Start()
    {
        SturtupSet();
        gameObject.SetActive(false);
    }


    public void SturtupSet()
    {
        splashAnimation = GetComponent<Animation>();
        background = GetComponentInChildren<Image>();
        textMesh = GetComponentInChildren<TextMesh>();
    }

    public void SetAndPlay(Sprite splashBackground, string text)
    {
        background.sprite = splashBackground;
        textMesh.text = text;
        splashAnimation.Play();

        StartCoroutine(OnClipEnd());
    }

    IEnumerator OnClipEnd()
    {
        while (splashAnimation.isPlaying)
        {
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }
    }
}
