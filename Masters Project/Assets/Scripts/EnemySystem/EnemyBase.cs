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
    /*

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

    /// <summary>
    /// Center of this enemy
    /// </summary>
    public Transform centerMass;

    /// <summary>
    /// Prepare setup
    /// </summary>
    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        playerCenter = FindObjectOfType<PlayerController>().CenterMass;
        
        if(centerMass is null)
        {
            centerMass = transform;
        }
    }

    */
}
