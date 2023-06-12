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
    [SerializeField] private Image flashOverlay;
    [SerializeField] private Color flashColor1;
    [SerializeField] private Color flashColor2;
    [Tooltip("Interval for the flash")]
    [SerializeField] private float flashInterval;

    private Coroutine flashingRoutine;
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

        init = true;
        
        GetIncomingCall();
    }
    /// <summary>
    /// Try to get an incoming call on open
    /// </summary>
    private void OnEnable()
    {
        if(init)
            GetIncomingCall();
    }
    /// <summary>
    /// Stop flashing when off
    /// </summary>
    private void OnDisable()
    {
        newConvo = null;

        if (flashingRoutine != null)
            StopCoroutine(flashingRoutine);
    }

    /// <summary>
    /// Get the first available incoming call
    /// </summary>
    public void GetIncomingCall()
    {
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
            Stopflashing();
    }

    /// <summary>
    /// Begin the flashing routine
    /// </summary>
    private void StartFlashing()
    {
        flashingRoutine = StartCoroutine(FlashRoutine());
    }
    private IEnumerator FlashRoutine()
    {
        flashOverlay.color = flashColor1;
        flashOverlay.gameObject.SetActive(true);

        Color currColor = flashColor1;
        while(CallManager.instance.HasNewCall(newConvo))
        {
            yield return new WaitForSecondsRealtime(flashInterval);

            currColor = (currColor == flashColor1) ? flashColor2 : flashColor1;
            flashOverlay.color = currColor;
            yield return null;           
        }
        flashOverlay.gameObject.SetActive(false);
    }
    private void Stopflashing()
    {
        StopAllCoroutines();
        flashOverlay.gameObject.SetActive(false);
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

        ConvoRefManager.instance.GetCallUI().OpenScreen(newConvo);
        return;
    }

    public Character LoadedChar()
    {
        return loadedData;
    }

    #endregion
}
