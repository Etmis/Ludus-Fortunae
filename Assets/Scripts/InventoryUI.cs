using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("References")]
    [SerializeField] private Transform skinGridParent;
    [SerializeField] private GameObject skinButtonPrefab;
    [SerializeField] private Transform loadoutSlotsParent;
    [SerializeField] private GameObject loadoutSlotPrefab;

    [Header("Loadout Slots")]
    [SerializeField] private LoadoutSlot[] loadoutSlots;

    private string currentCategory;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeLoadoutSlots();
        RefreshAllCategories();
    }

    public void RefreshAllCategories()
    {
        if (InventoryManager.Instance.allSkins.Count == 0)
        {
            Debug.LogWarning("No skins available in InventoryManager");
            return;
        }

        string firstCategory = InventoryManager.Instance.allSkins[0].Category;
        if (!string.IsNullOrEmpty(firstCategory))
        {
            ShowCategory(firstCategory);
        }
    }

    public void ShowCategory(string category)
    {
        currentCategory = category;
        ClearSkinGrid();

        foreach (Skin skin in InventoryManager.Instance.GetSkinsInCategory(category))
        {
            CreateSkinButton(skin);
        }
    }

    private void CreateSkinButton(Skin skin)
    {
        GameObject buttonObj = Instantiate(skinButtonPrefab, skinGridParent);
        SkinButtonHandler handler = buttonObj.GetComponent<SkinButtonHandler>();
        handler.Initialize(skin);
    }

    public void RefreshCurrentCategory()
    {
        ShowCategory(currentCategory);
    }

    private void InitializeLoadoutSlots()
    {
        foreach (LoadoutSlot slot in loadoutSlots)
        {
            slot.UpdateSlot();
        }
    }

    public void OnSkinSelected(Skin skin)
    {
        if (InventoryManager.Instance.SelectSkin(skin.Id))
        {
            Debug.Log($"Updating slot for category: {skin.Category}");
            LoadoutSlot slot = System.Array.Find(loadoutSlots, s => s.category == skin.Category);
            if (slot != null)
            {
                slot.UpdateSlot();
            }
            else
            {
                Debug.LogError($"No slot found for category: {skin.Category}");
            }
        }
    }

    private void ClearSkinGrid()
    {
        foreach (Transform child in skinGridParent)
        {
            Destroy(child.gameObject);
        }
    }
}