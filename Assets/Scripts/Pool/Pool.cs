using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pool : MonoBehaviour
{
    [SerializeField] GameObject poolItemtPrefab;
    [SerializeField] int startPoolsize;
    [SerializeField] int poolExpandStep;

    List<IpoolItem> itemsPool;
    int desiredItemIndex;


    private void OnEnable()
    {
        itemsPool = new List<IpoolItem>();

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


    public IpoolItem GetNewPoolItem()
    {
        return itemsPool[desiredItemIndex];
    }

    public int GetPoolSize()
    {
        return itemsPool.Count;
    }

    public IpoolItem GetItemByIndex(int index)
    {
        return itemsPool[index];
    }
}
