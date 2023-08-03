/* ================================================================================================
 * Author - Soma   
 * Date Created - October, 2022
 * Last Edited - June 13th, 2023 by Ben Schuster
 * Description - Interact manager for the conversation system
 * ================================================================================================
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationInteract : MonoBehaviour, Interactable
{
    /// <summary>
    /// Call manager that handles saving/choosing calls
    /// </summary>
    private CallManager calls;
    /// <summary>
    /// Bulk UI manager controlling the 3 main convo screens
    /// </summary>
    private ConvoRefManager ui;

    #region Incoming Call Indicator Variables

    [Tooltip("Source of the ringtone")]
    [SerializeField] protected AudioSource ringtonePlayer;

    [Tooltip("Renderer that flashes with new call")]
    [SerializeField] private MeshRenderer flashRenderer;
    /// <summary>
    /// Original color of the flashing screen
    /// </summary>
    private Color original;
    /// <summary>
    /// Timer used to track flash intervals
    /// </summary>
    private ScaledTimer timer;
    /// <summary>
    /// Time inbetween each flash
    /// </summary>
    public float flashTime;

    [Tooltip("Waypoint that enables when call available")]
    [SerializeField] private GameObject waypointAnchor;

    #endregion

    // Delegate for any special functions to use
    // used by tooltip system to close tooltip when its used
    public delegate void OnInteraction();
    public OnInteraction onStartCall;

    private bool inCall;

    /// <summary>
    /// Initialize functions and get any references needed
    /// </summary>
    private void Start()
    {
        ui = ConvoRefManager.instance;
        calls = CallManager.instance;
        if(flashRenderer == null)
        {
            flashRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        if(flashRenderer != null)
            original = flashRenderer.material.color;

        timer = new ScaledTimer(flashTime);
    }

    /// <summary>
    /// If a call is available, make sure the current call can be used
    /// </summary>
    public void Update()
    {
        if (flashRenderer == null)
            return;

        // keep the anchor on if calls are available
        if(waypointAnchor != null)
            waypointAnchor.SetActive((calls.HasAvailable() && CanInteract()));

        // if calls are available, flash
        // Stop loop if in game menu
        if(GameManager.instance.CurrentState != GameManager.States.GAMEMENU
            && calls.HasAvailable())
        {
            // make sure ringtone is playing
            if (ringtonePlayer != null && !ringtonePlayer.isPlaying)
            {
                AudioClipSO ringtone = calls.GetAvailableRingtone();
                ringtone.PlayClip(ringtonePlayer);
            }

            // flash if time is up
            if (timer.TimerDone())
            {
                if (flashRenderer.material.color == Color.red)
                {
                    flashRenderer.material.color = original;
                    timer.ResetTimer();
                }
                else
                {
                    flashRenderer.material.color = Color.red;
                    timer.ResetTimer();
                }
            }
        }
        // otherwise, revert back to normal
        else
        {
            if (ringtonePlayer != null && ringtonePlayer.isPlaying)
                ringtonePlayer.Stop();

            if (flashRenderer.material.color == Color.red)
            {
                flashRenderer.material.color = original;
            }
        }
    }

    /// <summary>
    /// On interact, open the UI
    /// </summary>
    public virtual void OnInteract()
    {
        if (ui == null)
        {
            Debug.Log("No UI found.");
        }

        if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            //Debug.Log("Not in a state where the player can interact with this object");
            return;
        }

        if (ui != null)
        {
            ui.OpenScreen();
        }

        if (ringtonePlayer != null && ringtonePlayer.isPlaying)
            ringtonePlayer.Stop();

        // call any subscribed functions to the caller
        onStartCall?.Invoke();
    }

    /// <summary>
    /// Can interact if the UI is not null
    /// </summary>
    /// <returns></returns>
    public virtual bool CanInteract()
    {
        return true;
    }
}
