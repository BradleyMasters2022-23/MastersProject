/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Concrete field trigger for tooltips
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipField : TooltipHolder
{
    private bool sent = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!sent && other.CompareTag("Player"))
        {
            sent = true;
            SubmitTooltip();
        }
            
    }

    private void OnDisable()
    {
        //RetractTooltip();
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //    RetractTooltip();
    }
}
