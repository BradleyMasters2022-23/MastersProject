using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDamageUp : IUpgrade {
    private PlayerGunController gun;
    private int baseDamage;
    [SerializeField] private float maxPercentIncrease;

    public override void LoadUpgrade(PlayerController player) {
        gun = FindObjectOfType<PlayerGunController>();
        baseDamage = gun.GetBaseDamage();
    }

    public void Update()
    {
        // this currently affects ALL projectiles in the scene, incl enemy ones
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            CalculateDamage(projectile);
        }
    }

    private void CalculateDamage(Projectile projectile)
    {
        if ((int)projectile.GetDistanceCovered() <= maxPercentIncrease && projectile.GetShotByPlayer())
        {

            Debug.Log("Max percent increase: " + maxPercentIncrease);
            float damageIncrease = maxPercentIncrease - (projectile.GetDistanceCovered());
            Debug.Log("Current percent increase: " + damageIncrease);
            damageIncrease /= 100;
            Debug.Log("Current increase in decimal: " + damageIncrease);
            damageIncrease *= (float)baseDamage;
            Debug.Log("Damage increase: " + damageIncrease);
            damageIncrease += (float)baseDamage;
            Debug.Log("Total damage: " + damageIncrease);
            projectile.ChangeDamageTo((int)damageIncrease);
            damageIncrease = 0;
        }
    }
}
