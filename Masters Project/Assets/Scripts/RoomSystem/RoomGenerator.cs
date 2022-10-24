/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Manage selecting and spawning in the rooms
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    [Tooltip("Name of scenes that can be used to load")]
    [SerializeField] private string[] scenePool;

    [Tooltip("Amount of rooms to be played per floor")]
    [SerializeField] private int floorLength;
    [Tooltip("Name of hub scene to return to on player death")]
    [SerializeField] private string returnRoom;
    [Tooltip("Name of room to load after reaching the length limit")]
    [SerializeField] private string finalRoom;

    /// <summary>
    /// Public instance of room generator
    /// </summary>
    public static RoomGenerator instance;

    /// <summary>
    /// Name of the currently loaded room
    /// </summary>
    private string currRoom;
    /// <summary>
    /// Name of the last loaded room
    /// </summary>
    private string lastRoom;
    /// <summary>
    /// Current floor count tracker
    /// </summary>
    private int count;
    
    /// <summary>
    /// Prepare singleton, initialize
    /// </summary>
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            instance.count = 1;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // TODO - prepare better
        currRoom = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Select a new room 
    /// </summary>
    public void SelectRoom()
    {
        // If player just finished the final room, return to 'hub'
        if (currRoom == finalRoom)
        {
            ReturnToHub();
            return;
        }

        // If reaching max floor length, load the last room
        if (count >= floorLength)
        {
            lastRoom = currRoom;
            currRoom = finalRoom;
            LoadRoom();

            return;
        }

        // Choose the next room, making sure its not the previous room
        string next;
        do
        {
            next = scenePool[Random.Range(0, scenePool.Length)];

        } while (next == currRoom);

        // Iterate next set of rooms
        lastRoom = currRoom;
        currRoom = next;

        // Load the new room
        LoadRoom();
    }

    /// <summary>
    /// Load the next room, unload previous room and increment
    /// </summary>
    public void LoadRoom()
    {
        instance.count++;
        SceneManager.UnloadSceneAsync(lastRoom);
        SceneManager.LoadSceneAsync(currRoom);
    }

    /// <summary>
    /// Return to the hub world and reset upgrades
    /// </summary>
    public void ReturnToHub()
    {
        RoomGenerator.instance = null;

        // TODO - RESET PLAYER UPGRADES
        //PlayerUpgradeManager.instance.DestroyPUM();
        //RewardStorage.instance.DestroyRS();

        SceneManager.UnloadSceneAsync(currRoom);
        SceneManager.LoadSceneAsync(returnRoom);
        Destroy(gameObject);
    }
}
