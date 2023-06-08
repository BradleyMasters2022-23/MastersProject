/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 8th, 2023
 * Last Edited - June 8th, 2023 by Ben Schuster
 * Description - Data for a contact for the UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Storytelling/CharacterData")]
public class ContactOptionSO : ScriptableObject
{
    [Tooltip("Thumbnail for this character")]
    public Sprite characterThumbnail;
    [Tooltip("Name of this character")]
    public string characterName;
    [Tooltip("Subtitle for this character")]
    public string subtitle;
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
