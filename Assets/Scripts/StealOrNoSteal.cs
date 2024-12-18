using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StealOrNoSteal : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject endOfRoundModal, leaderboard, nextPlayerModal, warning, stealOrNoStealQuestionModal, summaryModal;
    [SerializeField] TextMeshProUGUI endOfRoundModalText, leaderboardText, playerNameText, warningText, briefcaseText;
    [SerializeField] TextMeshProUGUI firstPlayerName, secondPlayerName, firstPlayerSummary, secondPlayerSummary;
    [SerializeField] Animator animator, transition;
    [SerializeField] ParticleSystem eliminated, safe;

    private bool isConfirmTurnButtonClicked, isStealOrNoStealButtonClicked, isConfirmWarningButtonClicked, isSummaryConfirmButtonClicked, isEndOfRoundConfirmButtonClicked;
    private PlayerData lastPlayer;
    private PlayerData firstPlayer, secondPlayer;
    private int index = 0;
    private int round = 1;
    #endregion

    private async void Start()
    {
        // potom smazat:
        //GameData.Instance = new GameData();
        //GameData.Instance.players.Add(new PlayerData(0, "name1", true, false));
        //GameData.Instance.players.Add(new PlayerData(1, "name2", true, false));
        //GameData.Instance.players.Add(new PlayerData(2, "name3", true, false));
        //GameData.Instance.players.Add(new PlayerData(3, "name4", true, false));

        firstPlayer = GameData.Instance.players[index];
        index++;
        secondPlayer = GameData.Instance.players[index];
        index++;

        while (true)
        {
            if (lastPlayer != null)
            {
                if (GameData.Instance.players.Count(player => player.isAlive).Equals(1))
                {
                    leaderboard.SetActive(true);
                    leaderboardText.text = "Last player standing: " + lastPlayer.name;
                    safe.Play();
                    Debug.Log("Last player standing: " + lastPlayer.name);
                    break;
                }
                await ShowNextPlayers();
                GenerateBriefcases();
                await ShowBriefcase();
                if (briefcaseText.text.Equals("ELIMINATED"))
                {
                    lastPlayer.isAlive = false;
                }
                else if (briefcaseText.text.Equals("SAFE"))
                {
                    lastPlayer.isSafe = true;
                }
                await Summary();
                await ShowEndOfRoundModal();
                foreach (var player in GameData.Instance.players)
                {
                    player.isSafe = false;
                }
                lastPlayer = null;
                firstPlayer = null;
                secondPlayer = null;
                index = 0;
                round++;
                while (index < GameData.Instance.players.Count)
                {
                    if (GameData.Instance.players[index].isAlive)
                    {
                        firstPlayer = GameData.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }

                while (index < GameData.Instance.players.Count)
                {
                    if (GameData.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameData.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }
            }
            if (GameData.Instance.players.Count(player => player.isAlive).Equals(1))
            {
                leaderboard.SetActive(true);
                leaderboardText.text = "Last player standing: " + lastPlayer.name;
                Debug.Log("Last player standing: " + lastPlayer.name);
                break;
            }
            Debug.Log("Current Players: " + firstPlayer.name + " vs " + secondPlayer.name);

            await ShowNextPlayers();
            await ShowWarning();
            GenerateBriefcases();
            await ShowBriefcase();
            await StealOrNoStealQuestion();
            await Summary();

            if (GameData.Instance.players.Count(player => player.isAlive).Equals(1))
            {
                lastPlayer = GameData.Instance.players.First(player => player.isAlive);
            }

            Debug.Log("DONE");
        }
    }

    public void OnLeaderboardConfirmButtonClick()
    {
        GameData.Instance.players.Clear();
        StartCoroutine(LoadLevel());
    }

    private async Task ShowEndOfRoundModal()
    {
        isEndOfRoundConfirmButtonClicked = false;
        endOfRoundModalText.text = "End of round " + round;
        endOfRoundModal.SetActive(true);

        while (!isEndOfRoundConfirmButtonClicked)
        {
            await Task.Yield();
        }

        endOfRoundModal.SetActive(false);
    }

    public void OnConfirmEndOfRoundButtonClick()
    {
        isEndOfRoundConfirmButtonClicked = true;
    }

    private async Task ShowNextPlayers()
    {
        isConfirmTurnButtonClicked = false;
        if (lastPlayer != null)
        {
            playerNameText.text = lastPlayer.name + " (last player)";
        }
        else
        {
            playerNameText.text = firstPlayer.name + " vs " + secondPlayer.name;
        }
        nextPlayerModal.SetActive(true);

        while (!isConfirmTurnButtonClicked)
        {
            await Task.Yield();
        }

        nextPlayerModal.SetActive(false);
    }

    public void OnConfirmTurnButtonClick()
    {
        isConfirmTurnButtonClicked = true;
    }

    private async Task ShowWarning()
    {
        isConfirmWarningButtonClicked = false;
        warningText.text = "Only " + firstPlayer.name + " can look at the screen now!";
        warning.SetActive(true);
        while (!isConfirmWarningButtonClicked)
        {
            await Task.Yield();
        }
        warning.SetActive(false);
    }

    public void OnConfirmWarningButtonClick()
    {
        isConfirmWarningButtonClicked = true;
    }

    private void GenerateBriefcases()
    {
        System.Random random = new System.Random();
        if (random.Next(0, 2) == 1)
        {
            briefcaseText.text = "ELIMINATED";
        }
        else
        {
            briefcaseText.text = "SAFE";
        }
    }

    private async Task ShowBriefcase()
    {
        animator.SetTrigger("OpenBriefcase Trigger");

        await Task.Delay(3500);

        if (briefcaseText.text.Equals("ELIMINATED"))
        {
            eliminated.Play();
        }
        else if (briefcaseText.text.Equals("SAFE"))
        {
            safe.Play();
        }

        await Task.Delay(1500);

        animator.SetTrigger("CloseBriefcase Trigger");

        await Task.Delay(3500);
    }

    private async Task StealOrNoStealQuestion()
    {
        isStealOrNoStealButtonClicked = false;
        stealOrNoStealQuestionModal.SetActive(true);

        while (!isStealOrNoStealButtonClicked)
        {
            await Task.Yield();
        }
        stealOrNoStealQuestionModal.SetActive(false);
    }

    public void OnStealButtonClick()
    {
        if (briefcaseText.text.Equals("ELIMINATED"))
        {
            secondPlayer.isAlive = false;
        }
        else if (briefcaseText.text.Equals("SAFE"))
        {
            secondPlayer.isSafe = true;
        }
        isStealOrNoStealButtonClicked = true;
    }

    public void OnNoStealButtonClick()
    {
        if (briefcaseText.text.Equals("ELIMINATED"))
        {
            firstPlayer.isAlive = false;
        }
        else if (briefcaseText.text.Equals("SAFE"))
        {
            firstPlayer.isSafe = true;
        }
        isStealOrNoStealButtonClicked = true;
    }

    private async Task Summary()
    {
        isSummaryConfirmButtonClicked = false;

        if (lastPlayer != null)
        {
            firstPlayerName.text = lastPlayer.name;
            secondPlayerName.text = "";
            secondPlayerSummary.text = "";

            if (!lastPlayer.isAlive)
            {
                firstPlayerSummary.text = "ELIMINATED";
            }
            else if (lastPlayer.isAlive && lastPlayer.isSafe)
            {
                firstPlayerSummary.text = "SAFE";
            }
        }
        else
        {
            firstPlayerName.text = firstPlayer.name;
            secondPlayerName.text = secondPlayer.name;

            Debug.Log(firstPlayer.isAlive + ", " + firstPlayer.isSafe);
            Debug.Log(secondPlayer.isAlive + ", " + secondPlayer.isSafe);

            if (!secondPlayer.isAlive)
            {
                secondPlayerSummary.text = "ELIMINATED";
            }
            else if (secondPlayer.isAlive && secondPlayer.isSafe)
            {
                secondPlayerSummary.text = "SAFE";
            }
            else if (secondPlayer.isAlive && !secondPlayer.isSafe)
            {
                secondPlayerSummary.text = "ANOTHER ROUND";
                if (index >= GameData.Instance.players.Count || firstPlayer.Equals(null))
                {
                    lastPlayer = secondPlayer;
                }
            }

            if (!firstPlayer.isAlive)
            {
                firstPlayerSummary.text = "ELIMINATED";
                firstPlayer = secondPlayer;
                while (index < GameData.Instance.players.Count)
                {
                    if (GameData.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameData.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }
            }
            else if (firstPlayer.isAlive && firstPlayer.isSafe)
            {
                firstPlayerSummary.text = "SAFE";
                firstPlayer = secondPlayer;
                while (index < GameData.Instance.players.Count)
                {
                    if (GameData.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameData.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }
            }
            else if (firstPlayer.isAlive && !firstPlayer.isSafe)
            {
                firstPlayerSummary.text = "ANOTHER ROUND";
                if (index >= GameData.Instance.players.Count || secondPlayer.Equals(null))
                {
                    lastPlayer = firstPlayer;
                }
                else
                {
                    while (index < GameData.Instance.players.Count)
                    {
                        if (GameData.Instance.players[index].isAlive)
                        {
                            secondPlayer = GameData.Instance.players[index];
                            index++;
                            break;
                        }
                        index++;
                    }
                }
            }
        }

        summaryModal.SetActive(true);
        while (!isSummaryConfirmButtonClicked)
        {
            await Task.Yield();
        }
        summaryModal.SetActive(false);
    }

    public void OnSummaryConfirmButtonClick()
    {
        isSummaryConfirmButtonClicked = true;
    }

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("MainMenu");
    }
}
