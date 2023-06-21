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
            Debug.Log("Tooltip " + data.titleText + " reached max limit");
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

        StartCoroutine(titleTextbox.SlowTextLoadRealtime(currentTooltip.titleText, 0.02f));
        StartCoroutine(descriptionTextbox.SlowTextLoadRealtime(currentTooltip.GetPromptText(), 0.01f));

        // Load in an override if possible. otherwise use the default
        imageOverride.CrossFadeAlpha(1, 0.5f, true);
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
        Debug.Log("Display tooltip called");
        animator.SetBool("Open", true);
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

    private void OnDisable()
    {
        dismissTooltip.performed -= DismissCurrentTooltip;
        dismissTooltip.Disable();
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
}
