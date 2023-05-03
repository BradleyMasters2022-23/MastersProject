using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    public Target Target();

    public void RegisterEffect(float dmg);

    public void RegisterEffect(TeamDamage data, float dmgMultiplier = 1);
}
