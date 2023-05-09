
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CallManager : MonoBehaviour
{
    public List<Conversation> conversations = new List<Conversation>();
    [SerializeField] private List<Conversation> availableConversations = new List<Conversation>();
    public Conversation defaultConversation;
    public static CallManager instance;

    public static GameControls controls;
    private InputAction backslash;
    private InputAction forwardSlash;

    private DialogueSaveData saveData;
    private const string saveFileName = "conversationSaveData";

    private void Awake()
    {
        controls = new GameControls();
        backslash = controls.PlayerGameplay.ResetConversations;
        backslash.performed += ResetCalls;
        backslash.Enable();
        forwardSlash = controls.PlayerGameplay.IncrementConvs;
        forwardSlash.performed += IncrementRuns;
        forwardSlash.Enable();


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
        // Try to get save data
        saveData = DataManager.instance.Load<DialogueSaveData>(saveFileName);
        if (saveData == null)
            saveData = new DialogueSaveData();
        //saveData.SeeAllReads();


        //foreach (Conversation conversation in conversations)
        //{
        //    conversation.Lock();
        //}

        //conversations[0].Unlock();
        //if(conversations.Count >= 4)
        //    conversations[3].Unlock();


        UpdateCalls();
    }

    private void UpdateCalls()
    {
        // loop through conversations
        foreach(Conversation conversation in conversations.ToList<Conversation>())
        {
            /*
            // check each locked conversation's dependencies. once run counting is implemented that lives here too
            if (conversation.currentState == Conversation.ConversationState.LOCKED)
            {
                if (conversation.dependencies.Length > 0)
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
            if (conversation.currentState == Conversation.ConversationState.UNREAD && !availableConversations.Contains(conversation))
            {
                availableConversations.Add(conversation);
                //Debug.Log(conversation.ID + " added to available");
            }
            */

            // Check saved data if the dependencies have been satisfied
            if (saveData.CheckDependencies(conversation) 
                && !saveData.AlreadyRead(conversation)
                && !availableConversations.Contains(conversation))
            {
                //Debug.Log($"Adding {conversation} to list");
                availableConversations.Add(conversation);
            }

        }
        
        foreach(Conversation conversation in availableConversations.ToList<Conversation>())
        {
            if(saveData.AlreadyRead(conversation))
            {
                //Debug.Log($"Removing {conversation} from list bc it was already read");
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

    public void ResetCalls(InputAction.CallbackContext c = default)
    {
        // delete save data for conversaitons
        DataManager.instance.Delete(saveFileName);
        saveData = new DialogueSaveData();
        saveData.SeeAllReads();

        availableConversations.Clear();

        //foreach (Conversation conversation in conversations)
        //{
        //    conversation.Lock();
        //}

        //conversations[0].Unlock();
        //if (conversations.Count >= 4)
        //    conversations[3].Unlock();

        UpdateCalls();
        Debug.Log("Calls Reset");
    }

    /// <summary>
    /// Increment the runs of conversations and save
    /// </summary>
    public void IncrementRuns(InputAction.CallbackContext c = default)
    {
        GlobalStatsManager.data.runsAttempted++;
        saveData.IncrementRuns();
        bool s = DataManager.instance.Save<DialogueSaveData>(saveFileName, saveData);
        //saveData.SeeAllReads();
        //Debug.Log($"Increment runs saved successfully : {s}");
    }

    /// <summary>
    /// Mark a conversation as read
    /// </summary>
    /// <param name="c">Conversation to mark as read</param>
    public void SaveData(Conversation c)
    {
        saveData.ConversationRead(c);
        bool s = DataManager.instance.Save<DialogueSaveData>(saveFileName, saveData);
        //Debug.Log($"Conversation save successful : {s}");
        //saveData.SeeAllReads();
    }

    public void ResetData()
    {
        saveData = new DialogueSaveData();
        UpdateCalls();
    }
}
