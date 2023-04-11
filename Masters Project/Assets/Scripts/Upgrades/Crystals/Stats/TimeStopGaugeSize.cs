using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopGaugeSize : IStat
{
    public override void LoadStat(PlayerController player, int mod)
    {

    }

    public override void UnloadStat(PlayerController player, int mod)
    {

    }

    public override float GetStatIncrease(int mod)
    {
        return 1;
    }
}
