using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalContainer : MonoBehaviour
{
    private Crystal crystal;
    private int par;

    private void Start()
    {
        par = LinearSpawnManager.instance.GetCombatRoomCount();
        crystal = CrystalManager.instance.GenerateCrystal(par);
    }
}
