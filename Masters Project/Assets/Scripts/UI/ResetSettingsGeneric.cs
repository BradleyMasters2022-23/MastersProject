/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - Generic settings system using new abstract setting system. Allows for easy saving/loading
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSettingsGeneric : MonoBehaviour
{
    [Tooltip("All settings to reset and save")]
    [SerializeField] UISaveableSetting[] settingOptions;

    /// <summary>
    /// Reset each setting to default
    /// </summary>
    public void ResetToDefault()
    {
        foreach(var o in settingOptions)
        {
            o.ResetSetting();
        }
    }

    /// <summary>
    /// Save each setting
    /// </summary>
    public void Save()
    {
        foreach(var o in settingOptions) 
        {
            o.SaveSetting();
        }
    }

    /// <summary>
    /// when screen closes, save data
    /// </summary>
    private void OnDisable()
    {
        Save();
    }

    private void OnEnable()
    {
        foreach (var o in settingOptions)
        {
            o.LoadSetting();
        }
    }
}
