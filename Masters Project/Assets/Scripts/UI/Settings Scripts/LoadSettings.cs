/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 31th, 2022
 * Last Edited - October 31th, 2022 by Ben Schuster
 * Description - Load in the settings for the game
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSettings : MonoBehaviour
{
    [Header("Default Values")]

    [Tooltip("Default value for master volume")]
    [SerializeField] private float defaultMasterVolume;
    [Tooltip("Default value for mouse sensitivity")]
    [SerializeField] private float defaultMouseSensitivity;
    [Tooltip("Default value for x inversion")]
    [SerializeField] private bool defaultMouseInvertX;
    [Tooltip("Default value for y inversion")]
    [SerializeField] private bool defaultMouseInvertY;
    [Tooltip("Default value for mouse having aimassist")]
    [SerializeField] private bool defaultMouseAimAssist;

    [Tooltip("Default value for mouse sensitivity")]
    [SerializeField] private float defaultControllerSensitivity;
    [Tooltip("Default value for x inversion")]
    [SerializeField] private bool defaultControllerInvertX;
    [Tooltip("Default value for y inversion")]
    [SerializeField] private bool defaultControllerInvertY;
    [Tooltip("Default value for controller aim assist")]
    [SerializeField] private bool defaultControllerAimAssist;

    /// <summary>
    /// Load in the saved player settings
    /// </summary>
    private void Awake()
    {
        // Load Camera Option's settings
        //Settings.masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);

        Settings.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
        Settings.mouseInvertX = IntToBool(PlayerPrefs.GetInt("MouseInvertX", BoolToInt(defaultMouseInvertX)));
        Settings.mouseInvertY = IntToBool(PlayerPrefs.GetInt("MouseInvertY", BoolToInt(defaultMouseInvertY)));
        Settings.mouseAimAssist = IntToBool(PlayerPrefs.GetInt("MouseAimAssist", BoolToInt(defaultMouseAimAssist)));

        Settings.controllerSensitivity = PlayerPrefs.GetFloat("ControllerSensitivity", defaultControllerSensitivity);
        Settings.controllerInvertX = IntToBool(PlayerPrefs.GetInt("ControllerInvertX", BoolToInt(defaultControllerInvertX)));
        Settings.controllerInvertY = IntToBool(PlayerPrefs.GetInt("ControllerInvertY", BoolToInt(defaultControllerInvertY)));
        Settings.controllerAimAssist = IntToBool(PlayerPrefs.GetInt("ControllerAimAssist", BoolToInt(defaultControllerAimAssist)));

        Debug.Log("Settings loaded by " + gameObject.name);

        // Self destruct once complete
        Destroy(this);
    }

    #region Utility

    /// <summary>
    /// Converts int to bool (for using stored playerprefs)
    /// </summary>
    private bool IntToBool(int toConvert)
    {
        if (toConvert == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Converts bool to int (for playerpref storage)
    /// </summary>
    private int BoolToInt(bool toConvert)
    {
        if (toConvert)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    #endregion
}
