// MaxStaminaReward.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MaxStaminaReward", menuName = "Catalog/Rewards/MaxStamina")]
public class MaxStaminaReward : CatalogReward
{
    [Tooltip("기본 최대 스태미나의 몇 % 만큼 증가시킬지 (0.5 = 50%)")]
    [Range(0f, 1f)] public float percentIncrease;

    public override void Apply(PlayerStatus playerStatus)
    {
        var stat = playerStatus.GetStat(StatType.Stamina);
        if (stat == null) return;

        float delta = stat.maxValue * percentIncrease;
        playerStatus.IncreaseStatMax(StatType.Stamina, delta);
    }
}
