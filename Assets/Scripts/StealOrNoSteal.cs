using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StealOrNoSteal : MonoBehaviour
{
    #region SerializeFields
    [SerializeField]
    GameObject endOfRoundModal;
    [SerializeField]
    TextMeshProUGUI endOfRoundModalText;
    [SerializeField]
    GameObject leaderboard;
    [SerializeField]
    TextMeshProUGUI leaderboardText;
    [SerializeField]
    TextMeshProUGUI playerNameText;
    [SerializeField]
    GameObject nextPlayerModal;
    [SerializeField]
    GameObject warning;
    [SerializeField]
    TextMeshProUGUI warningText;
    [SerializeField]
    TextMeshProUGUI briefcaseText;
    [SerializeField]
    Animator animator;
    [SerializeField]
    GameObject stealOrNoStealQuestionModal;
    [SerializeField]
    GameObject summaryModal;
    [SerializeField]
    TextMeshProUGUI firstPlayerName;
    [SerializeField]
    TextMeshProUGUI secondPlayerName;
    [SerializeField]
    TextMeshProUGUI firstPlayerSummary;
    [SerializeField]
    TextMeshProUGUI secondPlayerSummary;
    #endregion

    bool isConfirmTurnButtonClicked = false;
    bool isStealOrNoStealButtonClicked = false;
    bool isConfirmWarningButtonClicked = false;
    bool isSummaryConfirmButtonClicked = false;
    bool isEndOfRoundConfirmButtonClicked = false;

    PlayerData lastPlayer = null;

    int index = 0;
    int round = 1;
    PlayerData firstPlayer;
    PlayerData secondPlayer;

    private async void Start()
    {
        // potom smazat:
        //GameManager.Instance = new GameManager();
        //GameManager.Instance.players.Add(new PlayerData(0, "name1", true, false, 0));
        //GameManager.Instance.players.Add(new PlayerData(1, "name2", true, false, 0));
        //GameManager.Instance.players.Add(new PlayerData(2, "name3", true, false, 0));
        //GameManager.Instance.players.Add(new PlayerData(3, "name4", true, false, 0));

        firstPlayer = GameManager.Instance.players[index];
        index++;
        secondPlayer = GameManager.Instance.players[index];
        index++;

        while (true)
        {
            if (lastPlayer != null)
            {
                if (GameManager.Instance.players.Count(player => player.isAlive).Equals(1))
                {
                    leaderboard.SetActive(true);
                    leaderboardText.text = "Last player standing: " + lastPlayer.name;
                    Debug.Log("Last player standing: " + lastPlayer.name);
                    Time.timeScale = 0;
                    break;
                }
                await ShowNextPlayers();
                GenerateStealOrNoSteal();
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
                foreach (var player in GameManager.Instance.players)
                {
                    player.isSafe = false;
                }
                lastPlayer = null;
                firstPlayer = null;
                secondPlayer = null;
                index = 0;
                round++;
                while (index < GameManager.Instance.players.Count)
                {
                    if (GameManager.Instance.players[index].isAlive)
                    {
                        firstPlayer = GameManager.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }

                while (index < GameManager.Instance.players.Count)
                {
                    if (GameManager.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameManager.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }
            }
            Debug.Log("Current Players: " + firstPlayer.name + " vs " + secondPlayer.name);

            await ShowNextPlayers();
            await ShowWarning();
            GenerateStealOrNoSteal();
            await ShowBriefcase();
            await StealOrNoStealQuestion();
            await Summary();

            if (GameManager.Instance.players.Count(player => player.isAlive).Equals(1))
            {
                lastPlayer = GameManager.Instance.players.First(player => player.isAlive);
            }

            Debug.Log("DONE");
        }
    }

    public void OnLeaderboardConfirmButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
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
        isConfirmWarningButtonClicked = true;
    }

    public void OnConfirmWarningButtonClick()
    {
        isConfirmWarningButtonClicked = true;
    }

    private void GenerateStealOrNoSteal()
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
        var animationCompletion = new TaskCompletionSource<bool>();

        animator.SetTrigger("OpenBriefcase Trigger");

        await Task.Delay(8000);

        animator.SetTrigger("CloseBriefcase Trigger");

        await Task.Delay(4000);

        Debug.Log("Animace skonèila!");

        animationCompletion.SetResult(true);

        await animationCompletion.Task;
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
                if (index >= GameManager.Instance.players.Count || firstPlayer.Equals(null))
                {
                    lastPlayer = secondPlayer;
                }
            }

            if (!firstPlayer.isAlive)
            {
                firstPlayerSummary.text = "ELIMINATED";
                firstPlayer = secondPlayer;
                while (index < GameManager.Instance.players.Count)
                {
                    if (GameManager.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameManager.Instance.players[index];
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
                while (index < GameManager.Instance.players.Count)
                {
                    if (GameManager.Instance.players[index].isAlive)
                    {
                        secondPlayer = GameManager.Instance.players[index];
                        index++;
                        break;
                    }
                    index++;
                }
            }
            else if (firstPlayer.isAlive && !firstPlayer.isSafe)
            {
                firstPlayerSummary.text = "ANOTHER ROUND";
                if (index >= GameManager.Instance.players.Count || secondPlayer.Equals(null))
                {
                    lastPlayer = firstPlayer;
                }
                else
                {
                    while (index < GameManager.Instance.players.Count)
                    {
                        if (GameManager.Instance.players[index].isAlive)
                        {
                            secondPlayer = GameManager.Instance.players[index];
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
}
