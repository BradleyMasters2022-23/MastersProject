using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    private TimeManager time;
    private TimeGaugeUI timeUI;
    [SerializeField] private float statBase;
    private const int FixedUpdateCalls = 50;

    public override void LoadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGaugeUI>();
        
        float temp = time.GetBaseMax() * ((float)mod / 10f);
        time.UpgradeSetGaugeMax(time.UpgradeMaxGauge() + temp);
        time.AddGauge(temp * FixedUpdateCalls);
        timeUI.ResetMaxValue();
        
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGaugeUI>();

        float temp = time.GetBaseMax() * ((float)mod / 10f);
        time.UpgradeSetGaugeMax(time.UpgradeMaxGauge() - temp);
        time.AddGauge(-1 * temp * FixedUpdateCalls);
        timeUI.ResetMaxValue();
        
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod*10);
    }
}
