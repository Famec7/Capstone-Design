using UnityEngine;
using System;

// 개별 스탯 표현 클래스
[System.Serializable]
public class Stat
{
    [SerializeField] private StatType type;
    [SerializeField] private float currentValue;
    [SerializeField] private float maxValue;

    public StatType Type => type;
    public float CurrentValue => currentValue;
    public float MaxValue => maxValue;

    // 스탯 값 변경될 때 알림용 이벤트
    public event Action<float> OnStatChanged;

    public Stat(StatType type, float current, float max)
    {
        this.type = type;
        this.currentValue = current;
        this.maxValue = max;
    }

    // 스탯 값 증가, 감소
    public void Modify(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
        OnStatChanged?.Invoke(currentValue);
    }

    // UI에 쓰일 퍼센트 반환
    public float GetPercentage()
    {
        return currentValue / maxValue;
    }

    // 도감작으로 인한 최대 스탯 증가
    public void IncreaseMax(float amount)
    {
        maxValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }
}
