using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoadoutSlot : MonoBehaviour, IPointerClickHandler
{
    public string category;
    private Image skinIcon;

    public void UpdateSlot()
    {
        Skin selectedSkin = InventoryManager.Instance.GetSelectedSkinForCategory(category);
        bool hasSkin = selectedSkin != null && selectedSkin.Icon != null;

        if (hasSkin)
        {
            skinIcon = GetComponent<Image>();
            skinIcon.sprite = selectedSkin.Icon;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryUI.Instance.ShowCategory(category);
    }
}