using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Damagable
{
    [SerializeField] private float damageMultiplier = 1;

    private EnemyHealth enemy;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyHealth>();
    }

    public override void Damage(int _dmg)
    {
        // Buff damage, send to host
        int buffedDmg = Mathf.CeilToInt(_dmg);

        enemy.Damage(buffedDmg);
    }

    protected override void Die()
    {
        return;
    }
}
