using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRange : IStat
{
    private PlayerGunController gun;
    [SerializeField] private int statBase = 1;

    public override void LoadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
        gun.SetMaxRange(gun.GetMaxRange() + (statBase * (float)mod));
        Debug.Log("Bullet range up");
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        gun = player.gameObject.GetComponent<PlayerGunController>();
    }

    public override float GetStatIncrease(int mod)
    {
        return 1;
    }
}
