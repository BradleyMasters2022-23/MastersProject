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


    /// <summary>
    /// Register an effect to the main target, applying any modifier
    /// </summary>
    /// <param name="dmg"></param>
    public void RegisterEffect(float dmg)
    {
        target?.RegisterEffect(dmg * damageMultiplier);
    }

    /// <summary>
    /// Register an effect to the main target, applying any damage multiplier. addative with base modifier.
    /// </summary>
    /// <param name="data">Team data to use</param>
    /// <param name="dmgMultiplier">multiplier to add</param>
    public void RegisterEffect(TeamDamage data, float dmgMultiplier = 1)
    {
        target?.RegisterEffect(data, dmgMultiplier + damageMultiplier);
    }

    /// <summary>
    /// Get reference to the target of this hitbox
    /// </summary>
    /// <returns></returns>
    public Target Target()
    {
        return target;
    }
}
