using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameMode gameMode;
    public PlayerMode playerMode;
    public List<PlayerData> players = new List<PlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int index;
    public string name;
    public bool isAlive;
    public bool isSafe;

    public PlayerData(int index, string name, bool isAlive, bool isSafe)
    {
        this.index = index;
        this.name = name;
        this.isAlive = isAlive;
        this.isSafe = isSafe;
    }
}

public enum GameMode
{
    StealOrNoSteal,
    TheFinalCase,

}

public enum PlayerMode
{
    OneDevice,
    MultipleDevices,

}
