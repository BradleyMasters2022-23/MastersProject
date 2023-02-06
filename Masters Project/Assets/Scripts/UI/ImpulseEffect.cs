/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - Februrary 6th, 2022
 * Last Edited - Februrary 6th, 2022 by Ben Schuster
 * Description - Controls the impulse effects playing on screen for simple things
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImpulseEffect : MonoBehaviour
{
    [SerializeField] private float impulseActivateTime;
    [SerializeField] private AnimationCurve impulseActivateCurve;
    [SerializeField] private float impulseDeactivateTime;
    [SerializeField] private AnimationCurve impuseDeactivateCurve;

    private ScaledTimer activateTracker;
    private ScaledTimer deactivateTracker;

    private Image imgRef;
    private Coroutine routine;

    private void Awake()
    {
        activateTracker = new ScaledTimer(impulseActivateTime, false);
        deactivateTracker = new ScaledTimer(impulseDeactivateTime, false);
        imgRef = GetComponent<Image>();
    }

    private IEnumerator Impulse(Color c)
    {
        Color temp = c;
        float alpha = 0;
        
        activateTracker.ResetTimer();
        while(!activateTracker.TimerDone())
        {
            alpha = (impulseActivateCurve.Evaluate(activateTracker.TimerProgress())) / temp.a;
            imgRef.color = new Color(temp.r, temp.g, temp.b, alpha);
            yield return null;
        }

        deactivateTracker.ResetTimer();
        while (!deactivateTracker.TimerDone())
        {
            alpha = (1 - impuseDeactivateCurve.Evaluate(deactivateTracker.TimerProgress())) / temp.a;
            imgRef.color = new Color(temp.r, temp.g, temp.b, alpha);
            yield return null;
        }
        imgRef.color = new Color(temp.r, temp.g, temp.b, 0);

        yield return null;
    }

    public void ActivateImpulse(Color targetColor)
    {
        if(routine!= null)
        {
            StopCoroutine(routine);
        }

        if (targetColor.a <= 0)
            return;

        routine = StartCoroutine(Impulse(targetColor));
    }
}
