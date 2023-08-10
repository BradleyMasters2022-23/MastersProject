/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - May 25th, 2022
 * Last Edited - May 25, 2022 by Ben Schuster
 * Description - A field that makes every entity within its field slow or stop
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TimeslowField : MonoBehaviour, TimeInfluencer
{
    [SerializeField] private float duration;
    [SerializeField] private float slowAmount;
    [SerializeField] private float applyTime;
    [SerializeField] private LayerMask layerMask;
    private ScaledTimer t;

    [SerializeField] private List<TimeAffectedEntity> detectedTargets;

    [Header("Comunication")]
    [SerializeField] Slider durationDisplay;
    [SerializeField] IIndicator[] activationIndicators;

    private void Start()
    {
        t = new ScaledTimer(duration, true);
        detectedTargets = new List<TimeAffectedEntity>();
        
        Indicators.SetIndicators(activationIndicators, true);
    }

    /// <summary>
    /// Check if timer is done 
    /// </summary>
    private void Update()
    {
        // Update the timer
        if(durationDisplay != null)
        {
            durationDisplay.value = 1 - t.TimerProgress();
        }

        if (t != null &&  t.TimerDone())
        {
            foreach(var t in detectedTargets.ToArray())
            {
                UnregisterTgt(t);
            }
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// When anything enters trigger, slow it
    /// </summary>
    /// <param name="other">Collider to register</param>
    private void OnTriggerEnter(Collider other)
    {
         RegisterTgt(other);
    }
    /// <summary>
    /// When anything exits trigger, unslow it
    /// </summary>
    /// <param name="other">collider to unregister </param>
    private void OnTriggerExit(Collider other)
    {
         UnregisterTgt(other);
    }

    /// <summary>
    /// Register the target, if possible
    /// </summary>
    /// <param name="tgt">Target collider to register</param>
    private void RegisterTgt(Collider tgt)
    {
        // Get the main target hitbox, get all time effected objects there
        TargetHitbox indirectTarget = tgt.GetComponent<TargetHitbox>();
        if (indirectTarget != null)
        {
            //Debug.Log($"Checking objects in root of {indirectTarget.Target().name}");
            TimeAffectedEntity[] targets = indirectTarget.Target().GetComponentsInChildren<TimeAffectedEntity>();

            foreach (var t in targets)
            {
                if (t != null && !detectedTargets.Contains(t))
                {
                    //Debug.Log($"registering {t.GetType()}");
                    t.SecondarySubscribe(this);
                    detectedTargets.Add(t);
                }
            }
        }
        
        // Also try to get any direct time affected entities
        TimeAffectedEntity directTarget = tgt.GetComponent<TimeAffectedEntity>();
        if (directTarget != null)
        {
            directTarget.SecondarySubscribe(this);
            detectedTargets.Add(directTarget);

            
            TimeAffectedEntity[] targets = directTarget.GetComponentsInChildren<TimeAffectedEntity>();

            foreach (var t in targets)
            {
                if (t != null && !detectedTargets.Contains(t))
                {
                    //Debug.Log($"registering {t.GetType()}");
                    t.SecondarySubscribe(this);
                    detectedTargets.Add(t);
                }
            }
        }
        
    }

    /// <summary>
    /// Unregister the target if possible. Uses direct references
    /// </summary>
    /// <param name="tgt">direct reference to unregister</param>
    private void UnregisterTgt(TimeAffectedEntity tgt)
    {
        if (tgt != null && detectedTargets.Contains(tgt))
        {
            tgt.SecondaryUnsubscribe(this);
            detectedTargets.Remove(tgt);
        }
    }
    /// <summary>
    /// Unregister the target if possible. Uses indirect references
    /// </summary>
    /// <param name="tgt">indirect references to unregister</param>
    private void UnregisterTgt(Collider tgt)
    {
        TimeAffectedEntity[] targets = tgt.transform.GetComponentsInChildren<TimeAffectedEntity>();

        foreach (var t in targets)
        {
            if (t != null && detectedTargets.Contains(t))
            {
                //Debug.Log($"Unregistering {t.name} bc it was child of {tgt.name}");
                t.SecondaryUnsubscribe(this);
                detectedTargets.Add(t);
            }
        }
    }

    public void SubToField(Collider tgt)
    {
        RegisterTgt(tgt);
    }

    public float GetScale()
    {
        return slowAmount;
    }

    private void OnDestroy()
    {
        if (detectedTargets.Count > 0)
        {
            foreach (var t in detectedTargets.ToArray())
            {
                UnregisterTgt(t);
            }
        }
    }
}
