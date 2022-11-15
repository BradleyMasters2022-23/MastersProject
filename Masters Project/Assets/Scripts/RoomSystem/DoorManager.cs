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

public class DoorManager : MonoBehaviour, MapInitialize
{
    /// <summary>
    /// All doorways found in segment
    /// </summary>
    private List<Door> doorways;

    /// <summary>
    /// Door chosen to be this room's exit
    /// </summary>
    private Door exit;
    /// <summary>
    /// Door chosen to be this room's entrance
    /// </summary>
    private Door entrance;

    /// <summary>
    /// Whether or not the manager is initialized
    /// </summary>
    private bool initialized;
    public bool Initialized { get { return initialized; } }
    bool MapInitialize.initialized => initialized;

    public IEnumerator InitializeComponent()
    {
        // Set up the lists
        doorways = new List<Door>();

        // Get temp pools for checking preset entrances/exits
        List<Door> potentialEntrances = new List<Door>();
        List<Door> potentialExits = new List<Door>();

        // Get all doors, organize accordingly. Nulls should be excluded
        Door[] temp = GetComponentsInChildren<Door>(true);
        foreach (Door d in temp)
        {
            d.Initialize();

            // Ignore nulled doors. Nulled doors are set to 'off'
            if (d.Type == Door.PlayerDoorType.Entrance)
                potentialEntrances.Add(d);
            else if (d.Type == Door.PlayerDoorType.Exit)
                potentialExits.Add(d);
            else if (d.Type == Door.PlayerDoorType.Door)
                doorways.Add(d);

        }

        // Validate
        if((doorways.Count <= 0 && potentialEntrances.Count <= 0)
            || (doorways.Count <= 0 && potentialExits.Count <= 0))
        {
            Debug.LogError($"[DoorManager] Error! There are no doors set to neutral, and not enough set to either entrance and/or exits!" +
                $" Door manager cannot continue initializing!");

            yield break;
        }

        // === Get an entrance === //

        // If any doors marked as entrance, choose from those
        if (potentialEntrances.Count > 0)
            entrance = GetDoor(potentialEntrances);
        // Otherwise, choose from all doors
        else
            entrance = GetDoor(doorways);

        entrance.SetType(Door.PlayerDoorType.Entrance);
        entrance.UnlockDoor();


        // === Get an exit === //

        // If any doors makred as exit, choose from those
        if (potentialExits.Count > 0)
            exit = GetDoor(potentialExits);
        // Otherwise, choose from all doors. Dont select entrance
        else
            exit = GetDoor(doorways, entrance);

        // Initialization complete
        initialized = true;

        //Debug.Log("Door manager initialized");

        yield return null;
    }

    /// <summary>
    /// Note this manager's doors as hallway, namely unlocking the entrance
    /// </summary>
    public void SetHallway()
    {
        entrance.SetType(Door.PlayerDoorType.Open);
        entrance.UnlockDoor();
    }

    /// <summary>
    /// Get a door from the passed in list. Exclude a specific door 
    /// </summary>
    /// <param name="options">List of options to choose from</param>
    /// <param name="exclude">What is the exception</param>
    /// <returns>Randomly selected door thats not the excluded option</returns>
    private Door GetDoor(List<Door> options, Door exclude = null)
    {
        // Validate thers enough to select from
        if(options.Count == 1 && options[0] == exclude)
        {
            Debug.LogError($"[DoorManager] Error! There are not enough doors set to neutral, and not enough set to either entrance and/or exits!" +
                $" Door manager cannot continue initializing!");
            Debug.Break();

            return null;
        }

        Door d;

        // Randomly select a door while its equal to the excluded option
        do
        { 
            d = options[Random.Range(0, options.Count)];
        } while (d == exclude);

        return d;
    }

    /// <summary>
    /// Get the room's entrance
    /// </summary>
    /// <returns>New entrance syncpoint</returns>
    public Door GetEntrance()
    {
        return entrance;
    }

    /// <summary>
    /// Get the room's exit
    /// </summary>
    /// <returns>new enterance sync point</returns>
    public Door GetExit()
    {
        return exit;
    }

    /// <summary>
    /// Unlock all doors set as exits
    /// </summary>
    public void UnlockExit()
    {
        exit.UnlockDoor();
    }

    /// <summary>
    /// Get all syncpoints this manager detects
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetSyncpoints()
    {
        if (doorways == null)
            return null;

        List<Transform> list = new List<Transform>();

        foreach(Door d in doorways)
        {
            list.Add(d.SyncPoint);
        }

        return list;
    }
}
