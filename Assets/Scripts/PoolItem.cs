using UnityEngine;

public class PoolItem
{
    public PoolItem(GameObject towerElementObj)
    {
        obj = towerElementObj;
    }

    public GameObject obj;
    public bool isPerfect;
    public Vector3 maxScaleTo;
    public Vector3 finalScaleTo;

    public void SetWaveScales(Vector3 maxScaleTo, Vector3 finalScaleTo)
    {
        this.maxScaleTo = maxScaleTo;
        this.finalScaleTo = finalScaleTo;
    }
}
