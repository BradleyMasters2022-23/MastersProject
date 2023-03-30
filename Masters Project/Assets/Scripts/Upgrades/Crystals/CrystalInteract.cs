using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalInteract : Interactable
{
    private Crystal crystal;
    private int par;

    private void Start()
    {
        par = LinearSpawnManager.instance.GetCombatRoomCount();
        crystal = CrystalManager.instance.GenerateCrystal(par);
    }

    public override void OnInteract(PlayerController player)
    {

    }
}
