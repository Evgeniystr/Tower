using System;
using UnityEngine;

public class SplashHandler : MonoBehaviour
{
    [SerializeField] Pool pool;
    [SerializeField] AnimationClip[] animations;
    [SerializeField] Sprite[] backgrounds;
    [SerializeField] SplashColorPalette splashColorPalette;

    string[] phrases;
    RectTransform[] splashPlace;


    private void Start()
    {
        phrases = new string[] { "Yummie", "Splash", "Mmmm", "Yaay!", "Juice!", "Sabroso", "Om nom nom", "Oh Lord!", "Perfect!", "So GOOD!!", "Nice!"};
        splashPlace = GetComponentsInChildren<RectTransform>();
    }


    public void DoSplash()
    {
        var poolItem = pool.GetFirstAvaliableItem();
        var splashPhrase = poolItem.obj.GetComponent<SplashPhrase>();

        //set background sprite
        int index;

        index = UnityEngine.Random.Range(0, backgrounds.Length);
        var sprite = backgrounds[index];

        //set text
        index = UnityEngine.Random.Range(0, phrases.Length);
        var text = phrases[index];

        //set animation
        index = UnityEngine.Random.Range(0, animations.Length);
        var animation = animations[index];

        //set color
        index = UnityEngine.Random.Range(0, splashColorPalette.ColorPalette.Length);
        var colors = splashColorPalette.ColorPalette[index];

        SetPositionAndRotation(poolItem.obj);

        splashPhrase.SetAndPlay(animation, sprite, text, colors);
    }

    void SetPositionAndRotation(GameObject go)
    {
        //position------
        var desiredPlace = splashPlace[UnityEngine.Random.Range(1, splashPlace.Length)];//skip 0 parent index
        Vector2 offset = GetSplashOffsets(go);

        var xPosRange = desiredPlace.sizeDelta.x - offset.x;
        var yPosRange = desiredPlace.sizeDelta.y - offset.y;

        var newPos = new Vector3(UnityEngine.Random.Range(-xPosRange/2, xPosRange/2),
                                 UnityEngine.Random.Range(-yPosRange/2, yPosRange/2),
                                 0);

        go.transform.SetParent(desiredPlace);

        //warning message
        if (xPosRange < 0 || yPosRange < 0)
            Debug.LogError("Splash size bigger than parent rectancle");

        //rotation------
        var euler = UnityEngine.Random.Range(-40, 40);
        //Quaternion newRot = Quaternion.Euler(0, 0, euler);
        var x = new Vector3(0, 0, euler);




        go.transform.localPosition = newPos;
        go.transform.localEulerAngles = x;
    }

    Vector2 GetSplashOffsets(GameObject go)
    {
        float xOffset = 0;
        float yOffset = 0;

        var rects = go.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < rects.Length; i++)
        {
            xOffset = Mathf.Max(xOffset, rects[i].sizeDelta.x);
            yOffset = Mathf.Max(yOffset, rects[i].sizeDelta.y);
        }

        return new Vector2(xOffset, yOffset);
    }
}
