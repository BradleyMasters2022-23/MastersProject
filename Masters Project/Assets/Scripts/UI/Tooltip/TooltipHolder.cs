/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Base class for a tooltip holder that will send a tooltip request to the manager
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TooltipHolder : MonoBehaviour
{
    [Header("Tooltip")]

    [Tooltip("Tooltip to display")]
    [SerializeField] protected TooltipSO tooltip;

    [Tooltip("Max time this tooltip is displayed for. Set to 0 to not use display time")]
    [SerializeField] private float tooltipDisplayTime;
    /// <summary>
    /// timer tracking the display time for the tooltip
    /// </summary>
    private ScaledTimer displayTimer;

    /// <summary>
    /// reference to the tooltip manager
    /// </summary>
    protected TooltipManager manager;

    protected virtual void Start()
    {
        if (tooltip == null)
        {
            Destroy(this);
            return;
        }
        manager = TooltipManager.instance;
        displayTimer = new ScaledTimer(tooltipDisplayTime, false);
    }

    /// <summary>
    /// Ask the manager to display the tooltip
    /// </summary>
    protected virtual void SubmitTooltip()
    {
        manager?.RequestTooltip(tooltip);

        // Set timer incase 
        if (tooltipDisplayTime > 0)
            StartCoroutine(WaitToUnload());
    }

    /// <summary>
    /// Ask the manager to retract the tooltip
    /// </summary>
    protected virtual void RetractTooltip()
    {
        manager?.UnloadTooltip(tooltip);
    }

    /// <summary>
    /// Try to unload the tooltip after a given time
    /// </summary>
    /// <returns></returns>
    protected IEnumerator WaitToUnload()
    {
        displayTimer.ResetTimer();

        yield return new WaitUntil(displayTimer.TimerDone);

        manager?.UnloadTooltip(tooltip);
    }
}
