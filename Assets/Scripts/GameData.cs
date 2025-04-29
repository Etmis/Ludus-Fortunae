using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public GameMode gameMode;
    public PlayerMode playerMode;
    public byte timeToAnswer;
    public bool showWarning;
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
    public int score;

    public PlayerData(int index, string name, bool isAlive, bool isSafe)
    {
        this.index = index;
        this.name = name;
        this.isAlive = isAlive;
        this.isSafe = isSafe;
        this.score = 0;
    }
    
    public void AddScore(int points)
    {
        score += points;
    }

    public void SubtractScore(int points)
    {
        score = Mathf.Max(0, score - points);
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

}
