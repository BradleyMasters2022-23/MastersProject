using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DisplayDialogueUI : MonoBehaviour
{
    GameControls c;
    InputAction e;
    private EventSystem eventSystem;
    private int activeLineIndex = 0;

    private Conversation conversation;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image pennySprite;
    [SerializeField] private Image npcSprite;

    private void Awake()
    {
        c = new GameControls();
        e = c.PlayerGameplay.Interact;
        e.performed += DisplayDialogue;
    }

    public void OpenScreen(Conversation c)
    {
        conversation = c;
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        DisplayDialogue();
        gameObject.SetActive(true);
        e.Enable();
    }

    private void DisplayDialogue(InputAction.CallbackContext c)
    {
        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        if (activeLineIndex < conversation.lines.Length)
        {
            nameText.text = conversation.lines[activeLineIndex].character.characterName;
            dialogueText.text = conversation.lines[activeLineIndex].text;
            if (conversation.lines[activeLineIndex].character.characterName == "Penny")
            {
                pennySprite.sprite = conversation.lines[activeLineIndex].character.sprites[conversation.lines[activeLineIndex].pennySpriteID];
                npcSprite.sprite = conversation.nonPennyCharacter.sprites[conversation.lines[activeLineIndex].npcSpriteID];
            }
            else
            {
                npcSprite.sprite = conversation.lines[activeLineIndex].character.sprites[conversation.lines[activeLineIndex].npcSpriteID];
                pennySprite.sprite = conversation.penny.sprites[conversation.lines[activeLineIndex].pennySpriteID];
            }
            
        }
        else
        {
            CloseScreen();
        }

        activeLineIndex++;
    }

    public void CloseScreen()
    {
        conversation.Read();
        gameObject.SetActive(false);
        GameManager.instance.ChangeState(GameManager.States.HUB);
    }

}
