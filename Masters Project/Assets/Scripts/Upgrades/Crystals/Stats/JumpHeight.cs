using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHeight : IStat
{
    [SerializeField] private float statBase;
    public override void LoadStat(PlayerController player, int mod)
    {
        float newForce;
        newForce = player.GetJumpForce() + (((float)mod * statBase) * player.GetJumpForce());
        player.SetJumpForce(newForce);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        float newForce;
        newForce = player.GetJumpForce() - (((float)mod * statBase) * player.GetJumpForce());
        player.SetJumpForce(newForce);
    }

    public override float GetStatIncrease(int mod)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        return ((float)mod * statBase) * player.GetJumpForce();
    }
}
