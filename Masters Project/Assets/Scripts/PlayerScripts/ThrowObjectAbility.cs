using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectAbility : Ability
{
    
    [Header("=== Throw Object Stats ===")]
    [Tooltip("The projectile to be thrown")]
    [SerializeField] private GameObject projectile;
    [Tooltip("Number of projectiles to be thrown in one activation")]
    [SerializeField] private int numOfProjectiles;
    [Tooltip("Where the projectile spawns from")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Extra force to increase initial height")]
    [SerializeField] private float verticalArcForce;

    [Tooltip("Force of the projectile being launched forwards")]
    [SerializeField] private float force;
    [Tooltip("Amount of inaccuracy. 0 results in perfect accuracy.")]
    [SerializeField] protected float throwInaccuracy;
    [Tooltip("Whether the player's velocity is added to the thrown projectile")]
    [SerializeField] protected bool includePlayerVelocity;

    protected override IEnumerator OnAbility()
    {
        if(projectile is null)
        {
            Debug.Log("Tried throwing projectile, but no projectile added!");
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
                    projRB.AddForce(upwardsForce + currVel + newProj.transform.forward * force, ForceMode.Impulse);
                }
                else
                {
                    projRB.AddForce(upwardsForce + newProj.transform.forward * force, ForceMode.Impulse);
                }
                
            }
            else
            {
                Destroy(newProj);
            }
        }

        yield return null;
    }

    private Vector3 ApplySpread(Vector3 rot)
    {
        return rot = new Vector3(
            rot.x + Random.Range(-throwInaccuracy, throwInaccuracy),
            rot.y + Random.Range(-throwInaccuracy, throwInaccuracy),
            rot.z + Random.Range(-throwInaccuracy, throwInaccuracy));
    }
}
