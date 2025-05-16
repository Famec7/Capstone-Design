// MaxHPReward.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MaxHPReward", menuName = "Catalog/Rewards/MaxHP")]
public class MaxHPReward : CatalogReward
{
    [Tooltip("기본 최대 생명력의 몇 % 만큼 증가시킬지 (0.5 = 50%)")]
    [Range(0f, 1f)] public float percentIncrease;

    public override void Apply(PlayerStatus playerStatus)
    {
        var stat = playerStatus.GetStat(StatType.Health);
        if (stat == null) return;

        // 현재 maxValue에 비례하여 증가량 계산
        float delta = stat.maxValue * percentIncrease;
        playerStatus.IncreaseStatMax(StatType.Health, delta);
    }
}
