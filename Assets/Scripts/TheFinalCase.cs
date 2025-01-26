using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheFinalCase : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject endOfRoundModal, leaderboard, nextPlayerModal, warning, votingModal;
    [SerializeField] TextMeshProUGUI endOfRoundModalText, leaderboardText, playerNameText, warningText, briefcaseText;
    [SerializeField] TMP_Dropdown playerDropdown;
    [SerializeField] Animator animator, transition;
    [SerializeField] ParticleSystem loser, winner;

    private bool isConfirmTurnButtonClicked, isStealOrNoStealButtonClicked, isConfirmWarningButtonClicked, isSummaryConfirmButtonClicked, isEndOfRoundConfirmButtonClicked, isConfirmVotingButtonClicked;
    private PlayerData lastPlayer;
    private List<string> briefcases;
    private Dictionary<string, int> voteCounts = new Dictionary<string, int>();
    private Dictionary<PlayerData, string> playerBriefcaseMap = new Dictionary<PlayerData, string>();
    private int round = 1;
    private int index = 0;
    #endregion

    private async void Start()
    {
        // potom smazat:
        //GameData.Instance = new GameData();
        //GameData.Instance.players.Add(new PlayerData(0, "name1", true, false));
        //GameData.Instance.players.Add(new PlayerData(1, "name2", true, false));
        //GameData.Instance.players.Add(new PlayerData(2, "name3", true, false));
        //GameData.Instance.players.Add(new PlayerData(3, "name4", true, false));

        while (true)
        {
            index = 0;
            if (GameData.Instance.players.Count(player => player.isAlive).Equals(1))
            {
                PlayerData winnerPlayer = GameData.Instance.players.First(player => player.isAlive);
                Debug.Log(winnerPlayer.name + " wins!");
                leaderboard.SetActive(true);
                leaderboardText.text = "Winner: " + winnerPlayer.name;
                winner.Play();
                return;
            }

            GenerateBriefcases();

            if (GameData.Instance.players.Count(player => player.isAlive).Equals(2))
            {
                foreach (PlayerData player in GameData.Instance.players.Where(p => p.isAlive))
                {
                    Debug.Log(player.name);
                    Debug.Log(player.isAlive);
                    await ShowNextPlayer(player);
                    //await ShowWarning(player);
                    await ShowBriefcase();
                }

                PlayerData winnerPlayer = GameData.Instance.players.FirstOrDefault(p => playerBriefcaseMap.ContainsKey(p) && playerBriefcaseMap[p] == "WINNER" && p.isAlive);

                if (winnerPlayer != null)
                {
                    Debug.Log(winnerPlayer.name + " wins!");
                    leaderboard.SetActive(true);
                    leaderboardText.text = "Winner: " + winnerPlayer.name;
                    winner.Play();
                    return;
                }
            }

            foreach (PlayerData player in GameData.Instance.players.Where(p => p.isAlive))
            {
                Debug.Log(player.name);
                Debug.Log(player.isAlive);
                await ShowNextPlayer(player);
                await ShowWarning(player);
                await ShowBriefcase();
            }

            await Voting();
            await ShowEndOfRoundModal();
            round++;
        }
    }

    private async Task ShowNextPlayer(PlayerData player)
    {
        isConfirmTurnButtonClicked = false;
        if (lastPlayer != null)
        {
            playerNameText.text = lastPlayer.name + " (last player)";
        }
        else
        {
            playerNameText.text = player.name;
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

    private async Task ShowWarning(PlayerData player)
    {
        isConfirmWarningButtonClicked = false;
        warningText.text = "Only " + player.name + " can look at the screen now!";
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
        briefcases = new List<string> { "WINNER" };
        int alivePlayerCount = GameData.Instance.players.Count(player => player.isAlive);
        for (int i = 1; i < alivePlayerCount; i++)
        {
            briefcases.Add("LOSER");
        }

        System.Random random = new System.Random();
        briefcases = briefcases.OrderBy(_ => random.Next()).ToList();

        List<PlayerData> alivePlayers = GameData.Instance.players.Where(player => player.isAlive).ToList();
        playerBriefcaseMap = new Dictionary<PlayerData, string>();

        for (int i = 0; i < alivePlayers.Count; i++)
        {
            var player = alivePlayers[i];
            playerBriefcaseMap[player] = briefcases[i];
        }
    }



    private async Task ShowBriefcase()
    {
        briefcaseText.text = briefcases[index];
        index++;

        animator.SetTrigger("OpenBriefcase Trigger");

        await Task.Delay(3500);

        if (briefcaseText.text.Equals("LOSER"))
        {
            loser.Play();
        }
        else if (briefcaseText.text.Equals("WINNER"))
        {
            winner.Play();
        }

        await Task.Delay(1500);

        animator.SetTrigger("CloseBriefcase Trigger");

        await Task.Delay(3500);
    }

    private async Task Voting()
    {
        voteCounts.Clear();
        foreach (PlayerData player in GameData.Instance.players.Where(p => p.isAlive))
        {
            await ShowNextPlayer(player);
            await AskPlayer(player);
        }

        PlayerData playerToEliminate = ProcessVotes();
        if (playerToEliminate != null)
        {
            if (playerBriefcaseMap[playerToEliminate] == "WINNER" && playerToEliminate.isAlive)
            {
                Debug.Log(playerToEliminate.name + " wins!");
                leaderboard.SetActive(true);
                leaderboardText.text = "Winner: " + playerToEliminate.name;
                winner.Play();
                return;
            }
            else
            {
                playerToEliminate.isAlive = false;
                Debug.Log(playerToEliminate.name + " was eliminated.");
            }
        }
    }

    private async Task AskPlayer(PlayerData player)
    {
        playerDropdown.ClearOptions();
        playerDropdown.AddOptions(GameData.Instance.players.Where(p => p.isAlive).Select(p => p.name).ToList());

        isConfirmVotingButtonClicked = false;
        votingModal.SetActive(true);

        float timeout = 10f; // jak moc má hráè èasu na odpovìï (v sekundách)
        float elapsedTime = 0f;

        while (!isConfirmVotingButtonClicked && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        votingModal.SetActive(false);

        if (!isConfirmVotingButtonClicked)
        {
            Debug.Log(player.name + " did not vote in time. They are eliminated.");
            player.isAlive = false;

            if (playerBriefcaseMap[player] == "WINNER")
            {
                Debug.Log("The winner's briefcase was held by " + player.name + ". Restarting round.");
                return;
            }

            return;
        }

        string selectedPlayerName = playerDropdown.options[playerDropdown.value].text;

        if (!voteCounts.ContainsKey(selectedPlayerName))
        {
            voteCounts[selectedPlayerName] = 0;
        }

        voteCounts[selectedPlayerName]++;
    }

    public void OnConfirmVotingButtonClick()
    {
        isConfirmVotingButtonClicked = true;
    }

    private PlayerData ProcessVotes()
    {
        var playerWithMostVotes = voteCounts.OrderByDescending(v => v.Value).FirstOrDefault();
        if (playerWithMostVotes.Value > 0)
        {
            return GameData.Instance.players.First(p => p.name == playerWithMostVotes.Key);
        }

        return null;
    }
    public void OnLeaderboardConfirmButtonClick()
    {
        GameData.Instance.players.Clear();
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

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
}
