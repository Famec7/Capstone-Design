using UnityEngine;

public class StatusWidgetController : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private PlayerStatusUI statusUI;

    private void Start()
    {
        // 모든 스탯에 이벤트 리스너 연결
        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
        {
            Stat stat = playerStatus.GetStat(type);
            if (stat != null)
            {
                stat.OnStatChanged += (currentValue) =>
                {
                    statusUI.UpdateStat(type, currentValue, stat.MaxValue);
                };

                // 게임 시작 시 초기값 반영
                statusUI.UpdateStat(type, stat.CurrentValue, stat.MaxValue);
            }
        }
    }
}
