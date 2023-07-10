/* ================================================================================================
 * Author - Soma    
 * Date Created - October, 2022
 * Last Edited - June 13th, 2023 by Ben Schuster
 * Description - Handles iterating over a conversation for the UI. 
 * ================================================================================================
 */
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

    public delegate void onDialogueFinish();
    private onDialogueFinish loadedFinishFunc;


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
        if (loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
            loadingRoutine = null;
        }

        currentActive = null;
        loadedFinishFunc = null;
        click.performed -= DisplayDialogue;
        pennyScreen.ResetScreen();
        NPCScreen.ResetScreen();
    }

    /// <summary>
    /// Open the screen and begin displaying dialogue
    /// </summary>
    /// <param name="c">Conversation to process</param>
    /// <param name="onFinishFunc">The function to perform upon COMPLETION of the dialogue</param>
    public void OpenScreen(Conversation c, onDialogueFinish onFinishFunc = null)
    {
        if (GameManager.instance.CurrentState != GameManager.States.GAMEMENU)
            GameManager.instance.ChangeState(GameManager.States.GAMEMENU);

        loadedFinishFunc = onFinishFunc;
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
            
        // save conversation if the player reaches the last line of dialogue
        if(activeLineIndex == conversation.lines.Length && conversation.ID >= 0)
        {
            // mark conversation as read and add to save data
            CallManager.instance.SaveData(conversation);
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
        else // otherwise, no more, close screen, perform any on finish functionality
        {
            // this is extremely sloppy code but if i want to have time to get any writing in today i need to just leave it
            // if i need another conversation to give a note i'll make it a proper system and not just a few lines of spaghetti
            // but for now this works so i'll leave it. let me know if for some reason it breaks something and i'll fix it then too
            if (conversation.frag != null && !AllNotesManager.instance.FragmentCollected(conversation.frag))
            {
                AllNotesManager.instance.FragmentFound(conversation.frag);
                SamTooltipTrigger.instance.TriggerTooltip();
            }
            loadedFinishFunc?.Invoke();
            CloseScreen();
        }

        activeLineIndex++;
    }
    /// <summary>
    /// Close the screen after it finishes the entire dialogue
    /// </summary>
    public void CloseScreen()
    {
        click.Disable();
        GameManager.instance.CloseTopMenu();
    }
}
