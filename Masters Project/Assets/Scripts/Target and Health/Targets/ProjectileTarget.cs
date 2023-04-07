using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTarget : Target
{
    protected override void DestroyObject()
    {
        GetComponent<Projectile>().Inturrupt();
        base.DestroyObject();
    }
}
