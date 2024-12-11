using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlayer : MonoBehaviour
{
    public int index;

    public void OnRemovePlayerButtonClick()
    {
        GameData.Instance.players.RemoveAt(index);
        Destroy(gameObject);
        for (int i = 0; i < GameData.Instance.players.Count; i++)
        {
            GameData.Instance.players[i].index = i;
        }
    }
}
