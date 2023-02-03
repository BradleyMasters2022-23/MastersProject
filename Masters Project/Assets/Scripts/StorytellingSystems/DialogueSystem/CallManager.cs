
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CallManager : MonoBehaviour
{
    public List<Conversation> conversations = new List<Conversation>();
    private List<Conversation> availableConversations = new List<Conversation>();
    public Conversation defaultConversation;
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

    private void Start()
    {
        foreach (Conversation conversation in conversations)
        {
            conversation.Lock();
        }

        conversations[0].Unlock();
        UpdateCalls();
    }

    private void UpdateCalls()
    {
        // loop through conversations
        foreach(Conversation conversation in conversations.ToList<Conversation>())
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
        
        foreach(Conversation conversation in availableConversations.ToList<Conversation>())
        {
            if(conversation.currentState == Conversation.ConversationState.READ)
            {
                availableConversations.Remove(conversation);
            }
        }
    }

    public Conversation GetRandomAvailableConversation()
    {
        return availableConversations[Random.Range(0, availableConversations.Count)];
    }

    public bool HasAvailable()
    {
        UpdateCalls();
        if (availableConversations.Count > 0)
        {
            return true;
        }
        return false;
    }

    public Conversation GetDefault()
    {
        return defaultConversation;
    }
}
