using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PPTimeScaler : TimeAffectedEntity
{
    [Tooltip("Target rendering volume")]
    [SerializeField] Volume renderingVolume;
    [Tooltip("Target weight in normal time")]
    [SerializeField] float normalTimeTarget;
    [Tooltip("Target weight in frozen time")]
    [SerializeField] float stoppedTimeTarget;

    // Start is called before the first frame update
    void Start()
    {
        if (renderingVolume == null)
        {
            Debug.LogError($"[PPTimeScaler] No rendering volume assigned!" +
                $" Double check the processor named {name}");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        renderingVolume.weight = Mathf.Lerp(stoppedTimeTarget, normalTimeTarget, Timescale);
    }
}
