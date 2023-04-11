using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : IStat
{
    private PlayerGunController gun;

    /// <summary>
    /// Multiplies with the mod to get the amount to increase the total damage modifier by.
    /// </summary>
    [SerializeField] private float statBase;

    public override void LoadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetDamageMultiplier(((float)mod * statBase) + gun.GetDamageMultiplier());
        Debug.Log("Bullet damage up");
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetDamageMultiplier(gun.GetDamageMultiplier() - ((float)mod * statBase));
    }

    public override float GetStatIncrease(int mod)
    {
        return 1;
    }
}
