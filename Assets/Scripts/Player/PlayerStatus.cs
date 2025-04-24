using UnityEngine;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private List<Stat> statList;
    private Dictionary<StatType, Stat> statDict;

    private void Awake()
    {
        statDict = new Dictionary<StatType, Stat>();
        foreach (var stat in statList)
        {
            statDict[stat.type] = stat;
        }
    }

    public Stat GetStat(StatType type)
    {
        return statDict.ContainsKey(type) ? statDict[type] : null;
    }

    public void ModifyStat(StatType type, float amount)
    {
        if (statDict.ContainsKey(type))
        {
            statDict[type].Modify(amount);
        }
    }

    public void IncreaseStatMax(StatType type, float amount)
    {
        if (statDict.ContainsKey(type))
        {
            statDict[type].IncreaseMax(amount);
        }
    }
}