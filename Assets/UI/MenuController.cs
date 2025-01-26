using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region Variables
    public GameObject mainMenu;
    public GameObject settings;
    public GameObject gameMode;
    public GameObject playerMode;
    public GameObject players;
    public GameObject gameModeName1;
    public GameObject gameModeName2;
    public GameObject gameModeText;

    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    #endregion

    private void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusic("MainMenu");
    }

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
        gameModeText.GetComponent<TextMeshProUGUI>().text = "One player receives a briefcase. The briefcase contains either \"ELIMINATED\" or \"SAFE\", and only the player holding the briefcase can see its contents.\r\nThe second player must decide whether to steal the briefcase or leave it with the other player.\r\nThe player with the briefcase can bluff, persuade, or tell the truth to influence the other player's decision.\r\nThe second player chooses either \"Steal\" or \"No Steal\".\r\nIf the player \"steals\" the briefcase, they take on its contents. If the briefcase contains \"Elimination\", they are removed from the game. If it contains \"Survival\", they continue playing.\r\nIf the player \"leaves the briefcase\", the outcome depends on the briefcase’s contents, which remain with the original player.";
        GameData.Instance.gameMode = GameMode.StealOrNoSteal;
        gameMode.SetActive(false);
        playerMode.SetActive(true);
    }

    public void OnTheFinalCaseButtonClick()
    {
        gameModeName1.GetComponent<TextMeshProUGUI>().text = "The Final Case";
        gameModeName2.GetComponent<TextMeshProUGUI>().text = "The Final Case";
        gameModeText.GetComponent<TextMeshProUGUI>().text = "Each player is given a briefcase. Only one briefcase contains the prize, while the rest are empty.\r\nAll players open their briefcases simultaneously, but no one reveals its contents to the others.\r\nPlayers discuss among themselves to figure out who has the prize. Bluffing, deception, and persuasion are all allowed.\r\nAfter the discussion, all players vote to eliminate one player who they believe does not have the prize.\r\nIf the player with the prize is eliminated, they win the game.\r\nIf the player with the prize remains, the game continues, and the rest must reassess their strategy.";
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

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
