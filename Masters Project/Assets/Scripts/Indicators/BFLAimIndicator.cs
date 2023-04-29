using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BFLAimIndicator : IIndicator
{
    [SerializeField] Transform[] objs;
    Vector3[] originalPositions;
    [SerializeField] float tightenTime;
    ScaledTimer t;

    [SerializeField] Vector3 targetLocation;

    private void Awake()
    {
        t = new ScaledTimer(tightenTime);

        // get original positions
        originalPositions = new Vector3[objs.Length];
        for (int i = 0; i < objs.Length; i++)
            originalPositions[i] = objs[i].localPosition;
    }

    public override void Activate()
    {
        // Reset to original positions
        for (int i = 0; i < objs.Length; i++)
            objs[i].localPosition = originalPositions[i];

        t.ResetTimer();
        StartCoroutine(Tighten());
    }

    public override void Deactivate()
    {
        StopAllCoroutines();
    }

    // Continuously update laser renderers
    protected IEnumerator Tighten()
    {
        while (true)
        {
            // determine lerp amount, slerp to target position
            float lerpAmt = t.TimerProgress();
            for(int i = 0; i < objs.Length; i++)
            {
                objs[i].transform.localPosition = Vector3.Slerp(originalPositions[i], targetLocation, lerpAmt);
            }

            yield return null;
        }
    }
}
