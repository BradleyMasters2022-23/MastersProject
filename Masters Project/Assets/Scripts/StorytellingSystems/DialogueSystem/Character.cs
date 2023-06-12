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

}
