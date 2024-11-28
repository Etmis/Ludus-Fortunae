using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void OnStartGameButtonClick()
    {
        if (GameManager.Instance.gameMode.Equals(GameMode.StealOrNoSteal))
        {
            SceneManager.LoadScene(GameMode.StealOrNoSteal.ToString());
            foreach (var player in GameManager.Instance.players)
            {
                Debug.Log($"{player.index}, {player.name}, {player.isAlive}, {player.score}\n{GameManager.Instance.gameMode}, {GameManager.Instance.mode}");
            }
        }
    }
}
