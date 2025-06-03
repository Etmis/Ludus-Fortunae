using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Advertisements;

public class SkinButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private Skin _skin;
    private Button _button;
    private Image _lockIcon; // Add a reference to a lock icon if you have one

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("Button component missing!");
        }
        
        // If you have a lock icon child object
        _lockIcon = transform.Find("LockIcon")?.GetComponent<Image>();
    }

    public void Initialize(Skin skin)
    {
        _skin = skin;

        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.sprite = _skin.Icon;
        }
        else
        {
            Debug.LogError("Image component missing!");
        }

        if (_button != null)
        {
            _button.interactable = true; // Always make it interactable
        }

        // Show/hide lock icon based on skin status
        if (_lockIcon != null)
        {
            _lockIcon.gameObject.SetActive(!_skin.IsUnlocked);
        }
    }

    public void OnButtonClick()
    {
        Debug.Log($"Button clicked: {_skin?.Id}");
        if (_skin == null) return;
        
        if (_skin.IsUnlocked)
        {
            // Select the skin if it's already unlocked
            InventoryUI.Instance?.OnSkinSelected(_skin);
        }
        else
        {
            // Show ad to unlock the skin
            ShowUnlockAd();
        }
    }

    private void ShowUnlockAd()
    {
        // Get the RewardAd instance (you might need to adjust this based on your setup)
        RewardAd rewardAd = FindObjectOfType<RewardAd>();
        if (rewardAd != null)
        {
            // Set up the callback for when the ad completes
            rewardAd.SetSkinToUnlock(_skin.Id);
            rewardAd.ShowAd();
        }
        else
        {
            Debug.LogError("RewardAd component not found!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer click detected");
        OnButtonClick();
    }
}