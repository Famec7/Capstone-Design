using UnityEngine;

public abstract class CatalogReward : ScriptableObject
{
    public abstract void Apply(PlayerStatus player);
}
