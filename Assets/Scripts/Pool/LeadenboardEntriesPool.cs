using UnityEngine;

public class LeadenboardEntriesPool : APool<LeaderboardEntryView>
{
    private LeaderboardEntryView _prefab;
    private Transform _parent;

    public LeadenboardEntriesPool(LeaderboardEntryView prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }


    protected override LeaderboardEntryView CreateItem()
    {
        var instanceView = GameObject.Instantiate(_prefab, _parent);
        instanceView.Hide();

        return instanceView;
    }

    public override void ReleaseItem(LeaderboardEntryView item)
    {
        item.Hide();
        base.ReleaseItem(item);
    }
}
