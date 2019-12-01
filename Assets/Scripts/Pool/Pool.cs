using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pool : MonoBehaviour
{
    [SerializeField] GameObject poolItemtPrefab;
    [SerializeField] int startPoolsize = 10;
    [SerializeField] int poolExpandStep = 5;

    List<IpoolItem> itemsPool;
    int desiredItemIndex;


    private void Start()
    {
        itemsPool = new List<IpoolItem>();

        GameManager.Instance.RestartEvent += ResetPool;
        PoolExpand(startPoolsize);
    }


    public void PoolExpand(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            var go = Instantiate(poolItemtPrefab, transform);
            go.SetActive(false);

            itemsPool.Add(new TowerPoolItem(go));
        }
    }

    //expand pool if all active enelemts in using
    public void ExpandNeedCheck()
    {
        if (desiredItemIndex == itemsPool.Count - 1)
            PoolExpand(poolExpandStep);

        desiredItemIndex++;
    }

    public virtual void ResetPool()
    {
        for (int i = 0; i < itemsPool.Count; i++)
        {
            itemsPool[i].obj.SetActive(false);
        }

        desiredItemIndex = 0;
    }

    /// <summary>
    /// Use when you active pool items one-by-one before before pool reset
    /// </summary>
    public IpoolItem GetNextPoolItem()
    {
        return itemsPool[desiredItemIndex];
    }

    /// <summary>
    /// Return first inactive item from pool
    /// </summary>
    public IpoolItem GetFirstAvaliableItem()
    {
        for (int i = 0; i < itemsPool.Count; i++)
        {
            var pi = itemsPool[0] as IpoolItem;
            if (!pi.obj.activeSelf)
                return pi;
        }

        return null;
    }

    //unused now
    public int GetPoolSize()
    {
        return itemsPool.Count;
    }

    public int GetActiveElementsCount()
    {
        int counnt = 0;

        for (int i = 0; i < itemsPool.Count; i++)
        {
            if (itemsPool[i].obj.activeSelf)
                counnt++;
            else
                break;
        }
        return counnt;

    }

    public IpoolItem GetItemByIndex(int index)
    {
        return itemsPool[index];
    }
}
