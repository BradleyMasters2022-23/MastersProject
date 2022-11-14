/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Manage all doors in a single room segment
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    /// <summary>
    /// All doorways found in segment
    /// </summary>
    [SerializeField] private List<Door> doorways;
    
    /// <summary>
    /// Door determined to be the entrance
    /// </summary>
    private List<Door> exits;

    private Door chosenExit;

    /// <summary>
    /// All doorways marked for exit
    /// </summary>
    private Door entrance;

    /// <summary>
    /// Initialize the door manager
    /// </summary>
    public void InitializeDoorManager()
    {
        doorways = new List<Door>();
        exits = new List<Door>();

        Debug.Log("Exits initialized");

        // Get all doors, organize accordingly
        Door[] temp = GetComponentsInChildren<Door>(true);
        foreach (Door d in temp)
        {
            doorways.Add(d);
            d.Initialize();
        }
    }

    /// <summary>
    /// Reset the doors when returned to pool
    /// </summary>
    public void ResetDoors()
    {
        if (doorways == null)
            return;

        foreach (Door d in doorways)
        {
            d.SetType(Door.PlayerDoorType.Door);
        }

        // re-add entrance to available doorways
        entrance = null;
        chosenExit = null;
        exits.Clear();
    }

    /// <summary>
    /// Select what door will be this room's entrance
    /// </summary>
    /// <returns>New entrance syncpoint</returns>
    public Transform SelectEntrance()
    {
        // Randomly choose one available doorway as an entrance
        entrance = doorways[Random.Range(0, doorways.Count)];
        entrance.SetType(Door.PlayerDoorType.Entrance);
        
        // remove so it wont be selected by exit
        doorways.Remove(entrance);

        // Load all other doors into entrances
        foreach(Door d in doorways)
        {
            if (d != entrance)
            {
                exits.Add(d);
                d.SetType(Door.PlayerDoorType.Null);
            }    
        }

        entrance.UnlockDoor();

        return entrance.SyncPoint;
    }

    /// <summary>
    /// Select what door will be this room's exit
    /// </summary>
    /// <returns>new enterance sync point</returns>
    public Transform SelectExit()
    {
        // Select door, make sure its not the entrance
        Door d;
        do
        {
            Debug.Log(exits.Count);
            //Debug.Break();
            int i = Random.Range(0, exits.Count);
            
            d = exits[i];
            d.SetType(Door.PlayerDoorType.Exit);
        } while (d == entrance);

        chosenExit = d;

        return d.SyncPoint;
    }

    /// <summary>
    /// Unlock all doors set as exits
    /// </summary>
    public void UnlockAllDoors()
    {
        StartCoroutine(UnlockExit());
    }

    private IEnumerator UnlockExit()
    {
        while(chosenExit == null)
        {
            yield return null;
        }

        chosenExit.UnlockDoor();
    }
}
