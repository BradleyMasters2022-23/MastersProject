using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weakpoint : Damagable
{
    [SerializeField] private float damageMultiplier = 2;

    private EnemyHealth enemy;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyHealth>();
    }

    public override void Damage(int _dmg)
    {
        // Buff damage, send to host 
        Debug.Log($"{enemy.name} is taking extra damage!");
        int buffedDmg = Mathf.CeilToInt(_dmg * damageMultiplier);

        enemy.Damage(buffedDmg);
    }

    protected override void Die()
    {
        return;
    }
}
