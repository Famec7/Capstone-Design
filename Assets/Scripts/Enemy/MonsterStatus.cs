using UnityEngine;
using System.Collections.Generic;

public class MonsterStatus : MonoBehaviour
{
    [SerializeField] private List<Stat> statList;
    private Dictionary<StatType, Stat> statDict;

    private void Awake()
    {
        statDict = new Dictionary<StatType, Stat>();
        foreach (var stat in statList)
            statDict[stat.type] = stat;
    }

    public Stat GetStat(StatType type)
    {
        return statDict.TryGetValue(type, out var stat) ? stat : null;
    }

    public void ModifyStat(StatType type, float amount)
    {
        if (statDict.TryGetValue(type, out var stat))
            stat.Modify(amount);
    }

    public void IncreaseMaxStat(StatType type, float amount)
    {
        if (statDict.TryGetValue(type, out var stat))
            stat.IncreaseMax(amount);
    }
}
