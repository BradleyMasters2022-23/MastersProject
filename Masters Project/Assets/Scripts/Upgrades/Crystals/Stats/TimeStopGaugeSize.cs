using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    private TimeManager time;
    private TimeGagueVisual timeUI;
    [SerializeField] private float statBase;

    public override void LoadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGagueVisual>();

        float temp = time.MaxGauge() * ((float)mod / 10);
        time.SetGaugeMax(((float)mod/10) + statBase);
        timeUI.ResetMaxValue();
        time.AddGauge(temp);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGagueVisual>();
        time.SetGaugeMax(1 / (((float)mod / 10) + statBase));
        float temp = time.MaxGauge() * ((float)mod / 10);
        timeUI.ResetMaxValue();
        time.AddGauge(-1 * temp);
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod*10);
    }
}
