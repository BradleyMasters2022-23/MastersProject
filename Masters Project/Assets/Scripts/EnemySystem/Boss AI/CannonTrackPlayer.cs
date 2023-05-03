using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTrackPlayer : MonoBehaviour
{
    private float rotateSpeed;
    private Transform target;
    private bool affectedByTimestop;

    private float accuracy;
    private Transform mainBody;

    //public CannonTrackPlayer(Transform mainBody, Target target, float rotateSpeed, float accuracy)
    //{
    //    this.mainBody = mainBody;
    //    this.target = target.Center;
    //    this.rotateSpeed = rotateSpeed;
    //    this.accuracy = accuracy;
    //}

    public void UpdateState()
    {
        Quaternion currRot = mainBody.rotation;
        Quaternion targetRot = Quaternion.LookRotation(target.position - mainBody.position);

        float maxRot = rotateSpeed * TimeManager.WorldTimeScale;

        if(maxRot > 0)
            mainBody.rotation = Quaternion.RotateTowards(currRot, targetRot, maxRot);
        
        CheckAcquired();
    }

    private void CheckAcquired()
    {
        Vector3 targetDirection = target.position - mainBody.position;
        float dot = Vector3.Dot(mainBody.forward.normalized, targetDirection.normalized);
    }
}
