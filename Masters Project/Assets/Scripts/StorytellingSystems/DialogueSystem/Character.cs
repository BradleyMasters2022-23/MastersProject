using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Storytelling/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    [Tooltip("Subtitle for this character"), TextArea]
    public string subtitle;
    
    [Tooltip("Thumbnail for this character")]
    public Sprite characterThumbnail;

    public Sprite[] sprites;

    [Tooltip("All conversations this character has")]
    public List<Conversation> allConversations;


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
