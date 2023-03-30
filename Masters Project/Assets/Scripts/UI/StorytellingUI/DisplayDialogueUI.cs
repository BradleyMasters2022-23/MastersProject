using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Reflection;

public class DisplayDialogueUI : MonoBehaviour
{
    GameControls c;
    InputAction click;
    private EventSystem eventSystem;
    private int activeLineIndex = 0;

    private Conversation conversation;

    [SerializeField] private Sprite pennyBackground;
    [SerializeField] private Sprite npcBackground;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI npcDialogueText;
    [SerializeField] private Image background;
    [SerializeField] private Image pennySprite;
    [SerializeField] private Image npcSprite;

    private void Awake()
    {
        c = new GameControls();
        click = c.Dialogue.Advance;
        click.performed += DisplayDialogue;
    }

    private void OnDisable()
    {
        if (click != null)
            click.performed -= DisplayDialogue;
    }

    private void OnEnable()
    {
        if(click!=null)
            click.performed += DisplayDialogue;
    }

    public void OpenScreen(Conversation c)
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        conversation = c;
        activeLineIndex = 0;
        GameManager.instance.ChangeState(GameManager.States.GAMEMENU);
        DisplayDialogue();
        gameObject.SetActive(true);
        click.Enable();
    }

    private void DisplayDialogue(InputAction.CallbackContext c)
    {
        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        if (activeLineIndex < conversation.lines.Length)
        {
            
            dialogueText.text = conversation.lines[activeLineIndex].text;
            if (conversation.lines[activeLineIndex].character.characterName == "Penny")
            {
                nameText.text = conversation.lines[activeLineIndex].character.characterName;
                nameText.enabled = true;
                npcNameText.enabled = false;
                dialogueText.text = conversation.lines[activeLineIndex].text;
                dialogueText.enabled = true;
                npcDialogueText.enabled = false;
                background.sprite = pennyBackground;
                pennySprite.sprite = conversation.lines[activeLineIndex].character.sprites[conversation.lines[activeLineIndex].pennySpriteID];
                npcSprite.sprite = conversation.nonPennyCharacter.sprites[conversation.lines[activeLineIndex].npcSpriteID];
                pennySprite.enabled = true;
            }
            else
            {
                npcNameText.text = conversation.lines[activeLineIndex].character.characterName;
                npcNameText.enabled = true;
                nameText.enabled = false;
                npcDialogueText.text = conversation.lines[activeLineIndex].text;
                npcDialogueText.enabled = true;
                dialogueText.enabled = false;
                background.sprite = npcBackground;
                npcSprite.sprite = conversation.lines[activeLineIndex].character.sprites[conversation.lines[activeLineIndex].npcSpriteID];
                pennySprite.enabled = false;
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
        if(conversation.ID >= 0)
        {
            conversation.Read();
        }

        click.Disable();

        GameManager.instance.CloseTopMenu();
        //gameObject.SetActive(false);
        //GameManager.instance.ChangeState(GameManager.States.HUB);
    }

}
