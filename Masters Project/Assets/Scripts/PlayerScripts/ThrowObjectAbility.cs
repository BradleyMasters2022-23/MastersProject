using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowObjectAbility : Ability
{
    
    [Header("=== Throw Object Stats ===")]
    [Tooltip("The projectile to be thrown")]
    [SerializeField] private GameObject projectile;
    [Tooltip("Number of projectiles to be thrown in one activation")]
    [SerializeField] private int numOfProjectiles;
    [Tooltip("Where the projectile spawns from")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Force of the projectile being launched forwards")]
    [SerializeField] private float force;
    [Tooltip("Extra force to increase initial height")]
    [SerializeField] private float verticalArcForce;
    [Tooltip("Amount of inaccuracy. 0 results in perfect accuracy.")]
    [SerializeField] protected float throwInaccuracy;
    [Tooltip("Whether the player's velocity is added to the thrown projectile")]
    [SerializeField] protected bool includePlayerVelocity;

    [SerializeField] protected AudioClipSO throwAbilitySound;

    protected override IEnumerator OnAbility()
    {
        if(projectile is null)
        {
            Debug.LogError("Tried throwing projectile, but no projectile added!");
            yield break;
        }

        for(int i = 0; i < numOfProjectiles; i++)
        {
            GameObject newProj =
            Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);

            // Apply arc

            // If any inaccuracy, apply the spread. Otherwise skip this for optimization sake
            if (throwInaccuracy > 0)
                newProj.transform.eulerAngles = ApplySpread(newProj.transform.eulerAngles);

            // Clamp angle to prevent going too high or low
            Vector3 rotation = newProj.transform.eulerAngles;
            rotation.x = Mathf.Clamp(rotation.x, -80, 90);
            //newProj.transform.eulerAngles = rotation;

            Rigidbody projRB = newProj.GetComponent<Rigidbody>();
            if (projRB != null)
            {
                // Add upwards force for arc
                Vector3 upwardsForce = Vector3.up * verticalArcForce;

                // apply current velocity with it too
                if (includePlayerVelocity)
                {
                    Vector3 currVel = GetComponent<Rigidbody>().velocity;
                    projRB.GetComponent<Throwable>().ApplyStartingForce(upwardsForce + currVel + spawnPoint.transform.forward * force);
                    //projRB.AddForce(upwardsForce + currVel + newProj.transform.forward * force, ForceMode.Impulse);
                }
                else
                {
                    projRB.GetComponent<Throwable>().ApplyStartingForce(upwardsForce + spawnPoint.transform.forward * force);
                    //projRB.AddForce(upwardsForce + newProj.transform.forward * force, ForceMode.Impulse);
                }

                throwAbilitySound.PlayClip(transform);
            }
            else
            {
                Destroy(newProj);
            }
        }

        //HideLine();
        yield return null;
    }

    private Vector3 ApplySpread(Vector3 rot)
    {
        return rot = new Vector3(
            rot.x + Random.Range(-throwInaccuracy, throwInaccuracy),
            rot.y + Random.Range(-throwInaccuracy, throwInaccuracy),
            rot.z + Random.Range(-throwInaccuracy, throwInaccuracy));
    }

    // Line projection stuff
    /*
    protected virtual void Trajectory(Vector3 launchVector, Rigidbody projRB, Vector3 startPoint)
    {
        // if not loading in, then dont bother
        if (!trajectoryArc.enabled)
            return;

        Vector3 startVelocity = (launchVector / projRB.mass);

        int i = 0;
        trajectoryArc.positionCount = Mathf.CeilToInt(trajectoryStepCount / timeBetweenPoints) + 1;
        trajectoryArc.SetPosition(i, startPoint);

        for (float time = 0; time < trajectoryStepCount; time += timeBetweenPoints)
        {
            i++;
            Vector3 point = startPoint + time * startVelocity;
            point.y = startPoint.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);
            trajectoryArc.SetPosition(i, point);
        }
    }

    private void ShowLine()
    {
        if (trajectoryArc != null)
            trajectoryArc.enabled = true;
    }
    private void HideLine()
    {
        if (trajectoryArc != null)
            trajectoryArc.enabled = false;
    }

    public override void OnHold()
    {
        ShowLine();
    }
    public override void Cancel()
    {
        HideLine();
    }
    */
}
