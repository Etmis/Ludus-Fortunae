using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform categoryButtonsParent;
    [SerializeField] private Transform skinGridParent;
    [SerializeField] private Transform loadoutSlotsParent;
    [SerializeField] private GameObject skinButtonPrefab;
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject loadoutSlotPrefab;
    [SerializeField] private Image selectedSkinImage;
    [SerializeField] private Text selectedSkinName;

    private string currentCategory;
    private Skin selectedSkin;

    private void Start()
    {
        InitializeCategories();
        RefreshLoadout();
    }

    private void InitializeCategories()
    {
        // Získání unikátních kategorií
        HashSet<string> categories = new HashSet<string>();
        foreach (Skin skin in InventoryManager.Instance.allSkins)
        {
            categories.Add(skin.Category);
        }

        // Vytvoøení tlaèítek pro kategorie
        foreach (string category in categories)
        {
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonsParent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = category;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => ShowCategory(category));
        }

        if (categories.Count > 0)
        {
            ShowCategory(new List<string>(categories)[0]);
        }
    }

    public void ShowCategory(string category)
    {
        currentCategory = category;
        ClearSkinGrid();

        List<Skin> skins = InventoryManager.Instance.GetSkinsInCategory(category);
        Skin selected = InventoryManager.Instance.GetSelectedSkinForCategory(category);

        foreach (Skin skin in skins)
        {
            GameObject skinButton = Instantiate(skinButtonPrefab, skinGridParent);
            SkinButtonHandler handler = skinButton.GetComponent<SkinButtonHandler>();

            handler.Initialize(skin, skin == selected, () => OnSkinSelected(skin));
        }
    }

    private void OnSkinSelected(Skin skin)
    {
        selectedSkin = skin;
        selectedSkinImage.sprite = skin.Icon;
        selectedSkinName.text = skin.Name;
    }

    public void ConfirmSelection()
    {
        if (selectedSkin != null)
        {
            InventoryManager.Instance.SelectSkin(selectedSkin.Id);
            ShowCategory(currentCategory); // Refresh grid
            RefreshLoadout();
        }
    }

    private void RefreshLoadout()
    {
        foreach (Transform child in loadoutSlotsParent)
        {
            Destroy(child.gameObject);
        }

        var selectedSkins = InventoryManager.Instance.GetSelectedSkins();
        foreach (var pair in selectedSkins)
        {
            GameObject slot = Instantiate(loadoutSlotPrefab, loadoutSlotsParent);
            slot.GetComponentInChildren<Text>().text = pair.Key;
            slot.GetComponent<Image>().sprite = pair.Value.Icon;
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