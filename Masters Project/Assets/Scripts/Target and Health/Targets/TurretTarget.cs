using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTarget : EnemyTarget
{
    [Header("Turret specifics")]
    [SerializeField] Transform turretStand;
    /// <summary>
    /// spawn rotation of the turret base. Used to keep it stationary without unparenting
    /// </summary>
    Quaternion spawnRotation;

    protected override void Awake()
    {
        base.Awake();
        spawnRotation = turretStand.rotation;
    }

    protected override void Update()
    {
        base.Update();
        turretStand.rotation = spawnRotation;
    }

    protected override void KillTarget()
    {
        turretStand.gameObject.SetActive(false);
        turretStand.localRotation= Quaternion.identity;
        base.KillTarget();
    }

    public override void PoolPull()
    {
        base.PoolPull();
        turretStand.gameObject.SetActive(true);
        spawnRotation = turretStand.transform.rotation;
    }
}
