
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class CallManager : MonoBehaviour
{
    public List<Conversation> conversations = new List<Conversation>();
    [SerializeField, ReadOnly, HideInEditorMode] 
    protected List<Conversation> availableConversations = new List<Conversation>();
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Try to get save data
        saveData = DataManager.instance.Load<DialogueSaveData>(saveFileName);
        if (saveData == null)
            saveData = new DialogueSaveData();

        UpdateCalls();
    }

    private void UpdateCalls()
    {
        // loop through conversations
        foreach(Conversation conversation in conversations.ToList<Conversation>())
        {

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

        UpdateCalls();
        Debug.Log("Calls Reset");
    }

    /// <summary>
    /// Increment the runs of conversations and save
    /// </summary>
    public void IncrementRuns(InputAction.CallbackContext c = default)
    {
        GlobalStatsManager.data.runsAttempted++;
        saveData?.IncrementRuns();
        bool s = DataManager.instance.Save(saveFileName, saveData);
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
        if (availableConversations.Contains(c))
            availableConversations.Remove(c);

        bool s = DataManager.instance.Save(saveFileName, saveData);
        //Debug.Log($"Conversation save successful : {s}");
        //saveData.SeeAllReads();
    }

    public void ResetData()
    {
        saveData = new DialogueSaveData();
        UpdateCalls();
    }

    public bool HasNewCall(Conversation c)
    {
        return availableConversations.Contains(c);
    }

    /// <summary>
    /// Check if call is found. Do here to keep save data only here.
    /// </summary>
    /// <param name="c">Call to check</param>
    /// <returns>Whether its in the save</returns>
    public bool CallInSave(Conversation c)
    {
        return saveData.AlreadyRead(c);
    }

    /// <summary>
    /// get the count of available conversations
    /// </summary>
    /// <returns></returns>
    public int AvailableCallCount()
    {
        if (availableConversations == null)
            return 0;

        return availableConversations.Count;
    }
}
