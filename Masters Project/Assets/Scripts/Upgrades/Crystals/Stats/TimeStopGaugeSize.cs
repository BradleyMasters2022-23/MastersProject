using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    private TimeManager time;
    [SerializeField] private float statBase;
    public override void LoadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        time.SetGaugeMax(((float)mod/10) + statBase);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        time = FindObjectOfType<TimeManager>();
        time.SetGaugeMax(1 / (((float)mod / 10) + statBase));
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod*10);
    }
}
