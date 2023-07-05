using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHP : IStat
{
    private HealthManager hp;
    [Tooltip("Value stat mod multiplies by to get final HP increase")]
    [SerializeField] private float statBase;
    public int healthBarIndex = 0;
    private float baseValue;
    public override void LoadStat(PlayerController player, int mod)
    {
        

        hp = player.gameObject.GetComponent<HealthManager>();
        baseValue = hp.ResourceDataAtIndex(healthBarIndex)._maxValue;
        hp.IncreaseMaxHealth(baseValue * statBase * mod, healthBarIndex);
        // Debug.Log("HP up " + (statBase * mod).ToString());
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        hp = player.gameObject.GetComponent<HealthManager>();
        baseValue = hp.ResourceDataAtIndex(0)._maxValue;
        hp.DecreaseMaxHealth(baseValue * statBase * mod, healthBarIndex);
        
    }

    public override float GetStatIncrease(int mod)
    {
        float statIncrease = (float)statBase * (float)mod * 100;

        return statIncrease;
    }
}
