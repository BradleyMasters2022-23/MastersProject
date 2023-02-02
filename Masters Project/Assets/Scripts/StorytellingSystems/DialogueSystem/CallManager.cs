
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallManager : MonoBehaviour
{
    public List<Conversation> conversations = new List<Conversation>();
    private List<Conversation> availableConversations = new List<Conversation>();
    public static CallManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    private void UpdateCalls()
    {
        // loop through available conversations
        foreach(Conversation conversation in availableConversations) 
        {
            if (conversation.currentState != Conversation.ConversationState.READ)
            {
                availableConversations.Remove(conversation);
            }
        }

        // loop through conversations
        foreach(Conversation conversation in conversations)
        {

            // check each locked conversation's dependencies. once run counting is implemented that lives here too
            if (conversation.currentState == Conversation.ConversationState.LOCKED)
            {   if(conversation.dependencies.Length > 0)
                {
                    foreach (int i in conversation.dependencies)
                    {
                        bool unlockable = true;
                        if (conversations[i].currentState != Conversation.ConversationState.READ)
                        {
                            unlockable = false;
                        }

                        if (unlockable == true)
                        {
                            conversation.Unlock();
                        }
                    }
                }
            }

            // check each conversation, add to available conversations if unread
            if(conversation.currentState == Conversation.ConversationState.UNREAD && !availableConversations.Contains(conversation))
            {
                availableConversations.Add(conversation);
                Debug.Log(conversation.ID + "added to available");
            }

        }     
    }

    public void UnlockConversation(Conversation conversation)
    {
        conversation.Unlock();
    }

    public Conversation GetRandomAvailableConversation()
    {
        return availableConversations[Random.Range(0, availableConversations.Count)];
    }

    public bool HasAvailable()
    {
        UpdateCalls();
        if (availableConversations.Count <= 0)
        {
            return false;
        }
        return true;
    }
}
