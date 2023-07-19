/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 6th, 2023
 * Last Edited - July 6th, 2023 by Ben Schuster
 * Description - Allows children in scroll fields to automatically update the viewport scroll on select
 * for controllers and arrow-key UI navigation
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingSelect : MonoBehaviour
{
    /// <summary>
    /// The scroll value this object sits at
    /// </summary>
    private float selectValue;
    /// <summary>
    /// Reference to the parent scroll rect
    /// </summary>
    private ScrollRect ScrollRect;
    /// <summary>
    /// reference to this objects rect transform
    /// </summary>
    private RectTransform t;
    /// <summary>
    /// reference to the scroll rect's transform
    /// </summary>
    private RectTransform viewportRect;

    /// <summary>
    /// On enable, calculate the proper scrollbar value for this object and get references
    /// </summary>
    private void OnEnable()
    {
        ScrollRect = GetComponentInParent<ScrollRect>();
        t = GetComponent<RectTransform>();

        InitScrollSelect();
    }

    /// <summary>
    /// Recalculate the scrolling select value for this object
    /// </summary>
    public void InitScrollSelect()
    {
        if (ScrollRect != null)
        {
            viewportRect = ScrollRect.GetComponent<RectTransform>();
            selectValue = 1 - ((float)transform.GetSiblingIndex() / (float)(transform.parent.childCount - 1));
            //Debug.Log($"1 - {transform.GetSiblingIndex()} / {transform.parent.childCount - 1} = {selectValue}");
        }
    }

    /// <summary>
    /// Update the scroll bar to this current value. Called via 'OnSelect' event trigger
    /// </summary>
    public void OnSelect()
    {
        // calculate the positions to check, both top half and bottom half so no options can be selected half-clipped
        Vector3 botPos = t.position;
        botPos.y -= (t.rect.height/2) * viewportRect.lossyScale.y;
        Vector3 topPos = t.position;
        topPos.y += (t.rect.height / 2) * viewportRect.lossyScale.y;

        // if it exists and either the top or bottom arent in screen, update the scrollbar
        if (ScrollRect != null 
            && (!RectTransformUtility.RectangleContainsScreenPoint(viewportRect, botPos) || !RectTransformUtility.RectangleContainsScreenPoint(viewportRect, topPos)))
        {
            ScrollRect.verticalScrollbar.value = selectValue;
        }
    }
}
