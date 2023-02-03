using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPSectionUp : IUpgrade
{
    private HealthManager hp;
    [Tooltip("Amount of health to increase by")]
    public float healthIncrease;
    [Tooltip("Healthbar index to increase by. 0 is the primary health bar while 1 is the shield")]
    public int healthbarIndex = 0;

    public override void LoadUpgrade(PlayerController player)
    {
        hp = player.gameObject.GetComponent<HealthManager>();

        // Use index of 0, 
        hp.IncreaseMaxHealth(healthIncrease, healthbarIndex);

    }
}