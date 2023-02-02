//ref: https://www.youtube.com/watch?v=YJLcanHcJxo&t=0s

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line
{
    public Character character;
    public int pennySpriteID;
    public int npcSpriteID;

    [TextArea(2, 5)]
    public string text;

}

[CreateAssetMenu(menuName = "Storytelling/Conversation")]
public class Conversation : ScriptableObject
{
    public Character penny;
    public Character nonPennyCharacter;
    public int ID;
    public int[] dependencies;
    // runs after last unlocked dependency OR total runs if no dependencies
    public int runReq;

    public bool soloDialogue = false;

    public Line[] lines;
    public enum ConversationState 
    { 
        LOCKED, 
        UNREAD, 
        READ 
    }
    public ConversationState currentState = ConversationState.LOCKED;

    public void Read()
    {
        currentState = ConversationState.READ;
    }

    public void Unlock()
    {
        if(currentState== ConversationState.LOCKED)
        {
            currentState = ConversationState.UNREAD;
            Debug.Log("conversation " + ID + " is now available");
        }
        else
        {
            Debug.Log("conversation is not locked");
        }
    }
}
