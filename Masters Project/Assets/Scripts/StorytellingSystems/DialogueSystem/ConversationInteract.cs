using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationInteract : Interactable
{
    private CallManager calls;
    // eventually, should instead pull up a screen with a list of characters & info about them
    // when a conversation is available, screen should blink; eventually, screen & contact both blink
    private DisplayDialogueUI ui;

    private MeshRenderer flashRenderer;
    private Color original;
    private ScaledTimer timer;
    public float flashTime;

    private void Start()
    {
        ui = FindObjectOfType<DisplayDialogueUI>(true);
        calls = CallManager.instance;
        flashRenderer = gameObject.GetComponent<MeshRenderer>();
        original = flashRenderer.material.color;
        timer = new ScaledTimer(flashTime);
    }

    public void Update()
    {
        if(calls.HasAvailable() && timer.TimerDone())
        {
            if(flashRenderer.material.color == Color.red)
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

    public override void OnInteract(PlayerController player)
    {
        if (ui == null)
        {
            Debug.Log("No UI found.");
        }

        if (GameManager.instance.CurrentState != GameManager.States.GAMEPLAY && GameManager.instance.CurrentState != GameManager.States.HUB)
        {
            Debug.Log("Not in a state where the player can interact with this object");
            return;
        }

        if (calls.HasAvailable())
        {
            ui.OpenScreen(calls.GetRandomAvailableConversation());
        }
        else
        {
            ui.OpenScreen(calls.GetDefault());
        }

        
    }
}
