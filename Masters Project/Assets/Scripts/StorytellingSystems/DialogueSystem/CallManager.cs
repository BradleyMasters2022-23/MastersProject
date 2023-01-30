
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

        UnlockConversation(conversations[0]);
    }

    private void UpdateCalls()
    {
        foreach(Conversation conversation in conversations)
        {
            if(conversation.currentState == Conversation.ConversationState.LOCKED)
            {   if(conversation.dependencies.Length > 0)
                {
                    foreach (int i in conversation.dependencies)
                    {
                        bool a = true;
                        if (conversations[i].currentState != Conversation.ConversationState.READ)
                        {
                            a = false;
                        }

                        if (a == true)
                        {
                            conversation.currentState = Conversation.ConversationState.UNREAD;
                        }
                    }
                }
            }

            if(conversation.currentState == Conversation.ConversationState.UNREAD)
            {
                availableConversations.Add(conversation);
            }
        }     
    }

    public void UnlockConversation(Conversation conversation)
    {
        conversation.Unlock();
    }

    public Conversation GetRandomAvailableConversation()
    {
        UpdateCalls();
        //Debug.Log(conversations.Count);
        return availableConversations[Random.Range(0, availableConversations.Count)];
    }
}
