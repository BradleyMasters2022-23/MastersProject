/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 27th, 2022
 * Last Edited - June 27th, 2022 by Ben Schuster
 * Description - Manage animation controller for player weapon.
 * Currently tuned to work for Ani_Con_Player_Blaster
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    /// <summary>
    /// Animator for the player weapon
    /// </summary>
    private Animator anim;

    [Header("Movement"), Space(5)]
    [Tooltip("Reference to the movement controller of the player")]
    [SerializeField] PlayerController moveController;
    [Tooltip("Minimum velocity magnitude to be considered moving for the animator")]
    [SerializeField] float minVelocityMag;

    [Header("Gun"), Space(5)]
    [Tooltip("Reference to the player gun controller")]
    [SerializeField] ChannelVoid gunShootChannel;

    [Header("Grenade"), Space(5)]
    [Tooltip("Reference to the grenade ability controller")]
    [SerializeField] ChannelVoid grenadeLaunchChannel;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();

        if (gunShootChannel != null)
            gunShootChannel.OnEventRaised += ShootGun;
        if (grenadeLaunchChannel != null)
            grenadeLaunchChannel.OnEventRaised += LaunchGrenade;
    }
    private void OnDisable()
    {
        if (gunShootChannel != null)
            gunShootChannel.OnEventRaised -= ShootGun;
        if (grenadeLaunchChannel != null)
            grenadeLaunchChannel.OnEventRaised -= LaunchGrenade;
    }
    private void Update()
    {
        if(moveController != null)
        {
            UpdateMovement();
        }
    }

    /// <summary>
    /// Trigger a weapon shoot
    /// </summary>
    public void ShootGun()
    {
        anim.SetTrigger("Shooting Trigger");
    }
    /// <summary>
    /// Trigger a grenade launch
    /// </summary>
    public void LaunchGrenade()
    {
        anim.SetTrigger("Grenade Trigger");
    }

    /// <summary>
    /// Update the player movement values for the animator
    /// </summary>
    public void UpdateMovement()
    {
        // Player is walking if they're grounded and moving at all
        bool playerMoving =
            moveController.CurrentState == PlayerController.PlayerState.GROUNDED
            && moveController.GetVelocity().magnitude > minVelocityMag;

        anim.SetBool("Is Walking", playerMoving);
    }

    public void UpdateTimestop()
    {
        bool slowingTime = TimeManager.TimeStopped;
        anim.SetBool("TimeStopped", slowingTime);
    }
}
