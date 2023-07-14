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

    #region SoundSettings

    //[Header("===== Audio =====")]

    //[Header("--- Default Values ---")]
    //[Tooltip("Default value for master volume"), Range(0, 1)]
    //[SerializeField] private float defaultMasterVolume;

    ///// <summary>
    ///// sensitivity of the mouse
    ///// </summary>
    //public static float masterVolume;

    //[Header("--- Setup ---")]
    //[Tooltip("Slider element for master volume")]
    //[SerializeField] private Slider masterVolumeSlider;

    #endregion

    #region Mouse and Keyboard Variables

    [Header("===== Mouse & Keyboard =====")]

    [Header("--- Default Values ---")]
    [Tooltip("Default value for mouse sensitivity")]
    [SerializeField] private float defaultMouseSensitivity;
    [Tooltip("Default value for mouse x inversion")]
    [SerializeField] private bool defaultMouseInvertX;
    [Tooltip("Default value for mouse y inversion")]
    [SerializeField] private bool defaultMouseInvertY;
    [Tooltip("Default value for mouse having aimassist")]
    [SerializeField] private bool defaultMouseAimAssist;

    /// <summary>
    /// sensitivity of the mouse
    /// </summary>
    public static float mouseSensitivity;
    /// <summary>
    /// Axis inversion for mouse
    /// </summary>
    public static bool mouseInvertX;
    public static bool mouseInvertY;
    public static bool mouseAimAssist;

    [Header("--- Setup ---")]
    [Tooltip("Slider element for mouse sensitivity")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [Tooltip("Toggle element for mouse Invert X")]
    [SerializeField] private Toggle mouseInvertXToggle;
    [Tooltip("Toggle element for mouse Invert Y")]
    [SerializeField] private Toggle mouseInvertYToggle;
    [Tooltip("Toggle element for mouse aim assist")]
    [SerializeField] private Toggle mouseAimAssistToggle;

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
    [Tooltip("Default value for controller aim assist")]
    [SerializeField] private bool defaultControllerAimAssist;

    /// <summary>
    /// sensitivity of the controller
    /// </summary>
    public static float controllerSensitivity;
    /// <summary>
    /// Axis inversion for controller
    /// </summary>
    public static bool controllerInvertX;
    public static bool controllerInvertY;
    public static bool controllerAimAssist;

    [Header("--- Setup ---")]
    [Tooltip("Slider element for controller sensitivity")]
    [SerializeField] private Slider controllerSensitivitySlider;
    [Tooltip("Toggle element for controller Invert X")]
    [SerializeField] private Toggle controllerInvertXToggle;
    [Tooltip("Toggle element for controller Invert Y")]
    [SerializeField] private Toggle controllerInvertYToggle;
    [Tooltip("Toggle element for controller aim assist")]
    [SerializeField] private Toggle controllerAimAssistToggle;
    #endregion

    #region Initialization

    /// <summary>
    /// Load in player refs
    /// </summary>
    private void Awake()
    {
        // Get player prefs data
        //masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);

        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultMouseSensitivity);
        mouseInvertX = IntToBool(PlayerPrefs.GetInt("MouseInvertX", BoolToInt(defaultMouseInvertX)));
        mouseInvertY = IntToBool(PlayerPrefs.GetInt("MouseInvertY", BoolToInt(defaultMouseInvertY)));
        mouseAimAssist = IntToBool(PlayerPrefs.GetInt("MouseAimAssist", BoolToInt(defaultMouseAimAssist)));

        controllerSensitivity = PlayerPrefs.GetFloat("ControllerSensitivity", defaultControllerSensitivity);
        controllerInvertX = IntToBool(PlayerPrefs.GetInt("ControllerInvertX", BoolToInt(defaultControllerInvertX)));
        controllerInvertY = IntToBool(PlayerPrefs.GetInt("ControllerInvertY", BoolToInt(defaultControllerInvertY)));
        controllerAimAssist = IntToBool(PlayerPrefs.GetInt("ControllerAimAssist", BoolToInt(defaultControllerAimAssist)));

        UpdateUI();
    }

    /// <summary>
    /// Updates UI elements to match what they should be set to
    /// </summary>
    private void UpdateUI()
    {
        //masterVolumeSlider.value = masterVolume;

        mouseSensitivitySlider.value = mouseSensitivity;
        mouseInvertXToggle.isOn = mouseInvertX;
        mouseInvertYToggle.isOn = mouseInvertY;
        mouseAimAssistToggle.isOn = mouseAimAssist;

        controllerSensitivitySlider.value = controllerSensitivity;
        controllerInvertXToggle.isOn = controllerInvertX;
        controllerInvertYToggle.isOn = controllerInvertY;
        controllerAimAssistToggle.isOn = controllerAimAssist;

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
    /// Inverts the aim assist for mouse
    /// </summary>
    public void ToggleMouseAimAssist()
    {
        mouseAimAssist = mouseAimAssistToggle.isOn;
        PlayerPrefs.SetInt("MouseAimAssist", BoolToInt(mouseAimAssist));
    }


    /// <summary>
    /// changing controller sensitivity
    /// </summary>
    public void ChangeControllerSensitivity()
    {
        controllerSensitivity = controllerSensitivitySlider.value;
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
    /// <summary>
    /// Inverts the aim assist for controller
    /// </summary>
    public void ToggleControllerAimAssist()
    {
        controllerAimAssist = controllerAimAssistToggle.isOn;
        PlayerPrefs.SetInt("ControllerAimAssist", BoolToInt(controllerAimAssist));
    }

    #endregion

    #region Utility

    /// <summary>
    /// Revert the mouse sensitivity values to default values
    /// </summary>
    public void RevertToDefault()
    {
        // Reset to default values
        //masterVolume = defaultMasterVolume;
        mouseSensitivity = defaultMouseSensitivity;
        mouseInvertY = defaultMouseInvertY;
        mouseInvertX = defaultMouseInvertX;
        mouseAimAssist = defaultMouseAimAssist;
        controllerSensitivity = defaultControllerSensitivity;
        controllerInvertX = defaultControllerInvertX;
        controllerInvertY = defaultControllerInvertY;
        controllerAimAssist = defaultControllerAimAssist;

        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("MouseInvertX", BoolToInt(mouseInvertX));
        PlayerPrefs.SetInt("MouseInvertY", BoolToInt(mouseInvertY));
        PlayerPrefs.SetInt("MouseAimAssist", BoolToInt(mouseAimAssist));

        PlayerPrefs.SetFloat("ControllerSensitivity", controllerSensitivity);
        PlayerPrefs.SetInt("ControllerInvertX", BoolToInt(controllerInvertX));
        PlayerPrefs.SetInt("ControllerInvertY", BoolToInt(controllerInvertY));
        PlayerPrefs.SetInt("ControllerAimAssist", BoolToInt(controllerAimAssist));


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

    private void OnDisable()
    {
        settingsChangedChannel.RaiseEvent();
    }

    #endregion
}
