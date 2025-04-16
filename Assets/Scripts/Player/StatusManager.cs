using UnityEngine;

// 스탯 자동 감소/회복 로직 클래스
public class StatusManager : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private float timer;

    public static StatusManager Instance { get; private set; }

    public bool IsRunning { get; private set; }

    [Header("스탯 자동 감소/회복 설정")]
    [SerializeField] private StatEffectSettings effectSettings;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            HandleHunger();
            HandleThirst();
            HandleHealth();
            HandleStamina();

            timer = 0f;
        }
    }

    // 허기 조절
    private void HandleHunger()
    {
        // 매 초 허기 감소
        if (IsRunning)
        {
            playerStatus.ModifyStat(StatType.Hunger, -effectSettings.hungerDecreasePerSecondWhileRunning);
        }
        else
        {
            playerStatus.ModifyStat(StatType.Hunger, -effectSettings.hungerDecreasePerSecond);
        }

        // 허기가 0일때 체력 감소
        if (playerStatus.GetStat(StatType.Hunger).currentValue == 0)
        {
            playerStatus.ModifyStat(StatType.Health, -effectSettings.hpDecreaseWhenHungerZero);
        }
    }

    // 갈증 조절
    private void HandleThirst()
    {
        // 매 초 갈증 감소
        if (IsRunning)
        {
            playerStatus.ModifyStat(StatType.Thirst, -effectSettings.thirstDecreasePerSecondWhileRunning);
        }
        else
        {
            playerStatus.ModifyStat(StatType.Thirst, -effectSettings.thirstDecreasePerSecond);
        }
    }

    // 체력 조절
    private void HandleHealth()
    {
        float hunger = playerStatus.GetStat(StatType.Hunger).currentValue;

        // 허기가 일정 수치 이상일 때 체력 회복
        if (hunger >= 900f)
        {
            playerStatus.ModifyStat(StatType.Health, effectSettings.hpRegenHunger900);
        }
        else if (hunger >= 500f)
        {
            playerStatus.ModifyStat(StatType.Health, effectSettings.hpRegenHunger500);
        }

        // 체력이 0이면 사망
        if (playerStatus.GetStat(StatType.Health).currentValue == 0)
        {
            Die();
        }
    }

    // 스태미나 조절
    private void HandleStamina()
    {
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        if (IsRunning)
        {
            playerStatus.ModifyStat(StatType.Stamina, -effectSettings.staminaDecreasePerSecondWhileRunning);
        }
        else
        {
            // 갈증이 0일때 스태미나 회복량 절반
            if(playerStatus.GetStat(StatType.Thirst).currentValue == 0)
            {
                playerStatus.ModifyStat(StatType.Stamina, effectSettings.staminaRecoverPerSecond * effectSettings.staminaDecreasePerSecondWhileThristZero);
            }
            else
            {
                playerStatus.ModifyStat(StatType.Stamina, effectSettings.staminaRecoverPerSecond);
            } 
        }
    }

    private void Die()
    {

    }
}