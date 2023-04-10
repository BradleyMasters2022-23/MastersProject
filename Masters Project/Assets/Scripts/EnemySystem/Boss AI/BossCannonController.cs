using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossCannonController : MonoBehaviour
{
    [SerializeField] private Target target;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float acquireTolerance;
    private CannonTrackPlayer trackState;

    private IState currState;

    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] protected float rotateSpeed;

    public void RotateToTarget()
    {
        Quaternion currRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(target.Center.position - transform.position);

        float maxRot = rotateSpeed * TimeManager.WorldTimeScale;

        if (maxRot > 0)
            transform.rotation = Quaternion.RotateTowards(currRot, targetRot, maxRot);
    }
}