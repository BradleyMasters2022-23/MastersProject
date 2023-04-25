using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHeight : IStat
{

    [SerializeField] private float statBase;
    private PlayerController p;
    private float baseForce;

    private void Start()
    {
        p = FindObjectOfType<PlayerController>();
        baseForce = p.GetJumpForce();
    }

    public override void LoadStat(PlayerController player, int mod)
    {
        float newForce;
        newForce = player.GetJumpForce() + (((float)mod * statBase) * baseForce);
        player.SetJumpForce(newForce);
    }

    public override void UnloadStat(PlayerController player, int mod)
    {
        float newForce;
        newForce = player.GetJumpForce() - (((float)mod * statBase) * baseForce);
        player.SetJumpForce(newForce);
    }

    public override float GetStatIncrease(int mod)
    {
        return ((float)mod * statBase) * 100;
    }
}
