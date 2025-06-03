using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StealOrNoSteal : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject endOfRoundModal, leaderboard, nextPlayerModal, warning, stealOrNoStealQuestionModal, summaryModal, briefcase;
    [SerializeField] private TextMeshProUGUI endOfRoundModalText, leaderboardText, playerNameText, warningText, briefcaseText;
    [SerializeField] private TextMeshProUGUI firstPlayerName, secondPlayerName, firstPlayerSummary, secondPlayerSummary;
    [SerializeField] private Animator animator, transition;
    [SerializeField] private ParticleSystem eliminated, safe, openEffect;
    [SerializeField] private Image timerCircle;
    [SerializeField] private TextMeshProUGUI firstPlayerScoreText, secondPlayerScoreText;

    [SerializeField] private ParticleSystem skinEffectElectricity;
    [SerializeField] private Material skinBriefcaseRed, skinBriefcaseBlue;

    private bool isConfirmTurnButtonClicked, isStealOrNoStealButtonClicked, isConfirmWarningButtonClicked, isSummaryConfirmButtonClicked, isEndOfRoundConfirmButtonClicked;
    private PlayerData lastPlayer;
    private PlayerData firstPlayer, secondPlayer;
    private int index = 0;
    private int round = 1;
    #endregion

    private async void Start()
    {
        Skin skinBriefcase = InventoryManager.Instance.GetSelectedSkinForCategory("briefcase");
        Skin skinEffect = InventoryManager.Instance.GetSelectedSkinForCategory("effect");

        Material selectedMaterial = null;

        if (skinBriefcase != null)
        {
            switch (skinBriefcase.Id)
            {
                case "0": selectedMaterial = null; break;
                case "1": selectedMaterial = skinBriefcaseRed; break;
                case "2": selectedMaterial = skinBriefcaseBlue; break;
                default: selectedMaterial = null; break;
            }
        }

        if (selectedMaterial != null)
        {
            foreach (Renderer renderer in briefcase.GetComponentsInChildren<Renderer>())
            {
                renderer.material = selectedMaterial;
            }
        }

        if (skinEffect != null)
        {
            switch (skinEffect.Id)
            {
                case "1000": openEffect = null; break;
                case "1001": openEffect = skinEffectElectricity; break;
                default: openEffect = null; break;
            }
        }

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

                await CrossfadeTransition();

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
                lastPlayer = GameData.Instance.players.Find(player => player.isAlive);
                leaderboard.SetActive(true);
                leaderboardText.text = "Last player standing: " + lastPlayer.name;
                break;
            }

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
        if (!GameData.Instance.showWarning)
        {
            return;
        }

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

    private static readonly System.Random random = new System.Random();

    private void GenerateBriefcases()
    {
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

        if (openEffect != null)
            openEffect.Play();

        await Task.Delay(3500);

        if (openEffect != null)
            openEffect.Stop();

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

        float elapsedTime = 0f;
        timerCircle.fillAmount = 1f;

        while (!isStealOrNoStealButtonClicked)
        {
            if (elapsedTime >= GameData.Instance.timeToAnswer)
            {
                briefcaseText.text = "ELIMINATED";
                OnStealButtonClick();
                Debug.Log($"Player {secondPlayer.name} did not respond in time and was automatically eliminated.");
                break;
            }

            timerCircle.fillAmount = 1f - (elapsedTime / GameData.Instance.timeToAnswer);

            await Task.Yield();
            elapsedTime += Time.deltaTime;
        }

        timerCircle.fillAmount = 0;
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
                lastPlayer.AddScore(10);
            }

            firstPlayerScoreText.text = $"Score: {lastPlayer.score}";
            secondPlayerScoreText.text = "";
        }
        else
        {
            PlayerData first = firstPlayer;
            PlayerData second = secondPlayer;

            firstPlayerName.text = first.name;
            secondPlayerName.text = second.name;

            Debug.Log(first.isAlive + ", " + first.isSafe);
            Debug.Log(second.isAlive + ", " + second.isSafe);

            if (!second.isAlive)
            {
                secondPlayerSummary.text = "ELIMINATED";
            }
            else if (second.isAlive && second.isSafe)
            {
                secondPlayerSummary.text = "SAFE";
                second.AddScore(10);
            }
            else if (second.isAlive && !second.isSafe)
            {
                secondPlayerSummary.text = "ANOTHER ROUND";
                if (index >= GameData.Instance.players.Count || first == null)
                {
                    lastPlayer = second;
                }
            }

            if (!first.isAlive)
            {
                firstPlayerSummary.text = "ELIMINATED";
                firstPlayer = second;
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
            else if (first.isAlive && first.isSafe)
            {
                firstPlayerSummary.text = "SAFE";
                first.AddScore(10);
                firstPlayer = second;
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
            else if (first.isAlive && !first.isSafe)
            {
                firstPlayerSummary.text = "ANOTHER ROUND";
                if (index >= GameData.Instance.players.Count || second == null)
                {
                    lastPlayer = first;
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

            firstPlayerScoreText.text = $"Score: {first.score}";
            secondPlayerScoreText.text = $"Score: {second.score}";
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

    private IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("MainMenu");
    }

    private async Task CrossfadeTransition()
    {
        transition.SetTrigger("Start");
        await Task.Delay(1000);
        transition.SetTrigger("End");
    }
}