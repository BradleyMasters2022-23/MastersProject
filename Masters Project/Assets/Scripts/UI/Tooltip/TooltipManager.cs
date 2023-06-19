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

    /// <summary>
    /// Currently loaded tooltip
    /// </summary>
    private TooltipSO currentTooltip;
    private Dictionary<TooltipSO, int> tooltipHistory;

    private const string fileName = "tooltipData";
    private TooltipSaveData saveData;

    /// <summary>
    /// Verify tooltip loaded is prepared
    /// </summary>
    private void Awake()
    {
        if(tooltipPanel == null || titleTextbox == null || descriptionTextbox == null 
            || instance != null)
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
    public void RequestTooltip(TooltipSO data)
    {
        // verify request is not a null
        if(data == null)
        {
            Debug.Log("[TOOLTIP] A tooltip was requested but passed in a null reference.");
            return;
        }

        // Verify it has not exceeded its display count max
        if (saveData.LimitReached(data))
        {
            Debug.Log("Tooltip " + data.titleText + " reached max limit");
            return;
        }

        // update save data
        saveData.IncrementTooltip(data);
        bool r = DataManager.instance.Save(fileName, saveData);
        Debug.Log($"Tooltip saving successful: {r}");

        // for now, load the data
        LoadTooltip(data);
    }

    /// <summary>
    /// Load in a tooltip and display it
    /// </summary>
    /// <param name="data">tooltip data to be loaded</param>
    private void LoadTooltip(TooltipSO data)
    {
        currentTooltip = data;

        StartCoroutine(titleTextbox.SlowTextLoad(data.titleText, 0.02f));
        StartCoroutine(descriptionTextbox.SlowTextLoad(data.tooltipText, 0.01f));

        // display tooltip after being loaded
        DisplayTooltip();
    }

    /// <summary>
    /// Unload a tooltip, if its active
    /// </summary>
    /// <param name="data">tooltip data to unload</param>
    public void UnloadTooltip(TooltipSO data)
    {
        // verify the requested tooltip to unload is the current one
        if(data == currentTooltip)
        {
            currentTooltip= null;
            titleTextbox.text = "";
            descriptionTextbox.text = "";

            StartCoroutine(titleTextbox.SlowTextUnload(0.005f));
            StartCoroutine(descriptionTextbox.SlowTextUnload(0.005f));

            // hide tooltip after being loaded
            HideTooltip();
        }
    }

    /// <summary>
    /// Unload any tooltip active
    /// </summary>
    public void UnloadTooltip()
    {
        currentTooltip = null;
        titleTextbox.text = "";
        descriptionTextbox.text = "";

        HideTooltip();
    }


    /// <summary>
    /// Display tooltip on HUD
    /// </summary>
    private void DisplayTooltip()
    {
        tooltipPanel.SetActive(true);
    }
    /// <summary>
    /// Hide tooltip on HUD
    /// </summary>
    private void HideTooltip()
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
    #endregion
}
