using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] Animator transition;

    public void OnStartGameButtonClick()
    {
        if (GameData.Instance.gameMode.Equals(GameMode.StealOrNoSteal))
        {
            if (GameData.Instance.players.Count >= 2)
                StartCoroutine(LoadLevel("StealOrNoSteal"));
        }
        else if (GameData.Instance.gameMode.Equals(GameMode.TheFinalCase))
        {
            if (GameData.Instance.players.Count >= 3)
            {
                StartCoroutine(LoadLevel("TheFinalCase"));
            }
        }
    }

    IEnumerator LoadLevel(string level)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(level);
    }
}
