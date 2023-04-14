using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHP : IStat
{
    private HealthManager hp;
    [Tooltip("Value stat mod multiplies by to get final HP increase")]
    [SerializeField] private int statBase;
    public int healthBarIndex = 0;

    public override void LoadStat(PlayerController player, int mod)
    {
        hp = player.gameObject.GetComponent<HealthManager>();
        hp.IncreaseMaxHealth(statBase * mod, healthBarIndex);
        Debug.Log("HP up " + (statBase * mod).ToString());
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        hp = player.gameObject.GetComponent<HealthManager>();

        hp.DecreaseMaxHealth(statBase*mod, healthBarIndex);
    }

    public override float GetStatIncrease(int mod)
    {
        float statIncrease = (float)statBase * (float)mod;

        return statIncrease;
    }
}
