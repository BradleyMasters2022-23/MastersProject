using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartExpandContent : MonoBehaviour
{
    private RectTransform container;
    private RectTransform[] children;
    private VerticalLayoutGroup layout;

    public void CalculateHeight()
    {
        // Adjust the size of the container based on the amount of active children in it
        // needed because unity wont allow this otherwise. 

        container = GetComponent<RectTransform>();
        layout = GetComponent<VerticalLayoutGroup>();
        children = GetComponentsInChildren<RectTransform>();


        float totalHeight = 0;
        int activeChildren = 0;
        foreach(RectTransform child in children)
        {
            if (child.parent != container)
                continue;

            if(child.gameObject.activeInHierarchy)
            {
                totalHeight += child.rect.height;
                activeChildren++;
            }
                
        }

        totalHeight += (layout.spacing * (activeChildren-1));
        totalHeight += layout.padding.top + layout.padding.bottom;
        // only apply if it's larger than original 
        if (container.sizeDelta.y <= totalHeight)
            container.sizeDelta = new Vector2(container.sizeDelta.x, totalHeight);
    }
}
