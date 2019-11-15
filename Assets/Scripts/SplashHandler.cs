using System;
using UnityEngine;

public class SplashHandler : MonoBehaviour
{
    [SerializeField] Pool pool;
    [SerializeField] AnimationClip[] animations;
    [SerializeField] Sprite[] backgrounds;

    string[] phrases;
    RectTransform[] splashPlace;


    private void Start()
    {
        phrases = new string[] { "Yummie", "Splash",};
        splashPlace = GetComponentsInChildren<RectTransform>();
    }


    public void DoSplash()
    {
        var poolItem = pool.GetFirstAvaliableItem();
        var splashPhrase = poolItem.obj.GetComponent<SplashPhrase>();

        //get sprite
        int index;

        index = UnityEngine.Random.Range(0, backgrounds.Length - 1);
        var sprite = backgrounds[index];

        //get text
        index = UnityEngine.Random.Range(0, phrases.Length - 1);
        var text = phrases[index];

        //get animation
        index = UnityEngine.Random.Range(0, animations.Length - 1);
        var animation = animations[index];

        SetPositionAndRotation(poolItem.obj);

        splashPhrase.SetAndPlay(animation, sprite, text);
    }

    void SetPositionAndRotation(GameObject go)
    {
        var desiredPlace = splashPlace[UnityEngine.Random.Range(1, splashPlace.Length - 1)];//skip 0 parent index
        Vector2 offset = GetSplashOffsets(go);

        var xPosRange = desiredPlace.sizeDelta.x - offset.x;
        var yPosRange = desiredPlace.sizeDelta.y - offset.y;

        var newPos = new Vector3(UnityEngine.Random.Range(-xPosRange/2, xPosRange/2),
                                 UnityEngine.Random.Range(-yPosRange/2, yPosRange/2),
                                 0);

        go.transform.SetParent(desiredPlace);
        go.transform.localPosition = newPos;

        //warning message
        if (xPosRange < 0 || yPosRange < 0)
            Debug.LogError("Splash size bigger than parent rectancle");
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
