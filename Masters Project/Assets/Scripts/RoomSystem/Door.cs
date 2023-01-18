/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - November 13th, 2022 by Ben Schuster
 * Description - Controls an individual door that the player uses
 * ================================================================================================
 */
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

public class Door : MonoBehaviour
{

    [Tooltip("Sound when door opens")]
    [SerializeField] private AudioClip openDoor;
    [Tooltip("Sound when door closes")]
    [SerializeField] private AudioClip closeDoor;

    private AudioSource source;

    public int id;

    public enum PlayerDoorType
    {
        Door,
        Entrance,
        Open,
        Exit, 
        Null,
        ReturnToHub, 
        Custom
    }

    #region Serialized Variables and Getters

    [Header("=== Gameplay ===")]

    [Tooltip("What type of door is this")]
    [SerializeField] private PlayerDoorType type;
    [Tooltip("The actual door object"), Required]
    [SerializeField] private GameObject door;
    [Tooltip("Syncpoint for when syncing with other doors"), Required]
    [SerializeField] private Transform syncPoint;

    [Tooltip("What happens when entering this door?"), HideIf("@this.type != PlayerDoorType.Custom")]
    [SerializeField] private UnityEvent customTriggerEvent;

    [Tooltip("Whether this door should always be open")]
    [SerializeField] private bool overrideOpen;
    public Transform SyncPoint { get { return syncPoint; } }
    public PlayerDoorType Type { get { return type; } }

    [Header("=== Visuals ===")]

    [Tooltip("Panel that will change color when locked or unlocked")]
    [SerializeField] private GameObject doorLight;
    [Tooltip("Color of the light when door is locked"), HideIf("@this.doorLight == null")]
    [SerializeField] private Material lockedColor;
    [Tooltip("Color of the light when door is unlocked"), HideIf("@this.doorLight == null")]
    [SerializeField] private Material unlockedColor;
    [Tooltip("Color of the light when door is disabled"), HideIf("@this.doorLight == null")]
    [SerializeField] private Material disabledColor;

    

    #endregion

    /// <summary>
    /// collider for the room triggers
    /// </summary>
    private Collider[] col;

    /// <summary>
    /// Animator for this door
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Whether the door is currently locked 
    /// </summary>
    [SerializeField] private bool locked;
    
    
    public bool Locked { get { return locked; } }


    private bool initialized = false;

    #region Initialization Functions

    /// <summary>
    /// Automatically initialize, unlock if needed
    /// </summary>
    private void Awake()
    {
        Initialize();

        if (overrideOpen)
        {
            UnlockDoor();
        }
        source = gameObject.AddComponent<AudioSource>();

    }

    /// <summary>
    /// Initialize this segment
    /// </summary>
    public void Initialize()
    {
        if (initialized)
            return;

        initialized = true;
        animator = GetComponent<Animator>();
        col = GetComponents<Collider>();
        // Debug.Log($"Initializing door {id} to locked");
        locked = true;

        id = Random.Range(0, 9999);

        foreach (Collider c in col)
            c.enabled = false;
    }

    #endregion

    #region Assigning Functions

    /// <summary>
    /// Set the type of this door
    /// </summary>
    /// <param name="t">Tyope of door this should be</param>
    public void SetType(PlayerDoorType t)
    {
        // Debug.Log($"Door {id} being set to {t}");
        type = t;

        // Disable door if possible
        if(t == PlayerDoorType.Null)
        {
            doorLight.GetComponent<Renderer>().material = disabledColor;
        }
            
    }

    /// <summary>
    /// Try to open or close the physical door
    /// </summary>
    /// <param name="open">Whether the door should open</param>
    /// <returns>Whether the door can be opened</returns>
    public void SetOpenStatus(bool open)
    {
        // Open and close door. Animate stuff here.
        if(open)
        {
            door.SetActive(false);

            if(openDoor != null)
                source.PlayOneShot(openDoor, 0.5f);
        }
        else
        {
            door.SetActive(true);

            if(closeDoor!= null)
                source.PlayOneShot(closeDoor, 0.5f);
        }
    }

    /// <summary>
    /// Lock this door
    /// </summary>
    public void LockDoor()
    {
        if (type == PlayerDoorType.Null)
            return;

        // Debug.Log($"Door {id} having locked state set to lock");

        doorLight.GetComponent<Renderer>().material = lockedColor;
        locked = true;

        foreach (Collider c in col)
            c.enabled = false;
    }

    /// <summary>
    /// Unlock this door
    /// </summary>
    public void UnlockDoor()
    {
        if (type == PlayerDoorType.Null)
            return;

        doorLight.GetComponent<Renderer>().material = unlockedColor;
        locked = false;

        foreach(Collider c in col)
            c.enabled = true;
    }

    /// <summary>
    /// Set this door as a decoration door that does not function otherwise
    /// </summary>
    public void SetDecor()
    {
        doorLight.GetComponent<Renderer>().sharedMaterial = disabledColor;
    }

    #endregion


    /// <summary>
    /// When player enters, tell the room generator to load next room
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && col[0].enabled)
        {

            foreach(Collider c in col)
                c.enabled= false;

            switch(type)
            {
                case PlayerDoorType.Open:
                    {
                        //Debug.Log("Player detected entering open hallway door, loading next segment!");
                        MapLoader.instance.UpdateLoadedSegments();

                        LockDoor();

                        break;
                    }
                case PlayerDoorType.Entrance:
                    {
                        //Debug.Log("Player detected entering a room! Telling it to activate!");
                        // Call system to activate room

                        MapLoader.instance.StartRoomEncounter();

                        // lock door behind player
                        LockDoor();
                        SetDecor();

                        break;
                    }
                case PlayerDoorType.ReturnToHub:
                    {
                        // TODO - go to the 'finished upgrades' stuff instead
                        MapLoader.instance.ReturnToHub();
                        //GameManager.instance.ChangeState(GameManager.States.HUB);
                        break;
                    }
                case PlayerDoorType.Custom:
                    {
                        // Trigger any custom events when this happens
                        customTriggerEvent.Invoke();
                        break;
                    }
                default:
                    {
                        // Debug.Log("Player Detected, invalid door set!");
                        break;
                    }
            }
        }
    }
}
