using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class StatUI
{
    public StatType type;            
    public TMP_Text nameText;        
    public Slider valueSlider;       
    public TMP_Text valueText;       
}

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private List<StatUI> statUIList;

    private Dictionary<StatType, StatUI> statUIDict;

    private void Awake()
    {
        // 스탯별로 빠르게 접근하기 위한 Dictionary 구성
        statUIDict = new Dictionary<StatType, StatUI>();
        foreach (var ui in statUIList)
        {
            if (!statUIDict.ContainsKey(ui.type))
            {
                statUIDict.Add(ui.type, ui);
                ui.nameText.text = ui.type.ToString();
            }
        }
    }

    public void UpdateStat(StatType type, float current, float max)
    {
        if (!statUIDict.ContainsKey(type))
        {
            return;
        }

        StatUI ui = statUIDict[type];

        float ratio = current / max;
        ui.valueSlider.value = ratio;
        ui.valueText.text = $"{current} / {max}";
    }
}