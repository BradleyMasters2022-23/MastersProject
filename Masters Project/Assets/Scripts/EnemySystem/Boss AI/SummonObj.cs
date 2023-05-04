using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonObj : MonoBehaviour
{
    [SerializeField] float summonTime;
    [SerializeField] IIndicator[] summoningIndicators;
    [SerializeField] GameObject target;
    private ScaledTimer t;

    public void Play()
    {
        StartCoroutine(SummonRoutine());
    }

    private IEnumerator SummonRoutine()
    {
        Indicators.SetIndicators(summoningIndicators, true);
        t = new ScaledTimer(summonTime);

        yield return new WaitUntil(t.TimerDone);

        Indicators.SetIndicators(summoningIndicators, false);

        target.SetActive(true);
        yield return null;
    }

}
