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

    /// <summary>
    /// Get any internal references
    /// </summary>
    //private void Awake()
    //{
    //    Initialize();
    //}

    public void Initialize()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
    }

    public void SetType(PlayerDoorType t)
    {
        type = t;
    }

    /// <summary>
    /// Lock this door
    /// </summary>
    public void LockDoor()
    {
        doorLight.GetComponent<Renderer>().material = lockedColor;
        door.SetActive(true);

        col.enabled = false;
    }

    /// <summary>
    /// Unlock this door
    /// </summary>
    public void UnlockDoor()
    {
        doorLight.GetComponent<Renderer>().material = unlockedColor;
        door.SetActive(false);

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
                case PlayerDoorType.Exit:
                    {
                        Debug.Log("Player Detected, calling enter door!");

                        //door.SetActive(false);

                        // If assigned as an entance, trigger load room
                        MapLoader.instance.enterDoor.Invoke();
                        col.enabled = false;
                        break;
                    }
                case PlayerDoorType.Entrance:
                    {
                        Debug.Log("Player Detected, calling exit door!");

                        // If assigned as an exit, trigger unload room
                        MapLoader.instance.exitDoor.Invoke();

                        LockDoor();
                        break;
                    }
                default:
                    {
                        //Debug.Log("Player Detected, not not set!");
                        break;
                    }
            }
        }
    }
}
