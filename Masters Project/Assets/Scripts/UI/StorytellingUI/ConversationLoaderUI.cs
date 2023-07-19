using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Masters.UI;
using TMPro;

public class ConversationLoaderUI : MonoBehaviour
{
    [SerializeField] private Character pennyData;
    [SerializeField] private Image background;
    [SerializeField] private Image pennySprite;
    [SerializeField] private Image NPCSprite;
    [SerializeField] private TextMeshProUGUI characterNameBox;
    [SerializeField] private TextMeshProUGUI dialogueBox;

    [SerializeField] private float textLoadPerChar;

    private Character other;
    private Line currData;

    public Coroutine LoadInDialogue(Character o, Line data, bool penny)
    {
        currData = data;
        other = o;

        // update visuals
        //background.sprite = data.background;
        if(pennySprite != null)
            pennySprite.sprite = pennyData.sprites[currData.pennySpriteID];

        NPCSprite.sprite = other.sprites[currData.npcSpriteID];

        // set name
        characterNameBox.text = (penny) ? pennyData.characterName : currData.character.characterName;
        gameObject.SetActive(true);

        // load in dialogue using text loader
        return StartCoroutine(dialogueBox.SlowTextLoadRealtime(currData.text, textLoadPerChar));
    }

    /// <summary>
    /// Instantly load the text
    /// </summary>
    public void InstantLoad()
    {
        dialogueBox.text = currData.text;
    }

    /// <summary>
    /// Reset this screen and its data
    /// </summary>
    public void ResetScreen()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        other = null;
        dialogueBox.text = "";
        characterNameBox.text = "";
        NPCSprite.name = null;
    }
    public bool LoadingDone()
    {
        return (dialogueBox.text == currData.text);
    }
}
