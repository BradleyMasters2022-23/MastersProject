/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 31th, 2022
 * Last Edited - October 31th, 2022 by Ben Schuster
 * Description - Control the settings for the settings menu
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Tooltip("Channel that sends out signal when the settings are changed")]
    [SerializeField] private ChannelVoid settingsChangedChannel;

    #region Mouse and Keyboard Variables

    [Header("===== Mouse & Keyboard =====")]

    [Header("--- Default Values ---")]
    [Tooltip("Default value for mouse sensitivity")]
    [SerializeField] private float defaultMouseSensitivity;
    [Tooltip("Default value for mouse x inversion")]
    [SerializeField] private bool defaultMouseInvertX;
    [Tooltip("Default value for mouse y inversion")]
    [SerializeField] private bool defaultMouseInvertY;

    /// <summary>
    /// sensitivity of the mouse
    /// </summary>
    public static float mouseSensitivity;
    /// <summary>
    /// Axis inversion for mouse
    /// </summary>
    public static bool mouseInvertX;
    public static bool mouseInvertY;

    [Header("--- Setup ---")]
    [Tooltip("Slider element for mouse sensitivity")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [Tooltip("Toggle element for mouse Invert X")]
    [SerializeField] private Toggle mouseInvertXToggle;
    [Tooltip("Toggle element for mouse Invert Y")]
    [SerializeField] private Toggle mouseInvertYToggle;

    #endregion

    #region Controller Variables

    [Header("===== Controller  =====")]

    [Header("--- Default Values ---")]
    [Tooltip("Default value for controller sensitivity")]
    [SerializeField] private float defaultControllerSensitivity;
    [Tooltip("Default value for controller x inversion")]
    [SerializeField] private bool defaultControllerInvertX;
    [Tooltip("Default value for controller y inversion")]
    [SerializeField] private bool defaultControllerInvertY;

    /// <summary>
    /// sensitivity of the controller
    /// </summary>
    public static float controllerSensitivity;
    /// <summary>
    /// multiplier to controller sensitivity [otherwise is too slow]
    /// </summary>
    public const float CONTROLLERMULTIPLIER = 50;
    /// <summary>
    /// Axis inversion for controller
    /// </summary>
    public static bool controllerInvertX;
    public static bool controllerInvertY;

    [Header("--- Setup ---")]
    [Tooltip("Slider element for controller sensitivity")]
    [SerializeField] private Slider controllerSensitivitySlider;
    [Tooltip("Toggle element for controller Invert X")]
    [SerializeField] private Toggle controllerInvertXToggle;
    [Tooltip("Toggle element for controller Invert Y")]
    [SerializeField] private Toggle controllerInvertYToggle;

    #endregion

    #region Initialization

    /// <summary>
    /// Load in player refs
    /// </summary>
    private void Awake()
    {
        // Get player prefs data
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
        mouseInvertX = IntToBool(PlayerPrefs.GetInt("MouseInvertX", BoolToInt(defaultMouseInvertX)));
        mouseInvertY = IntToBool(PlayerPrefs.GetInt("MouseInvertY", BoolToInt(defaultMouseInvertY)));

        controllerSensitivity = PlayerPrefs.GetFloat("ControllerSensitivity", defaultControllerSensitivity * CONTROLLERMULTIPLIER);
        controllerInvertX = IntToBool(PlayerPrefs.GetInt("ControllerInvertX", BoolToInt(defaultControllerInvertX)));
        controllerInvertY = IntToBool(PlayerPrefs.GetInt("ControllerInvertY", BoolToInt(defaultControllerInvertY)));

        UpdateUI();
    }

    /// <summary>
    /// Updates UI elements to match what they should be set to
    /// </summary>
    private void UpdateUI()
    {
        mouseSensitivitySlider.value = mouseSensitivity;
        mouseInvertXToggle.isOn = mouseInvertX;
        mouseInvertYToggle.isOn = mouseInvertY;

        controllerSensitivitySlider.value = controllerSensitivity / CONTROLLERMULTIPLIER;
        controllerInvertXToggle.isOn = controllerInvertX;
        controllerInvertYToggle.isOn = controllerInvertY;
    }

    #endregion

    #region ChangeFunctions

    /// <summary>
    /// changing mouse sensitivity
    /// </summary>
    public void ChangeMouseSensitivity()
    {
        mouseSensitivity = mouseSensitivitySlider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
    }
    /// <summary>
    /// Inverts the X axis movement controls on mouse
    /// </summary>
    public void MouseInvertX()
    {
        mouseInvertX = mouseInvertXToggle.isOn;
        PlayerPrefs.SetInt("MouseInvertX", BoolToInt(mouseInvertX));
    }
    /// <summary>
    /// Inverts the Y axis movement controls on mouse
    /// </summary>
    public void MouseInvertY()
    {
        mouseInvertY = mouseInvertYToggle.isOn;
        PlayerPrefs.SetInt("MouseInvertY", BoolToInt(mouseInvertY));
    }


    /// <summary>
    /// changing controller sensitivity
    /// </summary>
    public void ChangeControllerSensitivity()
    {
        controllerSensitivity = controllerSensitivitySlider.value * CONTROLLERMULTIPLIER;
        PlayerPrefs.SetFloat("ControllerSensitivity", controllerSensitivity);
    }

    /// <summary>
    /// Inverts the X axis movement controls on controller
    /// </summary>
    public void ControllerInvertX()
    {
        controllerInvertX = controllerInvertXToggle.isOn;
        PlayerPrefs.SetInt("controllerInvertX", BoolToInt(controllerInvertX));
    }

    /// <summary>
    /// Inverts the Y axis movement controls on controller
    /// </summary>
    public void ControllerInvertY()
    {
        controllerInvertY = controllerInvertYToggle.isOn;
        PlayerPrefs.SetInt("ControllerInvertY", BoolToInt(controllerInvertY));
    }

    #endregion

    #region Utility

    /// <summary>
    /// Revert the mouse sensitivity values to default values
    /// </summary>
    public void RevertToDefault()
    {
        // Reset to default values
        mouseSensitivity = defaultMouseSensitivity;
        mouseInvertY = defaultMouseInvertY;
        mouseInvertX = defaultMouseInvertX;
        controllerSensitivity = defaultControllerSensitivity * CONTROLLERMULTIPLIER;
        controllerInvertX = defaultControllerInvertX;
        controllerInvertY = defaultControllerInvertY;

        // Save playerprefs
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("MouseInvertX", BoolToInt(mouseInvertX));
        PlayerPrefs.SetInt("MouseInvertY", BoolToInt(mouseInvertY));

        PlayerPrefs.SetFloat("ControllerSensitivity", controllerSensitivity);
        PlayerPrefs.SetInt("ControllerInvertX", BoolToInt(controllerInvertX));
        PlayerPrefs.SetInt("ControllerInvertY", BoolToInt(controllerInvertY));

        // Update UI elements
        UpdateUI();
    }

    /// <summary>
    /// Convert an bool value to an int value. C# doesn't do this automatically...
    /// </summary>
    /// <param name="val">value to convert</param>
    /// <returns>converted value</returns>
    private int BoolToInt(bool val)
    {
        if (val)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// Convert an int value to an bool value. C# doesn't do this automatically...
    /// </summary>
    /// <param name="val">value to convert</param>
    /// <returns>converted value</returns>
    private bool IntToBool(int val)
    {
        if (val == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check to make sure this screen is closed before pausing. Bandaid fix
    /// </summary>
    /// <returns>whether or not it can pause</returns>
    public bool SettingsOpen()
    {
        return this.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Perform any on-settings-finished actions
    /// </summary>
    public void SettingsConfirmed()
    {
        settingsChangedChannel.RaiseEvent();
    }


    #endregion

}
