using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExit : MonoBehaviour
{
    /// <summary>
    /// Move the player to this exit point
    /// </summary>
    /// <param name="player">Player transform to move</param>
    public void MoveToPoint(Transform player)
    {
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
    }
}
