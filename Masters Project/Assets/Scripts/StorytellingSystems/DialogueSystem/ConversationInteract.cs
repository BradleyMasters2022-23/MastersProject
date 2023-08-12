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

    /// <summary>
    /// Whether this is currently flashing
    /// </summary>
    private bool flashing;

    [Tooltip("Source of the ringtone")]
    [SerializeField] protected AudioSource ringtonePlayer;

    [Tooltip("Renderer that flashes with new call")]
    [SerializeField] private MeshRenderer[] flashScreenRenderers;

    [Tooltip("Set of colors to loop through while flashing incoming call")]
    [SerializeField] private Color[] flashColors;

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
        calls.IncrementConvoTicks();

        timer = new ScaledTimer(flashTime);
    }

    /// <summary>
    /// If a call is available, make sure the current call can be used
    /// </summary>
    public void Update()
    {
        // keep the anchor on if calls are available
        if(waypointAnchor != null)
            waypointAnchor.SetActive((calls.HasAvailable() && CanInteract()));

        // if calls are available, flash
        // Stop loop if in game menu
        if(GameManager.instance.CurrentState != GameManager.States.GAMEMENU && GameManager.instance.CurrentState != GameManager.States.PAUSED
            && calls.HasAvailable() && !flashing)
        {
            BeginFlash();

            // make sure ringtone is playing
            //if (ringtonePlayer != null && !ringtonePlayer.isPlaying)
            //{
            //    AudioClipSO ringtone = calls.GetAvailableRingtone();
            //    ringtone.PlayClip(ringtonePlayer);
            //}
        }
        // otherwise, revert back to normal
        else if(flashing && (GameManager.instance.CurrentState == GameManager.States.GAMEMENU || GameManager.instance.CurrentState == GameManager.States.PAUSED || !calls.HasAvailable()))
        {
            //if (ringtonePlayer != null && ringtonePlayer.isPlaying)
            //    ringtonePlayer.Stop();
            
            StopFlash();
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

        //if (ringtonePlayer != null && ringtonePlayer.isPlaying)
        //    ringtonePlayer.Stop();

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

    /// <summary>
    /// Begin the flash routine
    /// </summary>
    private void BeginFlash()
    {
        foreach (var mesh in flashScreenRenderers)
        {
            mesh.enabled = true;
        }

        StartCoroutine(FlashRoutine());

        AudioClipSO ringtone = GetRingtone();
        ringtone.PlayClip(ringtonePlayer);

        flashing = true;
    }

    /// <summary>
    /// Stop the flash routine. 
    /// </summary>
    private void StopFlash()
    {
        StopCoroutine(FlashRoutine());

        foreach (var mesh in flashScreenRenderers)
            mesh.enabled = false;

        if (ringtonePlayer != null && ringtonePlayer.isPlaying)
            ringtonePlayer.Stop();

        flashing = false;
    }

    /// <summary>
    /// Continually flash through colors on the designated renders
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashRoutine()
    {
        int colorIdx = 0;
        while (true)
        {
            // apply color to marked renderes
            foreach(var mesh in flashScreenRenderers)
                mesh.material.color = flashColors[colorIdx];

            // iterate to next color, looping around if at the end of the array
            colorIdx = (colorIdx + 1) % (flashColors.Length);

            // wait for designated time
            timer.ResetTimer();
            yield return new WaitUntil(timer.TimerDone);
            yield return null;
        }
    }

    protected virtual AudioClipSO GetRingtone()
    {
        return calls.GetAvailableRingtone();
    }
}
