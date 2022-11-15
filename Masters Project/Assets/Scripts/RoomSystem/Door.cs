/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Controls an individual door that the player uses
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public enum PlayerDoorType
    {
        Door,
        Entrance,
        Open,
        Exit, 
        Null
    }

    [Tooltip("What type of door is this")]
    [SerializeField] private PlayerDoorType type;
    /// <summary>
    /// What type of door is this
    /// </summary>
    public PlayerDoorType Type
    {
        get { return type; }
    }

    [Tooltip("Panel that will change color when locked or unlocked")]
    [SerializeField] private GameObject doorLight;
    [Tooltip("Color of the light when door is locked")]
    [SerializeField] private Material lockedColor;
    [Tooltip("Color of the light when door is unlocked")]
    [SerializeField] private Material unlockedColor;
    [Tooltip("The actual door object")]
    [SerializeField] private GameObject door;

    [Tooltip("Syncpoint for when syncing with other doors")]
    [SerializeField] private Transform syncPoint;
    public Transform SyncPoint
    {
        get { return syncPoint; }
    }

    /// <summary>
    /// collider for the room triggers
    /// </summary>
    private Collider col;

    /// <summary>
    /// Animator for this door
    /// </summary>
    private Animator animator;

    private bool locked;
    public bool Locked { get { return locked; } }

    [SerializeField] private bool overrideOpen;

    private void Awake()
    {
        if (overrideOpen)
        {
            col = GetComponent<Collider>();
            UnlockDoor();
        }
            
    }

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        locked = true;
    }

    public void SetType(PlayerDoorType t)
    {
        type = t;
    }

    /// <summary>
    /// Try to open or close the door
    /// </summary>
    /// <param name="open">Whether the door should open</param>
    /// <returns>Whether the door can be opened</returns>
    public void SetOpenStatus(bool open)
    {
        // Open and close door. Animate stuff here.
        if(open)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
        }
    }

    /// <summary>
    /// Lock this door
    /// </summary>
    public void LockDoor()
    {
        doorLight.GetComponent<Renderer>().material = lockedColor;
        locked = true;
        col.enabled = false;
    }

    /// <summary>
    /// Unlock this door
    /// </summary>
    public void UnlockDoor()
    {
        doorLight.GetComponent<Renderer>().material = unlockedColor;
        locked = false;
        col.enabled = true;
    }

    /// <summary>
    /// When player enters, tell the room generator to load next room
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            switch(type)
            {
                case PlayerDoorType.Open:
                    {
                        //Debug.Log("Player detected entering open hallway door, loading next segment!");
                        MapLoader.instance.UpdateLoadedSegments();

                        LockDoor();
                        col.enabled = false;

                        break;
                    }
                case PlayerDoorType.Entrance:
                    {
                        //Debug.Log("Player detected entering a room! Telling it to activate!");
                        // Call system to activate room

                        MapLoader.instance.StartRoomEncounter();

                        // lock door behind player
                        LockDoor();
                        col.enabled = false;
                        break;
                    }
                default:
                    {
                        // Debug.Log("Player Detected, invalid door set!");
                        col.enabled = false;
                        break;
                    }
            }
        }
    }
}
