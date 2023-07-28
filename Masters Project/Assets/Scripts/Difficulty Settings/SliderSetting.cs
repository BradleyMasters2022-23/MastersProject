/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - Concrete save setting managed by slider elements
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SliderSetting : UISaveableSetting
{
    [Header("Slider Settings")]

    [Tooltip("Slider to observe")]
    [SerializeField] protected Slider targetSlider;
    [Tooltip("Min/Max values for this setting")]
    [SerializeField] protected Vector2 valueRangeMinMax;

    /// <summary>
    /// set slider value to current value
    /// </summary>
    protected override void UpdateUIElement()
    {
        targetSlider.value = currentValue;
    }

    /// <summary>
    /// On slider interact, set currentl value to current value
    /// </summary>
    public override void OnUIInteract()
    {
        currentValue = targetSlider.value;
    }

    /// <summary>
    /// On validate, clamp the range so it only needs to be set in one place
    /// </summary>
    private void OnValidate()
    {
        if (targetSlider != null)
        {
            targetSlider.minValue = valueRangeMinMax.x;
            targetSlider.maxValue = valueRangeMinMax.y;
            targetSlider.value = defaultValue;
        }
    }
}
