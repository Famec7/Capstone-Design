using UnityEngine;
using System;

[System.Serializable]
public class Stat
{
    public StatType type;
    public float currentValue;
    public float maxValue;

    public float ValuePerBar => (type == StatType.Hunger || type == StatType.Thirst) ? 100f : 10f;

    public event Action<float> OnStatChanged;

    public void Modify(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        OnStatChanged?.Invoke(currentValue);
    }

    public void IncreaseMax(float amount)
    {
        maxValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        OnStatChanged?.Invoke(currentValue);
    }
}