using UnityEngine;

public class TowerPoolItem : IpoolItem
{
    public GameObject obj { get; private set; }

    public bool isPerfect;
    public Vector3 maxScaleTo;
    public Vector3 finalScaleTo;


    public TowerPoolItem(GameObject go)
    {
        obj = go;
    }


    public void SetWaveScales(Vector3 maxScaleTo, Vector3 finalScaleTo)
    {
        this.maxScaleTo = maxScaleTo;
        this.finalScaleTo = finalScaleTo;
    }
}
