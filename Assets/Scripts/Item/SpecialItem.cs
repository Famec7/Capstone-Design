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
}
