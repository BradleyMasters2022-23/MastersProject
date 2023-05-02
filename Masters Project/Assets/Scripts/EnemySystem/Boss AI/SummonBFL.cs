using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBFL : MonoBehaviour
{
    [SerializeField] float summonTime;
    [SerializeField] IIndicator[] summoningIndicators;
    [SerializeField] GameObject BFL;
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

        BFL.SetActive(true);
        BFL.GetComponent<BFLController>().enabled = true;
        yield return null;
    }

}
