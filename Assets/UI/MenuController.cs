using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject gameModeMenu;
    public GameObject gameModeSettingsMenu;
    public GameObject mode;
    public GameObject players;
    public GameObject gameModeName;

    public void OnPlayButtonClick()
    {
        mainMenu.SetActive(false);
        gameModeMenu.SetActive(true);
    }

    public void OnGameModeBackButtonClick()
    {
        gameModeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnSettingsButtonClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OnSettingsBackButtonClick()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnStealOrNoStealButtonClick()
    {
        gameModeName.GetComponent<TextMeshProUGUI>().text = "Steal Or No Steal";
        gameModeMenu.SetActive(false);
        gameModeSettingsMenu.SetActive(true);
    }

    public void OnGameModeSettingsBackButtonClick()
    {
        if (players.activeSelf && !mode.activeSelf)
        {
            players.SetActive(false);
            mode.SetActive(true);
        }
        else if (!players.activeSelf && mode.activeSelf)
        {
            gameModeSettingsMenu.SetActive(false);
            gameModeMenu.SetActive(true);
        }
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
