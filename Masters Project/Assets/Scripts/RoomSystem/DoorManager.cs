/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Manage all doors to be locked, unlocked when called
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    /// <summary>
    /// All doorways found in scene
    /// </summary>
    private List<Door> doorways;
    /// <summary>
    /// All doorways marked for exit
    /// </summary>
    private List<Door> exits;
    /// <summary>
    /// Door determined to be the entrance
    /// </summary>
    private Door entrance;

    /// <summary>
    /// Whether or not this is already locked
    /// </summary>
    private bool locked = false;


    private void Start()
    {
        // Get all doors, organize accordingly
        Door[] temp = FindObjectsOfType<Door>();
        foreach (Door f in temp)
        {
            if (f.Type == Door.PlayerDoorType.Entrance && entrance is null)
            {
                entrance = f;
            }
            else if (f.Type == Door.PlayerDoorType.Exit)
            {
                exits.Add(f);
            }
            else
            {
                doorways.Add(f);
            }
        }

        // If no set enterance found, get one from doorways
        if (entrance == null)
        {
            entrance = doorways[Random.Range(0, doorways.Count)];
            doorways.Remove(entrance);
        }
        entrance.SetEntrance();

        // If no exits, set doors to exits. Otherwise, disable remaining doors
        if (exits.Count <= 0)
        {
            // If no exits, choose doors from doorways
            foreach (Door f in doorways)
            {
                f.SetExit();
                exits.Add(f);
            }
            doorways.Clear();
        }
        else
        {
            // If set exits, lock remaining doors
            foreach (Door f in doorways)
            {
                f.LockDoor();
            }
        }

        // Lock all doors set to exits
        locked = true;
        foreach (Door f in exits)
        {
            f.LockDoor();
        }

        // If no spawner, unlock doors
        if (GameObject.FindGameObjectWithTag("Spawn") is null)
        {
            Debug.Log("No spawn found, unlocking doors");
            UnlockAllDoors();
        }
    }

    /// <summary>
    /// Unlock all doors set as exits
    /// </summary>
    public void UnlockAllDoors()
    {
        foreach (Door f in exits)
        {
            f.UnlockDoor();
        }
    }
}
