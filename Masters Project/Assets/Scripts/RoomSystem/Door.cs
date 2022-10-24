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

    /// <summary>
    /// What type of door is this
    /// </summary>
    private PlayerDoorType type;
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

    /// <summary>
    /// The light used to indicate if locked or unlocked
    /// </summary>
    private Light indicatorLight;
    [Tooltip("Color of the light when door is locked")]
    [SerializeField] private Color lockedColor;
    [Tooltip("Color of the light when door is unlocked")]
    [SerializeField] private Color unlockedColor;
    [Tooltip("The collider for the actual door object")]
    [SerializeField] private Collider doorCollider;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Get light, disable dev image
        indicatorLight = GetComponentInChildren<Light>();
        Destroy(FindObjectOfType<Image>());
    }

    /// <summary>
    /// Set this door to entrance, move player to its spawn position.
    /// </summary>
    public void SetEntrance()
    {
        // Lock door, turn to locked color
        doorCollider.isTrigger = false;
        indicatorLight.color = lockedColor;

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
        indicatorLight.color = lockedColor;
        doorCollider.isTrigger = false;
    }

    /// <summary>
    /// Unlock this door
    /// </summary>
    public void UnlockDoor()
    {
        indicatorLight.color = lockedColor;
        doorCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoomGenerator.instance.SelectRoom();
        }
    }
}
