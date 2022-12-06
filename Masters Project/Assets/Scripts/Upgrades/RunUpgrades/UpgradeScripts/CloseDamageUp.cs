using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDamageUp : IUpgrade {
    private PlayerGunController gun;
    [SerializeField] private int maxPercentIncrease;

    public override void LoadUpgrade(PlayerController player) {
        gun = FindObjectOfType<PlayerGunController>();
    }

    public void Update()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            if((int)projectile.GetDistanceCovered() <= maxPercentIncrease)
            {
                int damageIncrease = maxPercentIncrease - (int)(projectile.GetDistanceCovered());
                damageIncrease /= 100;
                damageIncrease *= projectile.GetDamage();
                damageIncrease += projectile.GetDamage();
                projectile.ChangeDamageTo(damageIncrease);
            }
        }
    }
}
