/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - February 3rd, 2022
 * Last Edited - February 3rd, 2022 by Ben Schuster
 * Description - Core data for any resource bar, such as healthbar data. 
 * ================================================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum BarType
{
    NA,
    Health,
    Shield
}

[CreateAssetMenu(fileName = "New Resource Bar", menuName = "Gameplay/Resource Bar")]
public class ResourceBarSO : ScriptableObject
{
    [Header("Core Resourcebar Information")]

    [Tooltip("What type of bar is this? Determines recovery sources.")]
    public BarType _type;
    [Tooltip("The maximum value this bar will start at.")]
    public float _maxValue;


    [Header("Natural Regen Information")]

    [Tooltip("Whether or not this bar can regenerate.")]
    public bool _regen;
    [HideIf("@this._regen == false")]
    [Tooltip("Whether or not this bar's regeneration is stopped by timestop.")]
    public bool _affectedByTimestop;
    [HideIf("@this._regen == false")]
    [Tooltip("How fast this bar regenerates to full. " +
        "Enter a value in seconds it will take to fully regenerate.")]
    public float _regenRate;
    [HideIf("@this._regen == false")]
    [Tooltip("How long it takes to not take damage to begin regenerating. " +
        "Enter a value in seconds it will take.")]
    public float _regenDelay;

    [Header("UI display Information")]

    [Tooltip("If this healthbar has a UI, the color of its fill.")]
    public Color _fillColor;
    [Tooltip("If this healthbar has UI, the color of its empty.")]
    public Color _emptyColor;
}
