/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 14th, 2023
 * Last Edited - July 14th, 2023 by Ben Schuster
 * Description - Allows for scroll inputs to be pass throughed to the parent scroll rect.
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollPassthrough : MonoBehaviour, IScrollHandler
{
    private ScrollRect parent;

    private void OnEnable()
    {
        if(parent == null)
        {
            parent = GetComponentInParent<ScrollRect>();

            // if no scroll rect, just disable itself
            if (parent == null) enabled = false;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        Debug.Log("Scrolling");
        parent.OnScroll(eventData);
    }
}
