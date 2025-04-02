using UnityEngine;
using System.Collections.Generic;

// 플레이어의 모든 스탯을 관리하는 클래스
public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private List<Stat> stats = new List<Stat>(); 
    private Dictionary<StatType, Stat> statDict;                   

    private void Awake()
    {
        statDict = new Dictionary<StatType, Stat>();

        // 리스트에서 딕셔너리로 매핑
        foreach (var stat in stats)
        {
            statDict[stat.Type] = stat;
        }
    }

    // 특정 스탯을 반환
    public Stat GetStat(StatType type)
    {
        if (statDict.ContainsKey(type))
        {
            return statDict[type];
        }

        return null;
    }

    // 특정 스탯 값 변경
    public void ModifyStat(StatType type, float amount)
    {
        if (statDict.ContainsKey(type))
        {
            statDict[type].Modify(amount);
        }
    }

    // 도감작 등으로 최대값 증가
    public void IncreaseStatMax(StatType type, float amount)
    {
        if (statDict.ContainsKey(type))
        {
            statDict[type].IncreaseMax(amount);
        }
    }
}