using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAmount : IStat
{
    private HealthManager hp;
    [Tooltip("Value the stat mod multiplies by to get final shield increase")]
    [SerializeField] private float statBase;
    private int healthBarIndex = 1;

    public override void LoadStat(PlayerController player, int mod)
    {
        hp = player.gameObject.GetComponent<HealthManager>();
        float originalMax = hp.ResourceDataAtIndex(healthBarIndex)._maxValue;
        hp.IncreaseMaxHealth(originalMax * statBase * (float)mod, healthBarIndex);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        hp = player.gameObject.GetComponent<HealthManager>();
        float originalMax = hp.ResourceDataAtIndex(healthBarIndex)._maxValue;

        hp.DecreaseMaxHealth(originalMax * statBase * mod, healthBarIndex);
    }

    public override float GetStatIncrease(int mod)
    {
        float statIncrease = statBase * (float)mod;

        return statIncrease * 100;
    }
}
