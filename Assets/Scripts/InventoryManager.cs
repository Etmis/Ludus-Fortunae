using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Skin
{
    public string Id;
    public string Name;
    public string Category;
    public bool IsUnlocked;
    public Sprite Icon;

    public Skin(string id, string name, string category, bool isUnlocked)
    {
        Id = id;
        Name = name;
        Category = category;
        IsUnlocked = isUnlocked;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<Skin> allSkins = new List<Skin>();

    private Dictionary<string, Skin> selectedSkinsByCategory = new Dictionary<string, Skin>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // for developing purposes (remove later):
        //SaveUnlockedSkins();

        //LoadUnlockedSkins();
        //LoadSelectedSkins();
    }

    public void UnlockSkin(string skinId)
    {
        Skin skin = allSkins.Find(s => s.Id == skinId);
        if (skin != null)
        {
            skin.IsUnlocked = true;
            SaveUnlockedSkins();
        }
        else
        {
            Debug.LogError($"Skin with ID {skinId} not found!");
        }
    }

    public bool SelectSkin(string skinId)
    {
        Skin skin = allSkins.Find(s => s.Id == skinId);
        if (skin == null || !skin.IsUnlocked)
        {
            Debug.Log($"Skin selection failed: {skinId}");
            return false;
        }

        Debug.Log($"Selected skin: {skin.Id} for category: {skin.Category}");
        selectedSkinsByCategory[skin.Category] = skin;
        SaveSelectedSkins();
        return true;
    }

    public List<Skin> GetUnlockedSkins()
    {
        return allSkins.Where(s => s.IsUnlocked).ToList();
    }

    public List<Skin> GetLockedSkins()
    {
        return allSkins.Where(s => !s.IsUnlocked).ToList();
    }

    public Dictionary<string, Skin> GetSelectedSkins()
    {
        return new Dictionary<string, Skin>(selectedSkinsByCategory);
    }

    public Skin GetSelectedSkinForCategory(string category)
    {
        return selectedSkinsByCategory.TryGetValue(category, out Skin skin) ? skin : null;
    }

    public List<Skin> GetSkinsInCategory(string category)
    {
        return allSkins.Where(s => s.Category == category).ToList();
    }

    private void LoadUnlockedSkins()
    {
        string unlockedIds = PlayerPrefs.GetString("UnlockedSkins", "");
        HashSet<string> unlockedSet = new HashSet<string>(unlockedIds.Split(','));

        foreach (Skin skin in allSkins)
        {
            skin.IsUnlocked = unlockedSet.Contains(skin.Id);
        }
    }

    private void SaveUnlockedSkins()
    {
        string unlockedIds = string.Join(",", allSkins.Where(s => s.IsUnlocked).Select(s => s.Id));
        PlayerPrefs.SetString("UnlockedSkins", unlockedIds);
        PlayerPrefs.Save();
    }

    private void LoadSelectedSkins()
    {
        string json = PlayerPrefs.GetString("SelectedSkins", "{}");
        SelectedSkinsSaveData saveData = JsonUtility.FromJson<SelectedSkinsSaveData>(json);

        selectedSkinsByCategory.Clear();
        foreach (var entry in saveData.entries)
        {
            Skin skin = allSkins.Find(s => s.Id == entry.skinId);
            if (skin != null)
            {
                selectedSkinsByCategory[entry.category] = skin;
            }
        }
    }

    private void SaveSelectedSkins()
    {
        SelectedSkinsSaveData saveData = new SelectedSkinsSaveData();
        foreach (var pair in selectedSkinsByCategory)
        {
            saveData.entries.Add(new CategorySkinEntry
            {
                category = pair.Key,
                skinId = pair.Value.Id
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("SelectedSkins", json);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class SelectedSkinsSaveData
    {
        public List<CategorySkinEntry> entries = new List<CategorySkinEntry>();
    }

    [System.Serializable]
    private class CategorySkinEntry
    {
        public string category;
        public string skinId;
    }
}