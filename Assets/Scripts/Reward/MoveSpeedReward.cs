// MoveSpeedReward.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MoveSpeedReward", menuName = "Catalog/Rewards/MoveSpeed")]
public class MoveSpeedReward : CatalogReward
{
    [Tooltip("기본 이동속도의 몇 % 만큼 증가시킬지 (0.2 = 20%)")]
    [Range(0f, 1f)] public float percentIncrease;

    public override void Apply(PlayerStatus playerStatus)
    {
        var stat = playerStatus.GetStat(StatType.MovementSpeed);
        if (stat == null) return;

        float delta = stat.maxValue * percentIncrease;
        playerStatus.IncreaseStatMax(StatType.MovementSpeed, delta);
    }
}
