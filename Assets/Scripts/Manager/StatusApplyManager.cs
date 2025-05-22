using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusApplyManager : Singleton<StatusApplyManager>
{
    public PlayerStatus Status;
    protected override void Init()
    {
    }

    public void RecoveryHealth(float amount)
    {
        Status.ModifyStat(StatType.Health,amount);
    }

    public void RecoveryStamina(float amount)
    {
        Status.ModifyStat(StatType.Stamina, amount);
    }

    public void RecoveryThirst(float amount)
    {
        Status.ModifyStat(StatType.Thirst, amount);
    }

    public void RecoveryHunger(float amount)
    {
        Status.ModifyStat(StatType.Hunger, amount);
    }
}
