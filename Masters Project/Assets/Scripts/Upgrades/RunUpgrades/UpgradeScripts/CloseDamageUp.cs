using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDamageUp : IUpgrade {
    private PlayerGunController gun;
    private float baseDamage;
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
        // Commented out due to obselete code having issues with new structure

        //if ((int)projectile.GetDistanceCovered() <= maxPercentIncrease && projectile.GetShotByPlayer())
        //{

        //    float damageIncrease = maxPercentIncrease - (projectile.GetDistanceCovered());
        //    damageIncrease /= 100;
        //    damageIncrease *= (float)baseDamage;
        //    damageIncrease += (float)baseDamage;
        //    projectile.ChangeDamageTo((int)damageIncrease);
        //    damageIncrease = 0;
        //}
    }
}
