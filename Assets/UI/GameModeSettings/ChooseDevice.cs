using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDevice : MonoBehaviour
{
    public GameObject mode;
    public GameObject players;

    public void On1DeviceButtonClick()
    {
        mode.SetActive(false);
        players.SetActive(true);
    }
}
