using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerStatusUI_Bars : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private List<StatBarUI> statBarUIList;

    private Dictionary<StatType, StatBarUI> uiDict = new();
    private Dictionary<StatType, BlinkController> blinkingBars = new();

    void Start()
    {
        foreach (var ui in statBarUIList)
        {
            uiDict[ui.type] = ui;
            SetupBars(ui);
        }
    }

    void Update()
    {
        RefreshBars();
    }

    void SetupBars(StatBarUI ui)
    {
        if (playerStatus == null)
        {
            Debug.LogError($"[SetupBars] playerStatus is NULL for {ui.type}");
            return;
        }

        var stat = playerStatus.GetStat(ui.type);
        if (stat == null)
        {
            Debug.LogError($"[SetupBars] Stat is NULL for {ui.type} in {playerStatus.name}");
            return;
        }

        Debug.Log($"[SetupBars] Creating bars for {ui.type}, Max: {stat.maxValue}");

        int requiredBars = Mathf.CeilToInt(stat.maxValue / ui.valuePerBar);

        if (ui.iconPrefab != null && ui.iconParent != null)
        {
            GameObject icon = Instantiate(ui.iconPrefab, ui.iconParent);
            Image iconImage = icon.GetComponent<Image>();
            iconImage.sprite = ui.iconSprite;
        }

        for (int i = 0; i < requiredBars; i++)
        {
            var go = Instantiate(ui.barPrefab, ui.barParent);
            Debug.Log($"[BarSpawn] Instantiated {ui.type} bar #{i} under {ui.barParent.name}");
            if (!go.TryGetComponent(out BlinkController blink))
                blink = go.AddComponent<BlinkController>();

            blink.image = go.GetComponent<Image>();
            blink.filledSprite = ui.filledSprite;
            blink.blinkingSprite = ui.blinkingSprite;
            blink.emptySprite = ui.emptySprite;
            blink.SetEmpty(); 
        }
    }

    void RefreshBars()
    {
        foreach (var uiPair in uiDict)
        {
            StatType type = uiPair.Key;
            StatBarUI ui = uiPair.Value;
            Stat stat = playerStatus.GetStat(type);
            if (stat == null || ui.barParent == null) continue;

            int barCount = ui.barParent.childCount;
            int filledBars = Mathf.CeilToInt(stat.currentValue / ui.valuePerBar);

            for (int i = 0; i < barCount; i++)
            {
                var blink = ui.barParent.GetChild(i).GetComponent<BlinkController>();
                if (i < filledBars)
                    blink.SetFilled();
                else
                    blink.SetEmpty();
            }

            UpdateBlinkingBar(type, filledBars, ui, stat);
        }
    }

    void UpdateBlinkingBar(StatType type, int filledBars, StatBarUI ui, Stat stat)
    {
        int barCount = ui.barParent.childCount;
        int blinkIndex = filledBars - 1; 

        if (blinkingBars.TryGetValue(type, out var prevBlink))
        {
            if (blinkIndex >= 0 && blinkIndex < barCount &&
                ui.barParent.GetChild(blinkIndex).GetComponent<BlinkController>() == prevBlink)
            {
                float newInterval = GetBlinkInterval(type, stat);
                if (prevBlink.BlinkInterval != newInterval)
                {
                    prevBlink.StopBlink();
                    prevBlink.BlinkInterval = newInterval;
                    prevBlink.StartBlink();
                }
                return;
            }

            prevBlink.StopBlink();
            blinkingBars[type] = null;
        }

        if (blinkIndex >= 0 && blinkIndex < barCount)
        {
            var newBlink = ui.barParent.GetChild(blinkIndex).GetComponent<BlinkController>();
            blinkingBars[type] = newBlink;
            newBlink.SetFilled();

            if (type != StatType.Health && (
                (type == StatType.Stamina && StatusManager.Instance.IsRunning && stat.currentValue < stat.maxValue) ||
                (type == StatType.Hunger || type == StatType.Thirst)))
            {
                newBlink.BlinkInterval = GetBlinkInterval(type, stat);
                newBlink.StartBlink();
            }
            else
            {
                newBlink.StopBlink();
            }
        }
    }

    float GetBlinkInterval(StatType type, Stat stat)
    {
        if (type == StatType.Stamina && StatusManager.Instance.IsRunning && stat.currentValue < stat.maxValue)
            return 1f;
        if (type == StatType.Hunger || type == StatType.Thirst)
            return StatusManager.Instance.IsRunning ? 0.5f : 1f;
        return 1f;
    }
}

[System.Serializable]
public class StatBarUI
{
    public StatType type;
    public Sprite filledSprite;
    public Sprite emptySprite;
    public Sprite blinkingSprite;
    public GameObject barPrefab;
    public Transform barParent;
    public Sprite iconSprite;
    public GameObject iconPrefab;
    public Transform iconParent;
    public float valuePerBar = 10f;
}