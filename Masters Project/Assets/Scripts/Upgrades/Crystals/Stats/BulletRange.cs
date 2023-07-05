using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRange : IStat
{
    private PlayerGunController gun;
    [SerializeField] private float statBase = 1;

    public override void LoadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetMaxRange(gun.GetMaxRange() + (gun.GetOriginalRange() * statBase * (float)mod));
        // Debug.Log("Bullet range up " + (statBase * (float)mod).ToString());
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetMaxRange(gun.GetMaxRange() - (gun.GetOriginalRange() * statBase * (float)mod));
    }

    public override float GetStatIncrease(int mod)
    {
        return (float)mod * statBase * 100;
    }
}
