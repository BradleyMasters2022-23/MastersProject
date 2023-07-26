using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    public Target Target();

    public void RegisterEffect(float dmg, Vector3 orgin);

    public void RegisterEffect(TeamDamage data, Vector3 origin, float dmgMultiplier = 1);
}
