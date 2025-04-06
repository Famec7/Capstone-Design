using UnityEngine;

// 스탯 조정 수치 설정 클래스
[System.Serializable]
public class StatEffectSettings
{
    [Header("기본 감소량")]
    public float hungerDecreasePerSecond = 5f;
    public float thirstDecreasePerSecond = 5f;

    [Header("허기 상태에 따른 체력 회복")]
    public float hpRegenHunger500 = 1f;
    public float hpRegenHunger900 = 2f;

    [Header("허기 0일 때 체력 감소")]
    public float hpDecreaseWhenHungerZero = 5f;

    [Header("스태미나 사용/회복 속도")]
    public float staminaDecreasePerSecondWhileRunning = 10f;
    public float staminaRecoverPerSecond = 10f;

    [Header("갈증 0일 때 스태미나 회복 속도 저하")]
    public float staminaDecreasePerSecondWhileThristZero = 0.5f;
}
