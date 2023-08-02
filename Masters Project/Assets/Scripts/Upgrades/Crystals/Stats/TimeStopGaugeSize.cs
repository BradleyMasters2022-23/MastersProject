using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    private TimeManager time;
    private TimeGaugeUI timeUI;
    [SerializeField] private float statBase;

    public override void LoadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGaugeUI>();
        
        float temp = time.GetBaseMax() * ((float)mod) * statBase;
        //time.UpgradeSetGaugeMax(time.UpgradeMaxGauge() + temp);
        time.UpgradeIncrementMaxTimeGauge(temp);
        time.AddGauge(temp);
        //timeUI.ResetMaxValue();
        
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGaugeUI>();

        float temp = time.GetBaseMax() * ((float)mod) * statBase;
        //time.UpgradeSetGaugeMax(time.UpgradeMaxGauge() - temp);
        time.UpgradeIncrementMaxTimeGauge(-temp);
        time.AddGauge(-1 * temp);
        //timeUI.ResetMaxValue();
        
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod) * statBase * 100;
    }
}
