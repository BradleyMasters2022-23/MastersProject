using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Storytelling/Character")]
public class Character : ScriptableObject
{
    [Header("Data")]
    public string characterName;
    [Tooltip("Subtitle for this character"), TextArea]
    public string subtitle;

    [Tooltip("All conversations this character has")]
    public List<Conversation> allConversations;

    [Tooltip("When this characters dialogue is exhausted, pull from this list")]
    public GenericWeightedList<Conversation> repeatableConversations;

    [Header("Visuals")]
    [Tooltip("Thumbnail for this character")]
    public Sprite characterThumbnail;
    public Sprite[] sprites;

    [Header("Audio")]
    [Tooltip("The ringtone for this character. TBD")]
    public AudioClipSO ringtone;
    [Tooltip("The main theme for this character.")]
    public AudioClipSO characterTheme;

    /// <summary>
    /// Check whether this character has a conversation
    /// </summary>
    /// <param name="c">Conversation to check</param>
    /// <returns>Whether the concersation belongs to this contact</returns>
    public bool HasConversation(Conversation c)
    {
        return allConversations.Contains(c);
    }

    /// <summary>
    /// Whether this character has been met yet
    /// </summary>
    /// <param name="manager">Manager to check</param>
    /// <returns>Whether this character has been met</returns>
    public bool CharacterMet(CallManager manager)
    {
        return (manager.HasNewCall(allConversations[0])
            || manager.CallInSave(allConversations[0]));
    }

    public bool CharacterComplete(CallManager manager)
    {
        // If any conversation is NOT in save data, then character is not complete
        foreach(var conversation in allConversations)
        {
            if (!manager.CallInSave(conversation))
                return false;
        }
        // otherwise, complete
        return true;
    }
}
