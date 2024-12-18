using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settings;
    public GameObject gameMode;
    public GameObject playerMode;
    public GameObject players;
    public GameObject gameModeName1;
    public GameObject gameModeName2;

    public void OnPlayButtonClick()
    {
        mainMenu.SetActive(false);
        gameMode.SetActive(true);
    }

    public void OnGameModeBackButtonClick()
    {
        gameMode.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnSettingsButtonClick()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void OnSettingsBackButtonClick()
    {
        settings.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnStealOrNoStealButtonClick()
    {
        gameModeName1.GetComponent<TextMeshProUGUI>().text = "Steal Or No Steal";
        gameModeName2.GetComponent<TextMeshProUGUI>().text = "Steal Or No Steal";
        GameData.Instance.gameMode = GameMode.StealOrNoSteal;
        gameMode.SetActive(false);
        playerMode.SetActive(true);
    }

    public void OnTheFinalCaseButtonClick()
    {
        gameModeName1.GetComponent<TextMeshProUGUI>().text = "The Final Case";
        gameModeName2.GetComponent<TextMeshProUGUI>().text = "The Final Case";
        GameData.Instance.gameMode = GameMode.TheFinalCase;
        gameMode.SetActive(false);
        playerMode.SetActive(true);
    }

    public void OnPlayerModeBackButtonClick()
    {
        playerMode.SetActive(false);
        gameMode.SetActive(true);
    }

    public void OnPlayersBackButtonClick()
    {
        players.SetActive(false);
        playerMode.SetActive(true);
    }

    public void OnOneDeviceButtonClick()
    {
        GameData.Instance.playerMode = PlayerMode.OneDevice;
        playerMode.SetActive(false);
        players.SetActive(true);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
