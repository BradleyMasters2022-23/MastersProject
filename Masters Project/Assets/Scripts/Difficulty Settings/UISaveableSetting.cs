/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - Abstract func for any UI element managing player-pref save data
 * ================================================================================================
 */
using UnityEngine;

public abstract class UISaveableSetting : MonoBehaviour, ISavableSetting
{
    [Header("Save data")]

    [Tooltip("Name of the setting value")]
    [SerializeField] protected string settingKeyword;
    [Tooltip("Default value. If using bools, remember 0 = false and 1 = true.")]
    [SerializeField] protected float defaultValue;
    [Tooltip("Channel called when the setting is changed/loaded. Also used as setting name.")]
    [SerializeField] protected ChannelString onSettingChanged;

    /// <summary>
    /// current value of the setting
    /// </summary>
    protected float currentValue;

    /// <summary>
    /// Keyword for this setting
    /// </summary>
    public string Keyword
    {
        get { return settingKeyword; } 
    }

    /// <summary>
    /// Load in setting into player prefs
    /// </summary>
    public virtual void LoadSetting()
    {
        currentValue = PlayerPrefs.GetFloat(settingKeyword, defaultValue);
        UpdateUIElement();
    }

    /// <summary>
    /// Reset setting to default, save
    /// </summary>
    public virtual void ResetSetting()
    {
        currentValue = defaultValue;
        SaveSetting();
        UpdateUIElement();
    }

    /// <summary>
    /// Save the setting to player prefs
    /// </summary>
    public virtual void SaveSetting()
    {
        // only save if something changed
        if (PlayerPrefs.GetFloat(settingKeyword) != currentValue)
        {
            //Debug.Log("Saving setting " + settingKeyword);
            PlayerPrefs.SetFloat(settingKeyword, currentValue);
            onSettingChanged.RaiseEvent(settingKeyword);
        }
    }

    /// <summary>
    /// Update the UI element, whatever it may be
    /// </summary>
    protected abstract void UpdateUIElement();

    /// <summary>
    /// Function called when the UI element is called
    /// </summary>
    public abstract void OnUIInteract();
}
