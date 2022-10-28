using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : Damagable
{
    [Range(0f, 5f)]
    [SerializeField] private float damageMultiplier = 1;

    private EnemyHealth enemy;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyHealth>();
    }

    public override void Damage(int _dmg)
    {
        // Buff damage, send to host
        int modifiedDamage = Mathf.CeilToInt(_dmg * damageMultiplier);

        // Testing logs
        //if (damageMultiplier > 1)
        //    Debug.Log("enemy taking extra damage from vulnerable point!");
        //else if (damageMultiplier < 1)
        //    Debug.Log("enemy taking less damage from armored point!");

        enemy.Damage(modifiedDamage);
    }

    protected override void Die()
    {
        return;
    }
}
