/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 14st, 2022
 * Last Edited - November 14th, 2022 by Ben Schuster
 * Description - Trigger in the hallway that triggers cross-room events.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayEventTrigger : MonoBehaviour
{
    private bool triggered;

    /// <summary>
    /// On trigger, tell the map loader to update
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(!triggered && other.tag == "Player")
        {
            triggered = true;

            MapLoader.instance.UpdateLoadedSegments();

            GetComponent<Collider>().enabled = false;
        }
    }
}
