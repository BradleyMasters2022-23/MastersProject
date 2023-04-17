using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCount : IStat
{
    // theres problems
    Ability grenade;
    public override void LoadStat(PlayerController player, int mod)
    {
        grenade = player.gameObject.GetComponent<Ability>();
        grenade.IncreaseMaxCharges(mod);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        grenade = player.gameObject.GetComponent<Ability>();
        grenade.IncreaseMaxCharges(mod * -1);
    }

    public override float GetStatIncrease(int mod)
    {
        return mod;
    }
}
