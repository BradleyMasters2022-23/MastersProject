using System;
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
        foreach(Conversation conversation in conversations)
        {
            if(conversation.currentState == Conversation.ConversationState.LOCKED)
            {
                foreach(int i in conversation.dependencies)
                {
                    bool a = true;
                    if (conversations[i] != read)
                    {
                        a = false;
                    }

                    if(a == true)
                    {
                        conversation.currentState = Conversation.ConversationState.UNREAD;
                    }
                }
            }

            if(conversation.currentState == Conversation.ConversationState.UNREAD)
            {
                availableConversations.Add(conversation);
            }
        }     
    }

    public NoteObject GetRandomAvailableConversation()
    {
        UpdateCalls();
        Conversation c = Random.Range(0, availableConversations.Count);
        return c;
    }
}
