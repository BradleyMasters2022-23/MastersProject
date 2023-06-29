/* ================================================================================================
 * Author - Soma   
 * Date Created - October, 2022
 * Last Edited - June 13th, 2023 by Ben Schuster
 * Description - Manages all conversations the player has access to, loading them and saving them
 * ================================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class CallManager : MonoBehaviour
{
    /// <summary>
    /// manager instance
    /// </summary>
    public static CallManager instance;

    [Tooltip("All characters with available to call with")]
    public List<Character> characters = new List<Character>();
    [Tooltip("All conversations available at this moment")]
    [SerializeField, ReadOnly, HideInEditorMode] 
    protected List<Conversation> availableConversations = new List<Conversation>();
    /// <summary>
    /// List of characters with complete stories
    /// </summary>
    [SerializeField, ReadOnly, HideInEditorMode]
    private List<Character> randomChatOptions = new List<Character>();

    /// <summary>
    /// Cheat controls
    /// </summary>
    public static GameControls controls;
    private InputAction backslash;
    private InputAction forwardSlash;

    /// <summary>
    /// Save data 
    /// </summary>
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

    /// <summary>
    /// Get an updated list of conversations available
    /// </summary>
    private void UpdateCalls()
    {
        // loop through character conversations.
        foreach(Character character in characters.ToList())
        {
            bool oneFound = false;
            foreach (Conversation conversation in character.allConversations)
            {
                // Check saved data if the dependencies have been satisfied
                if (saveData.CheckDependencies(conversation) && !saveData.AlreadyRead(conversation))
                {
                    oneFound = true;
                    // prevent dupes from being added
                    if(!availableConversations.Contains(conversation))
                        availableConversations.Add(conversation);
                }
            }

            // If a character doesn't have any new talks available (due to complete or locked), add them to the 'small talk' option list
            if (!oneFound && character.CharacterMet(this))
                randomChatOptions.Add(character);
        }
    }
    /// <summary>
    /// Get a random conversation
    /// </summary>
    /// <returns></returns>
    public Conversation GetRandomAvailableConversation()
    {
        if(availableConversations.Count > 0) // If new chats available, get this instead
        {
            Debug.Log("Active call found, loading");
            return availableConversations[Random.Range(0, availableConversations.Count)];
        }
        else // if no options, get a random final choice from list of complete characters
        {
            Debug.Log("No call found, loading misc");
            return randomChatOptions[Random.Range(0, randomChatOptions.Count)].repeatableConversations.Pull();
        }
    }

    public bool HasAvailable()
    {
        //UpdateCalls();
        if (availableConversations.Count > 0)
        {
            return true;
        }
        return false;
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
        UpdateCalls();
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
