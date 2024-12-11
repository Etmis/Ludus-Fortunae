using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDevice : MonoBehaviour
{
    public GameObject mode;
    public GameObject players;

    public void OnOneDeviceButtonClick()
    {
        GameData.Instance.playerMode = PlayerMode.OneDevice;
        mode.SetActive(false);
        players.SetActive(true);
    }
}
