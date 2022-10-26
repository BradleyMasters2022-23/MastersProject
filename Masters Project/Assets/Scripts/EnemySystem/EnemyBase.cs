/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 26th, 2022
 * Last Edited - October 26th, 2022 by Ben Schuster
 * Description - Base class for all enemies
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Interface Stuff
    public enum EnemyState
    {
        Idle,
        Preparing,
        Moving,
        Attacking
    }

    [Header("---General Enemy Stats---")]

    /// <summary>
    /// Player object
    /// </summary>
    protected GameObject player;
    /// <summary>
    /// Center of the player
    /// </summary>
    protected Transform playerCenter;

    //[Tooltip("Current state of the enemy")]
    //[SerializeField] protected EnemyState currentState;

    /// <summary>
    /// Current world time, based on time manager
    /// </summary>
    //protected float currTime;

    //[Tooltip("Maximum speed for this enemy")]
    //[SerializeField]
    //protected float maxMoveSpeed;

    //[Tooltip("Speed the enemy reaches max speed")]
    //[SerializeField]
    //protected float accelerationSpeed;

    //[Tooltip("Speed the enemy can rotate")]
    //[SerializeField]
    //protected float rotationSpeed;

    /// <summary>
    /// Prepare setup
    /// </summary>
    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        playerCenter = FindObjectOfType<PlayerController>().CenterMass;
    }
}
