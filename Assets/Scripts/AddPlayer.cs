using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddPlayer : MonoBehaviour
{
    public GameObject addPlayerModal;
    public Button addPlayerButton;
    public Button confirmAddPlayerButton;
    public TMP_InputField nameInputField;
    public Transform playerListPanel;
    public GameObject errorModal;
    public Button startGameButton;
    public Button gameModeSettingsBackButton;
    public GameObject playerPrefab;

    public void OnAddPlayerButtonClick()
    {
        if (playerListPanel.childCount >= 8)
        {
            StartCoroutine(HideWarningAfterDelay(2));
            return;
        }

        gameModeSettingsBackButton.interactable = false;
        startGameButton.interactable = false;
        addPlayerButton.interactable = false;
        addPlayerModal.SetActive(true);
    }

    public void OnAddPlayerCloseButtonClick()
    {
        addPlayerModal.SetActive(false);
        gameModeSettingsBackButton.interactable = true;
        startGameButton.interactable = true;
        addPlayerButton.interactable = true;
    }

    public void OnConfirmAddPlayerButtonClick()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            GameObject player = Instantiate(playerPrefab, playerListPanel);
            player.GetComponentInChildren<TextMeshProUGUI>().text = playerName;

            int playerIndex = GameManager.Instance.players.Count;
            GameManager.Instance.players.Add(new PlayerData(playerIndex, playerName, true, false, 0));
            player.GetComponent<RemovePlayer>().index = playerIndex;

            nameInputField.text = "";

            addPlayerModal.SetActive(false);
            gameModeSettingsBackButton.interactable = true;
            startGameButton.interactable = true;
            addPlayerButton.interactable = true;
        }
    }

    private IEnumerator HideWarningAfterDelay(float delay)
    {
        addPlayerButton.interactable = false;
        errorModal.SetActive(true);
        yield return new WaitForSeconds(delay);
        errorModal.SetActive(false);
        addPlayerButton.interactable = true;
    }
}
