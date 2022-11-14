using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Circuit Object Data")]
public class CircuitObject : ScriptableObject
{
    [Tooltip("Circuit display name.")]
    public string displayName;

    [Tooltip("Circuit display description.")]
    public string displayDesc;

    [Tooltip("Upgrade ID #.")]
    public int ID;

    [Tooltip("Circuits that must be unlocked first")]
    public CircuitObject[] dependencies;

    [Tooltip("Amount of voltage to unlock")]
    public int cost;

    [Tooltip("Holds the actual circuit info.")]
    public GameObject circuitPrefab;

    [Tooltip("Whether circuit has been unlocked")]
    public bool unlocked;

    /// <summary>
    /// node can only be unlocked if it is currently locked and all dependencies are unlocked
    /// </summary>
    public bool Unlockable()
    {
        if(unlocked) {
            Debug.Log("Circuit already unlocked!");
            return false;
        }

        foreach(CircuitObject circuit in dependencies)
        {
            if(!circuit.unlocked) {
                Debug.Log("At least one circuit dependency is locked.");
                return false;
            }
        }

        return true;
    }
}
