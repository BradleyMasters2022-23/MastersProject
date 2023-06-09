using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DisplayDialogueUI : MonoBehaviour
{
    GameControls controls;
    InputAction click;
    private int activeLineIndex = 0;

    private Conversation conversation;

    [SerializeField] ConversationLoaderUI pennyScreen;
    [SerializeField] ConversationLoaderUI NPCScreen;
    private ConversationLoaderUI currentActive;

    private bool init = false;

    private Coroutine loadingRoutine;

    private void OnEnable()
    {
        if (!init)
        {
            init = true;
            controls = GameManager.controls;
            click = controls.Dialogue.Advance;
        }
    }
    private void OnDisable()
    {
        click.performed -= DisplayDialogue;
    }

    public void OpenScreen(Conversation c)
    {
        conversation = c;
        activeLineIndex = 0;
        gameObject.SetActive(true);
        DisplayDialogue();

        if (!init)
        {
            init = true;
            controls = GameManager.controls;
            click = controls.Dialogue.Advance;
        }

        click.performed += DisplayDialogue;
        click.Enable();
    }

    private void DisplayDialogue(InputAction.CallbackContext c)
    {
        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        // If its still loading, instantly stop it
        if (currentActive != null && !currentActive.LoadingDone() && loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
            loadingRoutine = null;
            currentActive.InstantLoad();
            return;
        }
            
        if(activeLineIndex == conversation.lines.Length)
        {
            if (conversation.ID >= 0)
            {
                // mark conversation as read and add to save data
                CallManager.instance.SaveData(conversation);
            }
        }

        // Check which dialogue screen to use, if there are any lines left 
        if (activeLineIndex < conversation.lines.Length)
        {
            currentActive?.ResetScreen();

            if (conversation.lines[activeLineIndex].character.characterName == "Penny")
            {
                currentActive = pennyScreen;
                loadingRoutine = pennyScreen.LoadInDialogue(conversation.nonPennyCharacter, conversation.lines[activeLineIndex], true);
            }
            else
            {
                currentActive = NPCScreen;
                loadingRoutine = NPCScreen.LoadInDialogue(conversation.nonPennyCharacter, conversation.lines[activeLineIndex], false);
            }
        }
        else // otherwise, no more, close screen
        {
            CloseScreen();
        }

        activeLineIndex++;
    }

    public void CloseScreen()
    {
        

        click.Disable();
        GameManager.instance.CloseTopMenu();
    }
}
