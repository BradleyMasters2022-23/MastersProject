/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - June 21st, 2023 by Ben Schuster
 * Description - Concrete field trigger for tooltips
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TooltipField : TooltipHolder
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
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
