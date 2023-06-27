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
    [Tooltip("Channel that fires when the player shoots the gun")]
    [SerializeField] ChannelVoid gunShootChannel;
    private float buildup;
    private float shotIncrement;

    [Header("Grenade"), Space(5)]
    [Tooltip("Channel that fires when the player launches a grenade")]
    [SerializeField] ChannelVoid grenadeLaunchChannel;

    [SerializeField] ChannelBool timestopActivatedChannel;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();

        if (gunShootChannel != null)
            gunShootChannel.OnEventRaised += ShootGun;
        if (grenadeLaunchChannel != null)
            grenadeLaunchChannel.OnEventRaised += LaunchGrenade;
        if (timestopActivatedChannel != null)
            timestopActivatedChannel.OnEventRaised += UpdateTimestop;
    }
    private void OnDisable()
    {
        if (gunShootChannel != null)
            gunShootChannel.OnEventRaised -= ShootGun;
        if (grenadeLaunchChannel != null)
            grenadeLaunchChannel.OnEventRaised -= LaunchGrenade;
        if (timestopActivatedChannel != null)
            timestopActivatedChannel.OnEventRaised -= UpdateTimestop;
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
    /// default max velocity of the player. Used for ratio stuff
    /// </summary>
    private const float maxPlayerVelocity = 11f;

    /// <summary>
    /// Update the player movement values for the animator
    /// </summary>
    public void UpdateMovement()
    {
        // Player is walking if they're grounded and moving at all
        bool playerMoving =
            moveController.CurrentState == PlayerController.PlayerState.GROUNDED
            && moveController.GetVelocity().magnitude > minVelocityMag;
        //Debug.Log("Vel: " + moveController.GetVelocity().magnitude);
        anim.SetBool("Is Walking", playerMoving);
        anim.SetFloat("Velocity", Mathf.Clamp((moveController.GetVelocity().magnitude / maxPlayerVelocity), 0.10f, 1));
    }


    private void UpdateTimestop(bool stopping)
    {
        if (stopping)
            OnStop();
        else
            OnResume();
    }
    private void OnStop()
    {
        anim.SetTrigger("TimeStopped");
    }

    private void OnResume()
    {
        anim.SetTrigger("TimeResumed");
    }
}
