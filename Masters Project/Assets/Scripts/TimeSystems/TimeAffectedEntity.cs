using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class TimeAffectedEntity : MonoBehaviour
{
    [Header("Timestop")]

    [Tooltip("Whether this entity is affected by timestop")]
    [SerializeField] private bool affectedByTimestop = true;

    [SerializeField] private float minimumTimescale = 0;

    [SerializeField] protected LayerMask slowFieldLayers;

    /// <summary>
    /// The secondary time scale that other abilities can influence
    /// </summary>
    [SerializeField, ReadOnly] private float secondaryTimescale = 1;

    /// <summary>
    /// Whether this entity scales with time
    /// </summary>
    protected bool Affected
    {
        get { return affectedByTimestop; }
    }
    /// <summary>
    /// Get this entity's timescale
    /// </summary>
    protected float Timescale
    {
        get
        {
            // If affected by timestop, return the slowest timescale
            if(affectedByTimestop)
            {
                float t = TimeManager.WorldTimeScale * Time.timeScale;

                // If secondary timescale is slower, use that
                if (t > secondaryTimescale)
                    t = secondaryTimescale;

                // clamp it based on minimum timescale value
                return Mathf.Clamp(t, minimumTimescale, 1);
            }
            // Otherwise, return normal
            else
                return Time.timeScale;
        }
    }
    /// <summary>
    /// Get this entity's delta time
    /// </summary>
    protected float DeltaTime
    {
        get
        {
            // If affected by timestop, return the slowest delta time
            if (affectedByTimestop)
            {
                // clamp it based on minimum timescale value
                return Timescale * Time.deltaTime;

            }
            // Otherwise, return normal
            else
                return Time.deltaTime;
        }
    }

    public void SetSeconaryTimescale(float amt)
    {
        secondaryTimescale = amt;
    }
}
