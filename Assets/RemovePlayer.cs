using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePlayer : MonoBehaviour
{
    public void OnRemovePlayerButtonClick()
    {
        Destroy(gameObject);
    }
}
