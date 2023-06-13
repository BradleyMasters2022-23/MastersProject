/* ================================================================================================
 * Author - Ben Schuster   
 * Date Created - June 8th, 2023
 * Last Edited - June 8th, 2023 by Ben Schuster
 * Description - Controls individual contact option on the UI
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContactOptionUI : MonoBehaviour
{
    private Character loadedData;
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Tooltip("Image container for the flash")]
    [SerializeField] private FlashProtocol flashComponent;
    [SerializeField] private Conversation newConvo;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private bool init = false;

    #region Contact Option Funcs

    /// <summary>
    /// Initialize this contact option's data
    /// </summary>
    /// <param name="data">Data to load</param>
    public void InitContact(Character data)
    {
        if (data != null)
            loadedData = data;
        else
        {
            Debug.Log($"{name} Tried to initiate contact, but null data was given!");
            return;
        }
            

        if (thumbnail != null)
            thumbnail.sprite = data.characterThumbnail;
        if (nameText != null)
            nameText.text = data.name;
        if(subtitleText!= null )
            subtitleText.text = data.subtitle;

        InitLogs();

        GetIncomingCall();

        init = true;
    }
    /// <summary>
    /// Try to get an incoming call on open
    /// </summary>
    private void OnEnable()
    {
        if(init)
            GetIncomingCall();
        else
            CheckStatus();
    }
    /// <summary>
    /// Stop flashing when off
    /// </summary>
    private void OnDisable()
    {
        newConvo = null;
        StopFlashing();
    }

    /// <summary>
    /// Get the first available incoming call
    /// </summary>
    public void GetIncomingCall()
    {
        newConvo = null;
        // Reference available calls, see if its available
        // Store one thats available
        foreach (var c in loadedData.allConversations)
        {
            if (CallManager.instance.HasNewCall(c))
            {
                newConvo = c;
            }
        }
        // if theres a conversation, start flashing
        if (newConvo != null)
            StartFlashing();
        else
            StopFlashing();
    }

    /// <summary>
    /// Check if the loaded conversation has been read yet
    /// </summary>
    public void CheckStatus()
    {
        if(!CallManager.instance.HasNewCall(newConvo))
        {
            newConvo = null;
            StopFlashing();
        }
    }

    /// <summary>
    /// Toggle the flash routine on and off
    /// </summary>
    public void StartFlashing()
    {
        flashComponent?.BeginFlash();
        
    }
    public void StopFlashing()
    {
        flashComponent?.StopFlash();

        // Tell each log to stop flashing
        foreach (var o in spawnedObjects.ToArray())
        {
            o.GetComponent<ContactLogUI>()?.CheckConvoStatus();
        }
    }

    #endregion

    #region Log Display

    [Header("Logs")]
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private RectTransform logContainer;

    /// <summary>
    /// Load in all call log data, init them
    /// </summary>
    private void InitLogs()
    {
        // clear remaining logs , if any
        foreach(var o in spawnedObjects.ToArray())
        {
            spawnedObjects.Remove(o);
            Destroy(o);
        }

        // spawn in and init new logs
        foreach(var c in loadedData.allConversations)
        {
            ContactLogUI obj = Instantiate(logPrefab, logContainer).GetComponent<ContactLogUI>();
            obj.SetConvo(c);
            obj.CheckConvoStatus();
            spawnedObjects.Add(obj.gameObject);
        }
    }

    #endregion

    #region Screenflow Funcs

    /// <summary>
    /// Go to this character's call history
    /// </summary>
    public void GoToLog()
    {
        //Debug.Log("Going to call log");

        if (ConvoRefManager.instance == null) return;

        ConvoRefManager.instance.GetCallHistoryUI().OpenContactHistory(loadedData);
        return;
    }

    /// <summary>
    /// Go to a new call. Should only be available if theres a new conversation available
    /// </summary>
    public void GoToNewCall()
    {
        //Debug.Log("Going to new call");

        if (ConvoRefManager.instance == null || newConvo == null) return;

        ConvoRefManager.instance.GetCallUI().OpenScreen(newConvo, ConvoRefManager.instance.CheckCallStatuses);
        return;
    }

    public Character LoadedChar()
    {
        return loadedData;
    }

    #endregion
}
