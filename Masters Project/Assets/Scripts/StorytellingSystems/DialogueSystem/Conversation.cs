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
    public Conversation[] dependencies;
    // runs after last unlocked dependency OR total runs if no dependencies
    public int runReq;

    //public bool soloDialogue = false;

    public Fragment frag;

    public Line[] lines;
    public enum ConversationState 
    { 
        LOCKED, 
        UNREAD, 
        READ 
    }
    public ConversationState currentState = ConversationState.LOCKED;

    /*
    public void Read()
    {
        if(currentState != ConversationState.UNREAD && ID >= 0)
        {
            //Debug.Log("Conversation " + ID + " cannot be read");
        }
        else
        {
            currentState = ConversationState.READ;
        }
        
    }

    public void Unlock()
    {
        if(currentState== ConversationState.LOCKED)
        {
            currentState = ConversationState.UNREAD;
            //Debug.Log("Conversation " + ID + " is now available");
        }
        else
        {
            //Debug.Log("Conversation " + ID + " is not locked");
        }
    }

    public void Lock()
    {
        if(currentState != ConversationState.LOCKED)
        {
            currentState = ConversationState.LOCKED;
        }
        else
        {
            //Debug.Log("Conversation " + ID + " is already locked");
        }
    }
    */
}
