using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyVerticalMobility : MonoBehaviour
{
    [SerializeField] private float maxJumpHeight;

    [SerializeField] private float maxFallHeight;

    [SerializeField] private float detectionRange;

    [SerializeField] private LayerMask detectionLayers;

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForLedges();

        
    }

    private void CheckForLedges()
    {
        // Check if there is a wall in the way, and if so, check for a jump


    }

    private void CheckForJump()
    {
        // useful vars
        RaycastHit test;
        RaycastHit baseGround;
        Physics.Raycast(transform.position, Vector3.down, out baseGround, maxFallHeight, detectionLayers);

        // Visual indicator for jumping
        // Check for jumping
        Vector3 jumpArcPoint = transform.position + transform.up * maxJumpHeight;
        Debug.DrawLine(transform.position, jumpArcPoint, Color.red, 1f);
        Debug.DrawLine(jumpArcPoint, jumpArcPoint + transform.forward * detectionRange, Color.red, .1f);

        // get current ground to test
        if (!Physics.Raycast(transform.position, Vector3.down, out baseGround, maxFallHeight, detectionLayers))
        {
            Debug.Log("base ground not detected!");
            return;
        }
        // make it so if the enemy is on a slope, dont try anything
        if (baseGround.normal != Vector3.up)
        {
            return;
        }

        // Find land point for jumping
        if (Physics.Raycast(jumpArcPoint + transform.forward * detectionRange, Vector3.down, out test, maxJumpHeight, detectionLayers))
        {
            if (test.point.y > baseGround.point.y && test.normal == Vector3.up)
            {
                Debug.Log("Required jump detected!");
                return;
            }
        }
    }

    private void CheckForFall()
    {
        // Visual Indicator for falling
        Vector3 fallArcPoint = transform.position + transform.forward * detectionRange;
        Debug.DrawLine(transform.position, fallArcPoint, Color.green, 1f);
        Debug.DrawLine(fallArcPoint, fallArcPoint + Vector3.down * maxFallHeight, Color.green, .1f);

        // useful vars
        RaycastHit test;
        RaycastHit baseGround;
        Physics.Raycast(transform.position, Vector3.down, out baseGround, maxFallHeight, detectionLayers);

        // get current ground to test
        if (!Physics.Raycast(transform.position, Vector3.down, out baseGround, maxFallHeight, detectionLayers))
        {
            Debug.Log("base ground not detected!");
            return;
        }
        // make it so if the enemy is on a slope, dont try anything
        if (baseGround.normal != Vector3.up)
        {
            return;
        }

        // Find land point for falling
        if (Physics.Raycast(fallArcPoint, Vector3.down, out test, maxFallHeight, detectionLayers))
        {
            if (test.point.y < baseGround.point.y && test.normal == Vector3.up)
            {
                Debug.Log("Required fall detected!");
                return;
            }
        }
    }

    private void LeapToTarget(Vector3 target)
    {

    }
}
