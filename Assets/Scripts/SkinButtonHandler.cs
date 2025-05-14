using UnityEngine;
using UnityEngine.UI;

public class SkinButtonHandler : MonoBehaviour
{
    [SerializeField] private Image skinImage;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private GameObject selectionHighlight;

    public void Initialize(Skin skin, bool isSelected, UnityEngine.Events.UnityAction onClick)
    {
        skinImage.sprite = skin.Icon;
        lockOverlay.SetActive(!skin.IsUnlocked);
        selectionHighlight.SetActive(isSelected);

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skin.IsUnlocked)
            {
                onClick?.Invoke();
            }
            else
            {
                Debug.Log("Skin is locked!");
            }
        });
    }
}