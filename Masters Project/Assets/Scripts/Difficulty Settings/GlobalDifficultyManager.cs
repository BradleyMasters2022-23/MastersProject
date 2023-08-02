/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 28th, 2023
 * Last Edited - July 28th, 2023 by Ben Schuster
 * Description - Global difficulty manager that observes difficulty and sends update events
 * to those that request it
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for observing difficulty
/// </summary>
public interface IDifficultyObserver
{
    /// <summary>
    /// Get the new difficulty modifier and apply it
    /// </summary>
    /// <param name="newModifier">new modifier to apply</param>
    void UpdateDifficulty(float newModifier);
}

public class GlobalDifficultyManager : MonoBehaviour
{
    /// <summary>
    /// Global difficuklty manager instance. Handles distributing changes to difficulty
    /// </summary>
    public static GlobalDifficultyManager instance;

    /// <summary>
    /// Dictionary used to track settings and observers.
    /// KEY - string of the setting key
    /// VALUE - list of all current subscribers
    /// </summary>
    private Dictionary<string, List<IDifficultyObserver>> difficulties;

    /// <summary>
    /// Channel called when any setting is changed. String passed through is its key.
    /// </summary>
    public ChannelString onDifficultySettingChange;

    /// <summary>
    /// List of settings available for use
    /// note: Changing these involves changing the UI setting ref AND ref used by 
    /// concrete implementation of these settings
    /// </summary>
    private readonly string[] settingKeys = 
    {
        "PlayerDamage",
        "PlayerDamageVulnerability",
        "EnemyProjectileSpeed",
        "EnemyBudget",
        "HealthReplenishRate",
        "TimeReplenishRate",
        "AimAssistStrength"
    };

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PrepareSettingLookup();
    }


    private void OnEnable()
    {
        if (this != instance) return;

        onDifficultySettingChange.OnEventRaised += UpdateSettings;
    }

    private void OnDisable()
    {
        if (this != instance) return;

        onDifficultySettingChange.OnEventRaised -= UpdateSettings;
    }

    /// <summary>
    /// Prepare the dictionary lookup
    /// </summary>
    private void PrepareSettingLookup()
    {
        difficulties = new Dictionary<string, List<IDifficultyObserver>>();
        for (int i = 0; i < settingKeys.Length; i++)
        {
            difficulties.Add(settingKeys[i], new List<IDifficultyObserver>());
        }
    }

    /// <summary>
    /// Get the new value, tell each subscriber to update as well
    /// </summary>
    /// <param name="modifiedSetting">Key of setting to update</param>
    private void UpdateSettings(string modifiedSetting)
    {
        // if not a valid setting, return
        if (!difficulties.ContainsKey(modifiedSetting))
        {
            Debug.Log($"[DifficultyManager] Tried to update setting {modifiedSetting}, but not results." +
                $" Double check it spelling, and make sure its added to this manager's options.");
            return;
        }
        
        // get ref to subscribers
        float newModifier = (PlayerPrefs.GetFloat(modifiedSetting, 0)/100);
        List<IDifficultyObserver> group = difficulties[modifiedSetting];
        Debug.Log($"Updating Setting {modifiedSetting} to {newModifier}");

        // tell each subscriber to update its settings
        foreach (IDifficultyObserver observer in group)
        {
            observer.UpdateDifficulty(newModifier);
        }
    }

    /// <summary>
    /// Subscribe to difficulty manager to recieve updates from difficulty manager.
    /// Will also update its current modifer
    /// </summary>
    /// <param name="o">Observer to subscribe</param>
    /// <param name="settingKey">Difficulty setting to use</param>
    public void Subscribe(IDifficultyObserver o, string settingKey)
    {
        if(difficulties.ContainsKey(settingKey) && !difficulties[settingKey].Contains(o))
        {
            difficulties[settingKey].Add(o);

            // on subscribe, give it the current difficulty setting so its not out of date
            float modifier = (PlayerPrefs.GetFloat(settingKey, 0) / 100);
            o.UpdateDifficulty(modifier);
        }
    }

    /// <summary>
    /// Unsubscribe fromm the manager to be unaffected by difficulty. WIll reset its modifier to 1
    /// </summary>
    /// <param name="o">Object unsubscribing</param>
    /// <param name="settingKey">Difficulty key to check</param>
    public void Unsubscribe(IDifficultyObserver o, string settingKey)
    {
        if (difficulties.ContainsKey(settingKey) && difficulties[settingKey].Contains(o))
        {
            difficulties[settingKey].Remove(o);

            // on unsubscribe, reset its difficulty modifier to flat 1
            o.UpdateDifficulty(1);
        }
    }
}
