using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class DisplayDialogueUI : MonoBehaviour
{
    GameControls c;
    InputAction e;
    private EventSystem eventSystem;
    private int activeLineIndex = 0;

    [SerializeField] private Conversation dialogue;

    private void Awake()
    {
        c = new GameControls();
        e = c.PlayerGameplay.Interact;
        e.performed += AdvanceDialogue;
    }

    public void OpenScreen()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        gameObject.SetActive(true);
        e.Enable();
    }

    private void AdvanceDialogue(InputAction.CallbackContext c)
    {
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        activeLineIndex++;
        if (activeLineIndex < dialogue.lines.Length)
        {
            DisplayLine();
        }
    }

    private void DisplayLine()
    {
        Line line = dialogue.lines[activeLineIndex];
        Character character = line.character;
    }

    private void SetDialogue()
    {

    }
}
