using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoolManager : MonoBehaviour
{
    public static TowerPoolManager Instance;
    [SerializeField] GameObject towerElementPrefab;
    [SerializeField] int startPoolsize;
    [SerializeField] int poolExpandStep;


    [SerializeField] Material commonElementMaterial;
    [HideInInspector] public List<PoolItem> towerElementsPool;
    [HideInInspector] public int desiredItemIndex { get; private set; }


    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.towerElementsPool = new List<PoolItem>();
            Instance.commonElementMaterial = commonElementMaterial;
            Instance.towerElementPrefab = towerElementPrefab;
            Instance.poolExpandStep = poolExpandStep;

            Instance.PoolExpand(startPoolsize);
        }
        else
            Destroy(gameObject);
    }


    public void PoolExpand(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            var go = Instantiate(Instance.towerElementPrefab);
            go.SetActive(false);

            towerElementsPool.Add(new PoolItem(go));
        }
    }

    //expand pool if all active enelemts in using
    public void GoOnPoolExpandCheck()
    {
        if (desiredItemIndex == towerElementsPool.Count-1)
            Instance.PoolExpand(Instance.poolExpandStep);

        desiredItemIndex++;
    }

    public void ResetPoolOff()
    {
        for (int i = 0; i < towerElementsPool.Count; i++)
        {
            towerElementsPool[i].obj.SetActive(false);
            towerElementsPool[i].obj.GetComponent<MeshRenderer>().material = commonElementMaterial;
        }

        desiredItemIndex = 0;
    }

    public PoolItem GetNewTowerElement()
    {
        return Instance.towerElementsPool[desiredItemIndex];
    }
}