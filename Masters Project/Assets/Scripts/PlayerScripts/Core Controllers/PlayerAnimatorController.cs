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

    [Header("Grenade"), Space(5)]
    [Tooltip("Channel that fires when the player launches a grenade")]
    [SerializeField] ChannelVoid grenadeLaunchChannel;

    [SerializeField] ChannelBool timestopActivatedChannel;

    [Header("Weapon Sway")]
    [Tooltip("The intensity of the sway"), SerializeField]
    private float sway;
    [Tooltip("Speed of the sway"), SerializeField]
    private float smoothness;
    [SerializeField] float maxHorizontalMag;
    [SerializeField] float maxVerticalMag;

    /// <summary>
    /// default look angle for the weapon
    /// </summary>
    Quaternion defaultLookAng;

    private void OnEnable()
    {
        defaultLookAng = transform.localRotation;
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

        // if in idle, allow sway
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Sway"))
            WeaponSway();
    }

    /// <summary>
    /// Trigger a weapon shoot
    /// </summary>
    public void ShootGun()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("Sway"))
            transform.localRotation = defaultLookAng;

        anim.SetTrigger("Shooting Trigger");
    }

    /// <summary>
    /// Trigger a grenade launch
    /// </summary>
    public void LaunchGrenade()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Sway"))
            transform.localRotation = defaultLookAng;
        
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
        bool playerGrounded =
            moveController.CurrentState == PlayerController.PlayerState.GROUNDED;

        // slow down the swap if the player isnt grounded
        if (playerGrounded)
            anim.SetFloat("Velocity", Mathf.Clamp((moveController.GetVelocity().magnitude / maxPlayerVelocity), 0.10f, 1));
        else
            anim.SetFloat("Velocity", 0.20f);
    }


    private void UpdateTimestop(bool stopping)
    {
        //Debug.Log("timestop trigger set to " + stopping);
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

    /// <summary>
    /// Sway the weapon based on the input
    /// </summary>
    private void WeaponSway()
    {
        Vector2 data = InputManager.Controls.PlayerGameplay.Aim.ReadValue<Vector2>();

        // calculate angles
        float horRot = Mathf.Clamp(data.y * sway, -maxHorizontalMag, maxHorizontalMag);
        float verRot = Mathf.Clamp(data.x * sway, -maxVerticalMag, maxVerticalMag);
        Quaternion rotX = Quaternion.AngleAxis(horRot, Vector3.forward);
        Quaternion rotY = Quaternion.AngleAxis(-verRot, Vector3.up);

        // combine values and apply
        Quaternion tgtRot = defaultLookAng * rotY * rotX;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, tgtRot, smoothness * Time.deltaTime);
    }
}
