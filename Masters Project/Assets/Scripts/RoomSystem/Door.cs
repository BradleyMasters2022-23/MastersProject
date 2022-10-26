/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 24th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
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
        Exit
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

    [Tooltip("Point where player spawns when used as an entrance")]
    [SerializeField] private Transform spawnPoint;

    /// <summary>
    /// Animator for this door
    /// </summary>
    private Animator animator;

    [Tooltip("Panel that will change color when locked or unlocked")]
    [SerializeField] private GameObject doorLight;
    [Tooltip("Color of the light when door is locked")]
    [SerializeField] private Material lockedColor;
    [Tooltip("Color of the light when door is unlocked")]
    [SerializeField] private Material unlockedColor;
    [Tooltip("The actual door object")]
    [SerializeField] private GameObject door;

    /// <summary>
    /// Get any internal references
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Set this door to entrance, move player to its spawn position.
    /// </summary>
    public void SetEntrance()
    {
        // Lock door, turn to locked color
        LockDoor();

        // Spawn player to entrance position
        GameObject p = FindObjectOfType<PlayerController>().gameObject;
        p.transform.position = spawnPoint.transform.position;
        p.transform.rotation = spawnPoint.transform.rotation;
    }

    /// <summary>
    /// Set this door as an exit
    /// </summary>
    public void SetExit()
    {
        type = PlayerDoorType.Exit;
    }

    /// <summary>
    /// Lock this door
    /// </summary>
    public void LockDoor()
    {
        doorLight.GetComponent<Renderer>().material = lockedColor;
        door.SetActive(true);
    }

    /// <summary>
    /// Unlock this door
    /// </summary>
    public void UnlockDoor()
    {
        doorLight.GetComponent<Renderer>().material = unlockedColor;
        door.SetActive(false);
    }

    /// <summary>
    /// When player enters, tell the room generator to load next room
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoomGenerator.instance.SelectRoom();
        }
    }
}
