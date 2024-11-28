using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlayer : MonoBehaviour
{
    public int index;

    public void OnRemovePlayerButtonClick()
    {
        GameManager.Instance.players.RemoveAt(index);
        Destroy(gameObject);
        for (int i = 0; i < GameManager.Instance.players.Count; i++)
        {
            GameManager.Instance.players[i].index = i;
        }
    }
}
