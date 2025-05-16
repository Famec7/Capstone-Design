using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ClaimManager : Singleton<ClaimManager>
{
    private HashSet<string> claimed = new HashSet<string>();
    private Dictionary<string, HashSet<string>> usedMaterials = new Dictionary<string, HashSet<string>>();
    private const string PREF_CLAIM_KEY = "claimedCollections";
    private const string PREF_USE_PREFIX = "usedMaterials_";

    [SerializeField] private List<CatalogCollection> allCollections;
    [SerializeField] private PlayerStatus playerStatus;

    protected override void Init()
    {
        Load();
        LoadUsedMaterials();
        ApplyPreviouslyClaimed();
    }
    private void Load()
    {
        var data = PlayerPrefs.GetString(PREF_CLAIM_KEY, "");
        if (!string.IsNullOrEmpty(data))
            claimed = new HashSet<string>(data.Split(','));
    }

    private void Save()
    {
        PlayerPrefs.SetString(PREF_CLAIM_KEY, string.Join(",", claimed));
        PlayerPrefs.Save();
    }

    private void LoadUsedMaterials()
    {
        foreach (var col in allCollections)
        {
            var key = PREF_USE_PREFIX + col.name;
            var raw = PlayerPrefs.GetString(key, "");
            usedMaterials[col.name] =
                string.IsNullOrEmpty(raw)
                ? new HashSet<string>()
                : new HashSet<string>(raw.Split(','));
        }
    }

    private void SaveUsedMaterials(string collectionName)
    {
        var key = PREF_USE_PREFIX + collectionName;
        var set = usedMaterials[collectionName];
        PlayerPrefs.SetString(key, string.Join(",", set));
        PlayerPrefs.Save();
    }

    private void ApplyPreviouslyClaimed()
    {
        foreach (var id in claimed)
        {
            var col = allCollections.FirstOrDefault(c => c.name == id);
            if (col != null)
                col.reward.Apply(playerStatus);
        }
    }

    public bool IsClaimed(CatalogCollection col)
    {
        return claimed.Contains(col.name);
    }

    public bool TryClaim(CatalogCollection col)
    {
        var inv = InventoryManager.Instance;

        if (IsClaimed(col))
            return false;

        // 필요한 모든 아이템이 있는지 체크
        foreach (var req in col.requiredItems)
        {
            bool found = inv.Slots.Any(slot => !slot.IsEmpty && slot.Data == req);
            if (!found)
                return false;
        }

        // 아이템 소모
        foreach (var req in col.requiredItems)
        {
            var slotIndex = System.Array.FindIndex(inv.Slots,
                slot => !slot.IsEmpty && slot.Data == req);

            if (slotIndex >= 0)
                inv.RemoveItem(slotIndex);
        }

        // 보상 적용
        col.reward.Apply(playerStatus);

        // 획득 기록 저장
        claimed.Add(col.name);
        Save();

        return true;
    }

    public bool IsMaterialUsed(string collectionName, string itemName)
    {
        if (!usedMaterials.ContainsKey(collectionName))
            usedMaterials[collectionName] = new HashSet<string>();
        return usedMaterials[collectionName].Contains(itemName);
    }

    public bool UseMaterial(string collectionName, string itemName)
    {
        if (IsMaterialUsed(collectionName, itemName))
            return false;

        usedMaterials[collectionName].Add(itemName);
        SaveUsedMaterials(collectionName);
        ClaimManager.OnMaterialUsed?.Invoke(collectionName, itemName);
        return true;
    }


    public static event System.Action<string, string> OnMaterialUsed;
}
