/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - March 29th, 2023
 * Last Edited - March 29th, 2023 by Ben Schuster
 * Description - Scriptable object for tooltip data
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "UI/Tooltip", fileName = "New tooltip")]
public class TooltipSO : ScriptableObject
{
    [Tooltip("Title text for the tooltip")]
    public string titleText;
    [Tooltip("Content text for the tooltip that plays on screen")]
    [SerializeField, TextArea] private string tooltipText;

    [Tooltip("Content text for the tooltip on saved lookup. If empty, will use Tooltip Text instead.")]
    [SerializeField, TextArea] private string savedTooltipText;

    [Tooltip("How many times can this tooltip reappear")]
    public int timesToDisplay = 1;

    [Tooltip("Whether or not this should display in tooltip list")]
    public bool showInTooltipList = true;

    [Tooltip("A sprite to override the default image. Leave blank to not use")]
    public Sprite spriteOverride;

    [Tooltip("Reference to input action this tooltip references, if any")]
    public InputActionReference[] inputReference;

    /// <summary>
    /// Get the text to show on the immediate prompt
    /// </summary>
    /// <returns></returns>
    public string GetPromptText()
    {
        return tooltipText;
    }

    /// <summary>
    /// Get the text to show in the saved lookup
    /// </summary>
    /// <returns></returns>
    public string GetSavedText()
    {
        if (savedTooltipText == "")
            return tooltipText;
        else
            return savedTooltipText;
    }
}
