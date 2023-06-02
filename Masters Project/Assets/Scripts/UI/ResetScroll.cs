/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - June 1st, 2022
 * Last Edited - June 1st, 2022 by Ben Schuster
 * Description - Reset a scroll bar position when it is closed so its correct on next open
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ResetScroll : MonoBehaviour
{
    /// <summary>
    /// scrollbar ref
    /// </summary>
    private Scrollbar bar;
    
    void OnDisable()
    {
        // If not already acquired, get it
        if (bar == null)
            bar = GetComponent<Scrollbar>();

        bar.value = 1; // 1 is top, 0 is bottom
    }
}
