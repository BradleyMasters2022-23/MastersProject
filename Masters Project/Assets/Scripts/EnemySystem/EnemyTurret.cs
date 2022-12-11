using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyTurret : EnemyBase
{
    [SerializeField] private EnemyState state;

    public GameObject turretPoint;
    public Transform[] shootPoints;

    public float rotationSpeed;
    public GameObject shotPrefab;
    public float shootTime;

    [Tooltip("Range the enemy can attack from")]
    public float attackRange;
    private float currDist;
    public int lookRadius;

    private float time = 0;
    private float currTime = 1;

    private Vector3 neutralLook;

    private bool waitingNeutral;

    private ScaledTimer returnNeutralDelay;

    private Vector3 target;

    private float shotRadius;

    [Tooltip("What layers affects this enemy's vision")]
    [SerializeField] private LayerMask visionLayer;

    [SerializeField] private AttackTarget attackSystem;

    // Start is called before the first frame update
    void Start()
    {
        //neutralLook = shootPoints[0].transform.forward*3;
        currTime = TimeManager.WorldTimeScale;
        shotRadius = shotPrefab.GetComponent<SphereCollider>().radius * .5f;
        neutralLook = turretPoint.transform.forward * 5;

        returnNeutralDelay = new ScaledTimer(4f);
        attackSystem.GetComponent<AttackTarget>();
    }

    private void FixedUpdate()
    {
        CheckState();

        // Get current time state
        currTime = TimeManager.WorldTimeScale;
        currDist = Vector3.Distance(playerCenter.position, transform.position);

        switch (state)
        {
            case EnemyState.Idle:
                {
                    break;
                }
            case EnemyState.Attacking:
                {

                    if(attackSystem.currentAttackState == AttackState.Ready)
                    {
                        attackSystem.Attack(player.transform);
                    }

                    // Shoot every few seconds if in range
                    //if (time >= shootTime)
                    //{
                    //    foreach (Transform t in shootPoints)
                    //    {
                    //        Shoot(t);
                    //    }

                    //    time = 0;
                    //}
                    //else if (time < shootTime)
                    //{
                    //    time += Time.deltaTime * currTime;
                    //}

                    break;
                }
        }

        // Get direction of target, rotate towards them




        Vector3 direction = (target - shootPoints[0].position).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);

        if((attackSystem.currentAttackState == AttackState.Cooldown || attackSystem.currentAttackState == AttackState.Ready))
        {
            // Limit the next angles
            float nextXAng = Mathf.Clamp(Mathf.DeltaAngle(turretPoint.transform.localRotation.eulerAngles.x, rot.eulerAngles.x), -rotationSpeed, rotationSpeed) * currTime;
            float nextYAng = Mathf.Clamp(Mathf.DeltaAngle(gameObject.transform.rotation.eulerAngles.y, rot.eulerAngles.y), -rotationSpeed, rotationSpeed) * currTime;

            // Adjust the vertical neck of the turret
            turretPoint.transform.localRotation = Quaternion.Euler(turretPoint.transform.localRotation.eulerAngles.x + nextXAng, 0, 0);
            // Adjust the horizontal base of the turret
            gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y + nextYAng, 0);
        }
    }

    /// <summary>
    /// Check state, see if it should swap
    /// </summary>
    /// <returns>New state</returns>
    private void CheckState()
    {
        switch (state)
        {
            case EnemyState.Idle:
                {
                    // Switch to attacking if line of sight and within ideal range
                    if (LineOfSight(playerCenter))
                    {
                        state = EnemyState.Attacking;
                    }

                    break;
                }
            case EnemyState.Attacking:
                {
                    if (!LineOfSight(playerCenter) && returnNeutralDelay.TimerDone())
                    {
                        state = EnemyState.Idle;
                        waitingNeutral = false;
                        target = neutralLook;
                        
                        
                        time = shootTime / 2;
                    }

                    break;
                }
        }
    }

    /// <summary>
    /// Check if this object has line of sight on target via raycast
    /// </summary>
    /// <param name="target">target to check</param>
    /// <returns>Line of sight</returns>
    public bool LineOfSight(Transform t)
    {
        // First target direct, then higher, then lower, 
        Vector3 dir = t.position - turretPoint.transform.position;
        Vector3 dirHigh = (t.position + t.up * .35f) - turretPoint.transform.position;
        Vector3 dirLow = (t.position + t.up * -.7f) - turretPoint.transform.position;

        // Set mask to ignore raycasts and enemy layer
        //Debug.DrawRay(turretPoint.transform.position, dir, Color.red, 2f);
        //Debug.DrawRay(turretPoint.transform.position, dirHigh, Color.blue, 2f);
        //Debug.DrawRay(turretPoint.transform.position, dirLow, Color.green, 2f);

        // Try to get player
        RaycastHit hit;
        
        if (Physics.SphereCast(turretPoint.transform.position, shotRadius, dir, out hit, attackRange, visionLayer)
            || Physics.SphereCast(turretPoint.transform.position, shotRadius, dirHigh, out hit, attackRange, visionLayer)
            || Physics.SphereCast(turretPoint.transform.position, shotRadius, dirLow, out hit, attackRange, visionLayer))
        {
            if (hit.transform.CompareTag("Player"))
            {
                waitingNeutral = false;
                target = hit.point;
                returnNeutralDelay.ResetTimer();
                return true;
            }
        }

        if (!waitingNeutral)
        {
            returnNeutralDelay.ResetTimer();
            waitingNeutral = true;
        }
            

        return false;
    }


    private bool InVision()
    {
        Vector3 temp = (playerCenter.position - turretPoint.transform.position).normalized;
        float angle = Vector3.SignedAngle(temp, transform.forward, Vector3.up);
        return (Mathf.Abs(angle) <= lookRadius);
    }

    private void Shoot(Transform point)
    {
        GameObject o = Instantiate(shotPrefab, point.transform.position, point.transform.rotation);
        o.GetComponent<RangeAttack>().Activate();
    }
}
