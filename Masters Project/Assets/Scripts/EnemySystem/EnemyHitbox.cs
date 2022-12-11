using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using static Unity.VisualScripting.Member;
using UnityEngine.Rendering;

public class EnemyHitbox : Damagable
{
    [Range(0f, 5f)]
    [SerializeField] private float damageMultiplier = 1;

    private EnemyHealth enemy;

    [Tooltip("Weak spot sound effect")]
    [SerializeField] private AudioClip weakPoint;
    [Tooltip("Armored spot sound effect")]
    [SerializeField] private AudioClip armoredPoint;
    [Tooltip("Sound when its a basic hit")]
    [SerializeField] private AudioClip basicHit;


    private void Start()
    {
        enemy = GetComponentInParent<EnemyHealth>();
    }

    public override void Damage(int _dmg)
    {
        // Buff damage, send to host
        int modifiedDamage = Mathf.CeilToInt(_dmg * damageMultiplier);

        // Testing logs
        if (damageMultiplier > 1)
        {
            //Debug.Log("enemy taking extra damage from vulnerable point!");
            if(weakPoint!=null)
                AudioSource.PlayClipAtPoint(weakPoint, Camera.main.transform.position,0.5f);
        }
        else if (damageMultiplier < 1)
        {
            //Debug.Log("enemy taking less damage from armored point!");
            if (armoredPoint != null)
                AudioSource.PlayClipAtPoint(armoredPoint, Camera.main.transform.position, 0.5f);
        }
        else
        {
            //Debug.Log("Enemy takng hit from basic attack!");
            if (basicHit != null)
                AudioSource.PlayClipAtPoint(basicHit, Camera.main.transform.position, 0.5f);
        }
            

        enemy.Damage(modifiedDamage);
    }

    protected override void Die()
    {
        return;
    }
}
