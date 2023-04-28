using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitbox : MonoBehaviour, IDamagable
{
    [SerializeField] Target target;
    [SerializeField] float damageMultiplier = 1;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponentInParent<Target>();
            if (target == null)
                Destroy(this);
        }
    }

    public void RegisterEffect(float dmg)
    {
        target?.RegisterEffect(dmg * damageMultiplier);
    }

    public void RegisterEffect(TeamDamage data, float dmgMultiplier = 1)
    {
        target?.RegisterEffect(data, dmgMultiplier * damageMultiplier);
    }

    public Target Target()
    {
        return target;
    }
}
