// BackpackSizeReward.cs
using UnityEngine;

[CreateAssetMenu(fileName = "BackpackSizeReward", menuName = "Catalog/Rewards/BackpackSize")]
public class BackpackSizeReward : CatalogReward
{
    [Tooltip("백팩 슬롯을 몇 칸 늘릴지")]
    public int extraSlots;

    public override void Apply(PlayerStatus playerStatus)
    {
        var im = InventoryManager.Instance;
        im.InventoryCnt += extraSlots;
    }
}
