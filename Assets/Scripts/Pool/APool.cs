using System.Collections.Generic;
using System.Linq;

public abstract class APool<T>
{
    private List<PoolEntry> pool;//item, avaliable status
    private int startSize;
    private int growStep;


    public APool(int startSize = 4, int growStep = 4)
    {
        this.startSize = startSize;
        this.growStep = growStep;

        pool = new List<PoolEntry>(startSize);
    }

    protected abstract T CreateItem();
    public virtual void ReleaseItem(T item)
    {
        var poolEntry = pool.FirstOrDefault(entry => entry.Value.Equals(item));
        poolEntry.Avaliable = true;
    }

    public virtual T Get()
    {
        var poolEntry = pool.FirstOrDefault(item => item.Avaliable);

        T resultItem;

        //no avaliable items in pool
        if (poolEntry != null)
        {
            resultItem = poolEntry.Value;
        }
        else
        {
            Extend();
            resultItem = Get();
            poolEntry = pool.FirstOrDefault(entry => entry.Value.Equals(resultItem));
        }

        poolEntry.Avaliable = false;
        return resultItem;
    }

    private void Extend()
    {
        int newItemsCount = pool.Count == 0 ? startSize : growStep;

        for (int i = 0; i < newItemsCount; i++)
        {
            var newItem = CreateItem();
            var newPoolEntry = new PoolEntry(newItem);
            pool.Add(newPoolEntry);
        }
    }

    private class PoolEntry
    {
        public bool Avaliable = true;
        public T Value;

        public PoolEntry(T item)
        {
            Value = item;
        }
    }
}
