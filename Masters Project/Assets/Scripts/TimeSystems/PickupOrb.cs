/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages the behavior of a pickup orb
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupOrb : MonoBehaviour
{
    /// <summary>
    /// this objects rigidbody
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// Reference to player transform
    /// </summary>
    protected Transform player;

    [Tooltip("Whether this object is affected with timestop")]
    [SerializeField] protected bool affectedByTimestop;

    [Header("=====Spawning=====")]

    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] protected Vector2 dropVelocityRange;

    [Tooltip("Range of velocity items can drop with")]
    [SerializeField] protected Vector2 dropXAngleRange;

    [Tooltip("How high does this float in the air")]
    [SerializeField] protected float floatHeight;

    [SerializeField] protected LayerMask groundMask;

    [Header("=====Pickup=====")]

    [Tooltip("Range before getting picked up")]
    [SerializeField] protected float pickupRadius;

    [Tooltip("Range before chasing the target")]
    [SerializeField] protected float chaseRadius;

    [Tooltip("Speed it goes to the player at")]
    [SerializeField] protected float chaseSpeed;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>().CenterMass;

        // Randomly generate velocity and rotation angles
        float vel = Random.Range(dropVelocityRange.x, dropVelocityRange.y);
        float angY = Random.Range(0, 360);
        float angX = -Random.Range(dropXAngleRange.x, dropXAngleRange.y);

        transform.rotation = Quaternion.Euler(angX, angY, 0);

        rb.velocity = transform.forward * vel;

        GetComponent<SphereCollider>().radius = pickupRadius;
    }

    protected void Update()
    {
        RaycastHit hitInfo;

        // Switch to kinematic after the initial burst after spawning
        if(!rb.isKinematic)
        {
            Debug.DrawRay(transform.position, Vector3.down * floatHeight);
            // Check if it needs to start floating
            if (rb.velocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, out hitInfo, floatHeight, groundMask))
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.mass /= 2;
            }
        }
        // If it can chase, chase the player
        else if(CheckChaseRequirements())
        {
            // Chase the player
            Vector3 dir = player.position - transform.position;

            // Modify chase formula based on timestop preference
            if(affectedByTimestop)
            {
                rb.transform.position += dir.normalized * chaseSpeed * TimeManager.WorldDeltaTime;
            }
            else
            {
                rb.transform.position += dir.normalized * chaseSpeed * Time.deltaTime;
            }
        }
        // Otherwise, try falling again
        else if(rb.isKinematic && !Physics.Raycast(transform.position, Vector3.down, out hitInfo, floatHeight, groundMask))
        {
            rb.isKinematic = false;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnPickup();
        }
    }

    /// <summary>
    /// What happens when this item is picked up
    /// </summary>
    protected abstract void OnPickup();

    /// <summary>
    /// Check any chase requirements before chasing
    /// </summary>
    /// <returns>Whether the chase requirements have been met</returns>
    protected abstract bool CheckChaseRequirements();
}
