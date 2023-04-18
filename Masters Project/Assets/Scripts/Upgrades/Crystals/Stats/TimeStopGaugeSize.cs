using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    private TimeManager time;
    private TimeGagueVisual timeUI;
    [SerializeField] private float statBase;
    private float baseGauge;

    private void Start()
    {
        time = FindObjectOfType<TimeManager>();
        baseGauge = time.MaxGauge();
        
    }

    public override void LoadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGagueVisual>();
        
        float temp = baseGauge * ((float)mod / 10);
        time.UpgradeSetGaugeMax(time.MaxGauge() + temp);

        timeUI.ResetMaxValue();
        time.AddGauge(temp);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        timeUI = FindObjectOfType<TimeGagueVisual>();

        float temp = baseGauge * ((float)mod / 10);
        time.UpgradeSetGaugeMax(time.MaxGauge() - temp);

        timeUI.ResetMaxValue();
        time.AddGauge(-1 * temp);
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod*10);
    }
}
