using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadToGameplay : MonoBehaviour
{
    // TODO - link up with the UI screen instead
    public void MoveToGameplay()
    {
        GameManager.instance.GoToMainGame();
    }
}
