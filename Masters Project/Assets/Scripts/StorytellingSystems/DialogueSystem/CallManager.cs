
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
            {   if(conversation.dependencies.Count > 0)
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

    public Conversation GetRandomAvailableConversation()
    {
        UpdateCalls();
        Conversation c = availableConversations[Random.Range(0, availableConversations.Count)];
        return c;
    }
}
