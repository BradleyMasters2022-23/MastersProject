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

        tooltipHistory = new Dictionary<TooltipSO, int>();

        // hide tooltip on load
        HideTooltip();

        // make it 8 for simplicity. The queue shouldnt be used often if at all
        //tooltipQueue = new Queue<TooltipSO>(8);
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
        if(tooltipHistory.ContainsKey(data) && tooltipHistory[data] >= data.timesToDisplay)
        {
            return;
        }

        // increment dictionary with display count
        if (tooltipHistory.ContainsKey(data)) tooltipHistory[data] += 1;
        else tooltipHistory.Add(data, 1);

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
        titleTextbox.text = data.titleText;
        descriptionTextbox.text = data.tooltipText;

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

            // hide tooltip after being loaded
            HideTooltip();
        }
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

    #endregion

    #region Dismiss Current Tooptip
    
    private GameControls controls;
    private InputAction dismissTooltip;

    /// <summary>
    /// Prepare input controls
    /// </summary>
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

    #region Queue System
    /// <summary>
    /// Current queue of tooltips
    /// </summary>
    //private Queue<TooltipSO> tooltipQueue;
    //private IEnumerator TooltipUpdate()
    //{
    //    WaitForSeconds tooltipTickRate = new WaitForSeconds(2f);

    //    while(true)
    //    {


    //        yield return tooltipTickRate;
    //        yield return null;
    //    }

    //    yield return null;
    //}
    #endregion
}
