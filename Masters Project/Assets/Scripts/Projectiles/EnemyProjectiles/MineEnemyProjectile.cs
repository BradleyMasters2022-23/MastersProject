using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEnemyProjectile : ProjectileAccelerate
{
    [SerializeField] private Target targetManager;
    [SerializeField] private float shrinkTime;

    /// <summary>
    /// On mine enemy, detonate the explosive
    /// </summary>
    /// <param name="target"></param>
    protected override bool DealDamage(Transform _targetObj)
    {
        targetManager.RegisterEffect(999);
        return true;
    }

    protected override void End()
    {
        StartCoroutine(ShrinkRoutine());

        //base.End();
    }

    private IEnumerator ShrinkRoutine()
    {
        ScaledTimer t = new ScaledTimer(shrinkTime, false);
        Vector3 baseScale = transform.localScale;

        while(!t.TimerDone())
        {
            t.SetModifier(Timescale);
            transform.localScale = baseScale * (1 - t.TimerProgress());
            yield return null;
        }

        base.End();

        yield return null;
    }


}
