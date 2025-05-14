using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkinButtonHandler : MonoBehaviour, IPointerClickHandler
{
    private Skin _skin;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("Button component missing!");
        }
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
            _button.interactable = _skin.IsUnlocked;
        }
    }

    // Metoda pro UI.Button onClick event
    public void OnButtonClick()
    {
        Debug.Log($"Button clicked: {_skin?.Id}");
        if (_skin != null && _skin.IsUnlocked)
        {
            InventoryUI.Instance?.OnSkinSelected(_skin);
        }
    }

    // Metoda pro IPointerClickHandler (fallback)
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Pointer click detected");
        OnButtonClick();
    }
}