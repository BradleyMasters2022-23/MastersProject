/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Manager for processing tooltips
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Masters.UI;

public class TooltipManager : MonoBehaviour
{
    #region Main Tooltip Functionality

    public static TooltipManager instance;

    [Tooltip("Reference to the textbox panel containing everything.")]
    [SerializeField, Required] private GameObject tooltipPanel;
    [Tooltip("Reference to the textbox that displays the title.")]
    [SerializeField, Required] private TextMeshProUGUI titleTextbox;
    [Tooltip("Reference to the textbox that displays the description.")]
    [SerializeField, Required] private TextMeshProUGUI descriptionTextbox;
    [Tooltip("Reference to the image that can be overriden")]
    [SerializeField, Required] private Image imageOverride;
    [Tooltip("Default sprite that displays")]
    [SerializeField, Required] private Sprite defaultSprite;
    /// <summary>
    /// Currently loaded tooltip
    /// </summary>
    private TooltipSO currentTooltip;

    private const string fileName = "tooltipData";
    private TooltipSaveData saveData;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioClipSO openSound;
    [SerializeField] private AudioSource source;

    private Coroutine loadTitleRoutine;
    private Coroutine loadBodyRoutine;

    /// <summary>
    /// Verify tooltip loaded is prepared
    /// </summary>
    private void Awake()
    {
        if(tooltipPanel == null || titleTextbox == null || descriptionTextbox == null 
            || imageOverride == null || defaultSprite == null || instance != null)
        {
            Debug.LogError("[TOOLTIP] Tooltip is not set up correctly in inspector!");
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        // Get save data
        saveData = DataManager.instance.Load<TooltipSaveData>(fileName);
        if (saveData == null)
            saveData = new TooltipSaveData();
        // hide tooltip on load
        HideTooltip();
    }

    /// <summary>
    /// Request a tooltip to be loaded
    /// </summary>
    /// <param name="data">tooltip data to be requested</param>
    public void RequestTooltip(TooltipSO data, bool checkWithSave)
    {
        StartCoroutine(LoadTooltipRoutine(data, checkWithSave));
    }

    /// <summary>
    /// Process the tooltip load. Do in a routine so we can wait for frame for animator
    /// </summary>
    /// <param name="data"></param>
    /// <param name="checkWithSave"></param>
    /// <returns></returns>
    private IEnumerator LoadTooltipRoutine(TooltipSO data, bool checkWithSave)
    {
        // verify request is not a null
        if (data == null)
        {
            Debug.Log("[TOOLTIP] A tooltip was requested but passed in a null reference.");
            yield break;
        }

        if (data == currentTooltip)
        {
            yield break;
        }
        else if (currentTooltip != null)
        {
            // wait for frame to let the animator catch up
            UnloadTooltip();
            yield return new WaitForEndOfFrame();
        }

        // If set to check with save, check if a limit has been reached
        if (checkWithSave && saveData.LimitReached(data))
        {
            // Verify it has not exceeded its display count max
            //Debug.Log("Tooltip " + data.titleText + " reached max limit");
            yield break;
        }

        // update save data. Do this outside of check with save so it can still be found by the tooltip lookup UI
        saveData.IncrementTooltip(data);
        bool r = DataManager.instance.Save(fileName, saveData);
        //Debug.Log($"Tooltip saving successful: {r}");

        // save new tooltip, tell it to display. Reset stats now for smoother tranistion in animator
        currentTooltip = data;
        imageOverride.sprite = (currentTooltip.spriteOverride != null) ? currentTooltip.spriteOverride : defaultSprite;
        titleTextbox.text = "";
        descriptionTextbox.text = "";
        DisplayTooltip();
    }

    /// <summary>
    /// Load in a tooltip and display it. Called by animator
    /// </summary>
    public void LoadTooltip()
    {
        if(currentTooltip == null)
        {
            HideTooltip();
            return;
        }

        openSound.PlayClip(source);
        
        loadTitleRoutine = StartCoroutine(titleTextbox.SlowTextLoadRealtime(currentTooltip.titleText, 0.02f));
        loadBodyRoutine = StartCoroutine(LoadTooltipText(currentTooltip));

        // Load in an override if possible. otherwise use the default
        //imageOverride.CrossFadeAlpha(1, 0.5f, true);
        imageOverride.sprite = (currentTooltip.spriteOverride != null) ? currentTooltip.spriteOverride : defaultSprite;

        // display tooltip after being loaded
    }

    /// <summary>
    /// Unload a tooltip, if its active
    /// </summary>
    /// <param name="data">tooltip data to unload</param>
    public void UnloadTooltip(TooltipSO data)
    {
        // verify the requested tooltip to unload is the current one
        if (data == currentTooltip)
        {
            if (loadTitleRoutine != null)
                StopCoroutine(loadTitleRoutine);
            if (loadBodyRoutine != null)
                StopCoroutine(loadBodyRoutine);

            currentTooltip= null;
            titleTextbox.text = "";
            descriptionTextbox.text = "";

            imageOverride.CrossFadeAlpha(0, 0.5f, true);
            titleTextbox.text = "";
            descriptionTextbox.text = "";

            // hide tooltip after being loaded
            animator.SetBool("Open", false);
        }
    }

    /// <summary>
    /// Unload any tooltip active
    /// </summary>
    public void UnloadTooltip()
    {
        UnloadTooltip(currentTooltip);
    }

    /// <summary>
    /// Display tooltip on HUD, start is animation
    /// </summary>
    private void DisplayTooltip()
    {
        animator.SetBool("Open", true);
        imageOverride.sprite = (currentTooltip.spriteOverride != null) ? currentTooltip.spriteOverride : defaultSprite;
    }

    /// <summary>
    /// Hide tooltip on HUD immediately
    /// </summary>
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    /// <summary>
    /// Whether the tooltip is currently loaded 
    /// </summary>
    /// <param name="tooltipToCheck"></param>
    /// <returns></returns>
    public bool HasTooltip(TooltipSO tooltipToCheck)
    {
        return currentTooltip == tooltipToCheck;
    }
    /// <summary>
    /// Get the save data reference
    /// </summary>
    /// <returns></returns>
    public TooltipSaveData GetSaveData()
    {
        return saveData;
    }

    #endregion

    #region Dismiss Current Tooptip
    
    private GameControls controls;
    private InputAction dismissTooltip;

    private void Start()
    {
        controls = GameManager.controls;
        dismissTooltip = controls.PlayerGameplay.DismissTooltip;
        dismissTooltip.performed += DismissCurrentTooltip;
        dismissTooltip.Enable();
    }

    private void OnEnable()
    {
        onSchemeChangeChannel.OnEventRaised += InstantReloadTooltip;
    }
    private void OnDisable()
    {
        dismissTooltip.performed -= DismissCurrentTooltip;
        dismissTooltip.Disable();

        onSchemeChangeChannel.OnEventRaised -= InstantReloadTooltip;

    }
    
    /// <summary>
    /// Input call to dismiss the currently loaded tooltip
    /// </summary>
    /// <param name="c"></param>
    private void DismissCurrentTooltip(InputAction.CallbackContext c)
    {
        if (currentTooltip != null)
            UnloadTooltip(currentTooltip);
    }

    private void OnLevelWasLoaded(int level)
    {
        HideTooltip();
    }
    #endregion

    #region InputAction Lookup

    [SerializeField] ChannelControlScheme onSchemeChangeChannel;

    private IEnumerator LoadTooltipText(TooltipSO data)
    {
        // create delay here to minimize 'new' keyword
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.01f);

        // Loop through each character, updating the text team time
        // Wait for the delay for each character
        string text = data.GetPromptText();
        string fullTxt = "";
        int inputIdx = 0;
        char delimiter = InputManager.Instance.inputDelimiter;

        for (int i = 0; i < text.Length; i++)
        {
            // Check for a richtext tag
            if (text[i] == '<')
            {
                // Build a string until hitting end of the richtext '>'
                string richtextBuffer = "<";
                int j = i + 1;
                while (text[j] != '>' && j < text.Length)
                {
                    richtextBuffer += text[j];
                    j++;
                    yield return null;
                }
                richtextBuffer += '>';

                // Apply it to the text, update the i index so it doesnt re-read it
                fullTxt += richtextBuffer;
                descriptionTextbox.text = fullTxt;
                i = j;

                // dont wait for a delay, just keep going
                yield return null;
            }
            // If an input, get lookup and add here
            else if (text[i] == delimiter && inputIdx < data.inputReference.Length)
            {
                fullTxt += InputManager.Instance.ActionKeybindLookup(data.inputReference[inputIdx]);
                inputIdx++;
            }
            // If normal text, just add it to the screen
            else
            {
                fullTxt += text[i];
                descriptionTextbox.text = fullTxt;
                yield return delay;
            }
        }

        // Just to be sure, assign the text to the full thing at the end
        //descriptionTextbox.text = text;
        yield return null;
    }

    /// <summary>
    /// Instantly reload the tooltip prompt with the updated input action binding string
    /// </summary>
    private void InstantReloadTooltip(InputManager.ControlScheme c)
    {
        // if no input references or not enabled, then dont do anything. 
        if (currentTooltip == null || currentTooltip.inputReference.Length <= 0 || !descriptionTextbox.isActiveAndEnabled) return;

        // cancel anything currently loading
        if (loadBodyRoutine != null)
            StopCoroutine(loadBodyRoutine);

        // loop through the text to be loaded, insert input action bindings where needed
        string contentRef = currentTooltip.GetPromptText();
        string temp = "";
        int inputIdx = 0;
        // iterate through content, adding any input bindings when necessary
        for (int i = 0; i < contentRef.Length; i++)
        {
            if (contentRef[i] == InputManager.Instance.inputDelimiter)
            {
                temp += InputManager.Instance.ActionKeybindLookup(currentTooltip.inputReference[inputIdx]);
                inputIdx++;
            }
            else
            {
                temp += contentRef[i];
            }
        }
        descriptionTextbox.text = temp;
    }

    #endregion
}
