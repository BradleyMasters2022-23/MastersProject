using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationInteract : Interactable
{
    [SerializeField] private CallManager calls;
    // eventually, should instead pull up a screen with a list of characters & info about them
    // when a conversation is available, screen should blink; eventually, screen & contact both blink
    private DisplayDialogueUI ui;

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

        ui.OpenScreen(calls.GetRandomAvailableConversation());
    }
}
