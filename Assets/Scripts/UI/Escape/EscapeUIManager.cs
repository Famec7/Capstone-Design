using System.Collections.Generic;
using UnityEngine;

public class EscapeUIManager : MonoBehaviour
{
    public static EscapeUIManager Instance;

    [Header("UI Root")]
    [SerializeField] private GameObject uiRoot;   

    [Header("Requirements")]
    [SerializeField] private int requiredWood = 10;
    [SerializeField] private int requiredStone = 10;

    [Header("Slots")]
    [SerializeField] private List<RepairSlotHighlighter> woodSlots;
    [SerializeField] private List<RepairSlotHighlighter> stoneSlots;

    int currentWood, currentStone;

    void Awake()
    {
        Instance = this;
        uiRoot.SetActive(false);
    }

    public void ShowUI(bool show) => uiRoot.SetActive(show);

    public void RegisterMaterial(MaterialType type, Sprite icon)
    {
        if (type == MaterialType.Wood && currentWood < requiredWood)
        {
            woodSlots[currentWood].itemIcon.sprite = icon;
            woodSlots[currentWood].itemIcon.color = Color.white;
            currentWood++;
        }
        else if (type == MaterialType.Stone && currentStone < requiredStone)
        {
            stoneSlots[currentStone].itemIcon.sprite = icon;
            stoneSlots[currentStone].itemIcon.color = Color.white;
            currentStone++;
        }
        CheckComplete();
    }

    void CheckComplete()
    {
        if (currentWood >= requiredWood && currentStone >= requiredStone)
        {
            Debug.Log("모든 재료가 모였습니다! 탈출(수리) 처리 실행");
        }
    }
}