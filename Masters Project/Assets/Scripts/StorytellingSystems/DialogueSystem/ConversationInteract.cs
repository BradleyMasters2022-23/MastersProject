using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationInteract : MonoBehaviour, Interactable
{
    private CallManager calls;
    // eventually, should instead pull up a screen with a list of characters & info about them
    // when a conversation is available, screen should blink; eventually, screen & contact both blink
    private ConvoRefManager ui;

    [SerializeField] private MeshRenderer flashRenderer;
    private Color original;
    private ScaledTimer timer;
    public float flashTime;

    // Delegate for any special functions to use
    // used by tooltip system to close tooltip when its used
    public delegate void OnInteraction();
    public OnInteraction onStartCall;


    [SerializeField] private GameObject waypointAnchor;

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

    public void Update()
    {
        if (flashRenderer == null)
            return;

        // keep the anchor on if calls are available
        if(waypointAnchor != null)
            waypointAnchor.SetActive(calls.HasAvailable());

        // if calls are available, flash
        if(calls.HasAvailable())
        {
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
            if (flashRenderer.material.color == Color.red)
            {
                flashRenderer.material.color = original;
            }
        }

        
    }

    public virtual void OnInteract(PlayerController player)
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

        // call any subscribed functions to the caller
        onStartCall?.Invoke();
    }
}
