using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDevice : MonoBehaviour
{
    public GameObject mode;
    public GameObject players;

    public void OnOneDeviceButtonClick()
    {
        GameManager.Instance.mode = Mode.OneDevice;
        mode.SetActive(false);
        players.SetActive(true);
    }
}
