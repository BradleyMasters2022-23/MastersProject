using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEnemyProjectile : ProjectileAccelerate
{
    [SerializeField] private Target targetManager;
    [SerializeField] private float shrinkTime;

    bool shrinkStarted = false;

    /// <summary>
    /// On mine enemy, detonate the explosive
    /// </summary>
    /// <param name="target"></param>
    protected override bool DealDamage(Transform _targetObj)
    {
        // only detonate if it impacted with a damagable entity
        if(_targetObj.GetComponent<IDamagable>() != null)
        {
            targetManager.RegisterEffect(999);
        }    

        return true;
    }

    protected override void ApplyDamage(Transform target, Vector3 damagePoint)
    {
        if (target.GetComponent<IDamagable>() != null)
        {
            targetManager.RegisterEffect(999);
        }
    }

    protected override void End()
    {
        if (!shrinkStarted)
        {
            StartCoroutine(ShrinkRoutine());
            shrinkStarted = true;
        }
    }

    private IEnumerator ShrinkRoutine()
    {
        LocalTimer t = GetTimer(shrinkTime);
        Vector3 baseScale = transform.localScale;

        while(!t.TimerDone())
        {
            transform.localScale = baseScale * (1 - t.TimerProgress());
            yield return null;
        }

        base.End();

        yield return null;
    }
}
