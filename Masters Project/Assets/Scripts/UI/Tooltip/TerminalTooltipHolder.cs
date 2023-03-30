/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Specialized tooltip holder for the terminal.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalTooltipHolder : TooltipField
{
    private bool performed = false;
    [SerializeField] private ConversationInteract interact;

    /// <summary>
    /// Only submit tooltip if not performed.
    /// Subscribe to interaction manager
    /// </summary>
    protected override void SubmitTooltip()
    {
        if (performed)
            return;

        base.SubmitTooltip();
        interact.onStartCall += TooltipExecuted;
    }

    /// <summary>
    /// When the interaction is executed, unsubscribe from
    /// the conversation terminal and hide the tooltip
    /// </summary>
    public void TooltipExecuted()
    {
        interact.onStartCall -= TooltipExecuted;
        performed = true;
        RetractTooltip();
    }
}
