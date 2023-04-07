using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : IStat
{
    private PlayerGunController gun;
    private int totalMod;

    /// <summary>
    /// Multiplies with the mod to get the amount to increase the total damage modifier by.
    /// </summary>
    [SerializeField] private float statBase;
    public override void LoadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetDamageMultiplier((float)mod * statBase + gun.GetDamageMultiplier());
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetDamageMultiplier(gun.GetDamageMultiplier() - ((float)mod * statBase));
    }
}
