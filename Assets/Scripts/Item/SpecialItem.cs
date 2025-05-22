using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialItemType
{
    HealthRecovery,
    HungerRecovery,
    ThirstRecovery,
    StaminaRecovery,
    None
}


public class SpecialItem : BaseItem
{
    public SpecialItemType Type;

    public void Use()
    {
        float amount = Data.AttackPower;
        switch(Type)
        {
            case SpecialItemType.HealthRecovery:
                StatusApplyManager.Instance.RecoveryHealth(amount);
                break;
            case SpecialItemType.HungerRecovery:
                StatusApplyManager.Instance.RecoveryHunger(amount);
                break;
            case SpecialItemType.ThirstRecovery:
                StatusApplyManager.Instance.RecoveryThirst(amount);
                break;
            case SpecialItemType.StaminaRecovery:
                StatusApplyManager.Instance.RecoveryStamina(amount);
                break;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Mouth"))
        {
            Use();
            Destroy(gameObject);
        }
    }
}
