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
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "UI/Tooltip", fileName = "New tooltip")]
public class TooltipSO : ScriptableObject
{
    [Tooltip("Title text for the tooltip")]
    public string titleText;
    [Tooltip("Content text for the tooltip")]
    [TextArea] public string tooltipText;

    [Tooltip("How many times can this tooltip reappear")]
    public int timesToDisplay = 1;
}
