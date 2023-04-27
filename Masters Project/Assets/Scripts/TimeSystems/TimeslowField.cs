using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TimeslowField : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float slowAmount;
    [SerializeField] private float applyTime;
    [SerializeField] private LayerMask layerMask;
    private ScaledTimer t;

    private List<TimeAffectedEntity> detectedTargets;

    private void Start()
    {
        t = new ScaledTimer(duration, true);
        detectedTargets = new List<TimeAffectedEntity>();
    }

    private void Update()
    {
        if (t != null &&  t.TimerDone())
        {
            foreach(var t in detectedTargets.ToArray())
            {
                UnregisterTgt(t);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RegisterTgt(other);
    }

    private void OnTriggerExit(Collider other)
    {
        UnregisterTgt(other);
    }

    private void RegisterTgt(Collider tgt)
    {
        TimeAffectedEntity[] targets = tgt.transform.root.GetComponentsInChildren<TimeAffectedEntity>();

        foreach(var t in targets)
        {
            if (t != null && !detectedTargets.Contains(t))
            {
                Debug.Log($"registering {t.GetType()}");

                t.SetSeconaryTimescale(slowAmount);
                detectedTargets.Add(t);
            }
        }
    }
    private void UnregisterTgt(TimeAffectedEntity tgt)
    {
        if (tgt != null && detectedTargets.Contains(tgt))
        {
            tgt.SetSeconaryTimescale(1);
            detectedTargets.Remove(tgt);
        }
    }
    private void UnregisterTgt(Collider tgt)
    {
        TimeAffectedEntity[] targets = tgt.transform.root.GetComponentsInChildren<TimeAffectedEntity>();

        foreach (var t in targets)
        {
            if (t != null && detectedTargets.Contains(t))
            {
                t.SetSeconaryTimescale(slowAmount);
                detectedTargets.Add(t);
            }
        }
    }
}
