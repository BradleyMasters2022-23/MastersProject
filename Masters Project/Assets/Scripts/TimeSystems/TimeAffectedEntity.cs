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

    private float personalTime;
    private float personalTimerModifier;

    /// <summary>
    /// Whether this entity scales with time
    /// </summary>
    protected bool Affected
    {
        get { return affectedByTimestop; }
    }

    private List<LocalTimer> timerList;

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
                // Get time based on current timescale
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

    #region Local Timer Utility

    // These are various shortcut functions to make working with local timers easier
    // and allows for personal time to remain private.

    // This update to the scaled timer system is necessary for making localized timestop work
    // as intended. 

    /// <summary>
    /// Create a new timer, add it to list automatically.
    /// Helps streamline the process 
    /// </summary>
    /// <param name="target">Target time to aim for</param>
    /// <returns>Newly requested timer</returns>
    protected LocalTimer GetTimer(float target)
    {
        // create the new timer
        LocalTimer t = new LocalTimer(target);

        // if the list is not created, make one, add it back
        if(timerList == null)
            timerList = new List<LocalTimer>();

        timerList.Add(t);

        // return it to caller 
        return t;
    }

    protected virtual void FixedUpdate()
    {
        if(timerList != null)
        {
            foreach (LocalTimer t in timerList)
            {
                if (t != null)
                    t.UpdateTime(DeltaTime);
            }
        }
    }

    #endregion
}
