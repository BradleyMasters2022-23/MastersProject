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

    [SerializeField, ReadOnly] private List<TimeInfluencer> timeInfluencers = new List<TimeInfluencer>();

    /// <summary>
    /// Array of all time observed objects in this entity
    /// </summary>
    private TimeObserver[] observers;
    /// <summary>
    /// Whether or not this entity is already slowed
    /// </summary>
    protected bool slowed = false;

    /// <summary>
    /// Whether this entity scales with time
    /// </summary>
    protected bool Affected
    {
        get { return affectedByTimestop; }
    }
    /// <summary>
    /// Whether this entity is currently being slowed already
    /// </summary>
    public bool Slowed
    {
        get { return Timescale <= TimeManager.TimeStopThreshold; }
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
                // get world timescale
                float t = TimeManager.WorldTimeScale * Time.timeScale;

                // If secondary timescale is slower, use that
                if (t > secondaryTimescale)
                {
                    t = secondaryTimescale;
                }
                    

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

    #region Time Influencers

    /// <summary>
    /// Get the current secondary timescale
    /// </summary>
    /// <returns>Current value of the secondary time scale</returns>
    private float SecondaryTimescale()
    {
        float min = 1;

        // Go through each influencer to get the lowest value
        int i = 0;
        foreach(var entity in timeInfluencers.ToArray())
        {
            // Null check. Remove if null
            if (entity == null)
            {
                timeInfluencers.RemoveAt(i);
                i++;
                continue;
            }

            // Get the current value
            float v = entity.GetScale();

            // if the lowest value, then just return it outright
            if (v == 0)
            {
                return 0;
            }
            // Otherwise, check if its the new min
            else if(v < min)
            {
                min = v;
            }
                
            // iterate index. Used for null checking
            i++;
        }

        // return min
        return min;
    }

    /// <summary>
    /// Subscribe a secondary timescale to this entity
    /// </summary>
    /// <param name="i">Influencer to add</param>
    public void SecondarySubscribe(TimeInfluencer i)
    {
        if (timeInfluencers == null)
            timeInfluencers = new List<TimeInfluencer>();

        if (!timeInfluencers.Contains(i))
        {
            timeInfluencers.Add(i);
            secondaryTimescale = SecondaryTimescale();
        }
    }
    /// <summary>
    /// Unsubscribe a secondary timescale to this entity
    /// </summary>
    /// <param name="i">Influencer to remove</param>
    public void SecondaryUnsubscribe(TimeInfluencer i)
    {
        if (timeInfluencers != null && timeInfluencers.Contains(i))
        {
            timeInfluencers.Remove(i);
            secondaryTimescale = SecondaryTimescale();
        }
    }

    #endregion

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
        
        // If observers not yet acquired, do so now
        if(observers == null)
        {
            observers = GetComponentsInChildren<TimeObserver>();
        }

        // if there are observers...
        if(observers.Length> 0)
        {
            // If this is the first frame of being slowed, perform relevant actions
            if (!slowed && Slowed)
            {
                slowed = true;

                foreach (var o in observers)
                    o.OnStop();
            }
            // If this is the first frame of not being slowed, perform relevant actions
            else if (slowed && !Slowed)
            {
                slowed = false;

                foreach(var o in observers)
                    o.OnResume();
            }
        }
    }

    #endregion
}
