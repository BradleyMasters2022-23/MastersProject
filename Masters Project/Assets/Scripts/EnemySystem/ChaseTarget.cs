using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class ChaseTarget : MonoBehaviour
{
    private enum State
    {
        Moving,
        Offlink
    }

    [SerializeField] private State state;

    [SerializeField] private Transform target;

    Vector3 lastTargetPos;

    private NavMeshAgent nav;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private float jumpDuration;

    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private float fallDuration;

    private float defSpeed;
    private float defRot;


    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        defSpeed = nav.speed;
        defRot = nav.angularSpeed;
        
    }

    private void Update()
    {
        nav.speed = TimeManager.WorldTimeScale * defSpeed;
        nav.angularSpeed = TimeManager.WorldTimeScale * defRot;


        StateUpdate();
    }


    private void StateUpdate()
    {
        switch(state)
        {
            case State.Moving:
                {
                    Chase();
                    break;
                }
                case State.Offlink:
                {
                    break;
                }
        }
    }

    private void Chase()
    {
        if (target == null)
        {
            nav.ResetPath();
            return;
        }

        if (target.position != lastTargetPos)
        {
            lastTargetPos = target.position;
            nav.SetDestination(target.position);
        }

        if (nav.isOnOffMeshLink)
        {
            JumpToLedge();
        }
    }

    private void JumpToLedge()
    {
        state = State.Offlink;

        OffMeshLinkData data = nav.currentOffMeshLinkData;

        // Check whether to use jump or fall curve
        if (data.endPos.y >= data.startPos.y)
        {
            StartCoroutine(CurvedJump(jumpCurve, jumpDuration));
        }
        else
        {
            StartCoroutine(CurvedJump(fallCurve, fallDuration));
        }
        
    }

    private IEnumerator CurvedJump(AnimationCurve curve, float duration)
    {
        OffMeshLinkData data = nav.currentOffMeshLinkData;
        Vector3 startPos = nav.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * nav.baseOffset;

        // Set rotation to look torwards where they're jumping (lerp this later)
        nav.updateRotation = false;
        Quaternion rot = Quaternion.LookRotation((endPos-startPos).normalized);
        rot.x = transform.rotation.x;
        rot.z = transform.rotation.z;
        transform.rotation = rot;

        // Set to one because of the time
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = curve.Evaluate(normalizedTime);
            nav.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += TimeManager.WorldDeltaTime / duration;

            yield return null;
        }

        nav.CompleteOffMeshLink();
        state = State.Moving;
        nav.updateRotation = true;
    }
}
