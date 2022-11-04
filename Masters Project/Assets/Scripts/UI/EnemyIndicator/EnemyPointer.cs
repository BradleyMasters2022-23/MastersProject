/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages an individual enemy pointer indicator
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    /// <summary>
    /// reference to image component
    /// </summary>
    private Canvas img;

    /// <summary>
    /// Target enemy to monitor
    /// </summary>
    private GameObject targetEnemy;
    /// <summary>
    /// Whether this pointer has been initialized
    /// </summary>
    private bool initialized = false;

    [Tooltip("How close to the actual player horizontal view does this arrow hide. Forms a cone from line of sigt")]
    [SerializeField] private float hideYRadius;

    [Tooltip("How high and low can this arrow point")]
    [SerializeField] private float maxXAngle;

    private void Update()
    {
        // Dont update if not initialized
        if (!initialized)
            return;

        // If enemy is null, then enemy died. Destroy counter
        if (targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }

        // Get Y angle for pointing at enemy
        Vector3 dir = targetEnemy.transform.position - transform.position;
        float angY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (angY > 180)
            angY -= 360;

        // Get clamped X angle for pointing at enemy
        float angX = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angX > 180)
            angX -= 360;

        angX = -Mathf.Clamp(angX, -maxXAngle, maxXAngle);

        // apply new rotations
        transform.rotation = Quaternion.Euler(angX, angY, 0);


        // Check how 'infront' the arrow is pointing, check if it should hide
        float angleYDiff = (transform.parent.rotation.eulerAngles.y % 360) - (transform.rotation.eulerAngles.y % 360);

        if (Mathf.Abs(angleYDiff) <= hideYRadius && img.enabled == true)
        {
            img.enabled = false;
        }
        else if (Mathf.Abs(angleYDiff) > hideYRadius && img.enabled == false)
        {
            img.enabled = true;
        }
    }

    public void SetTarget(GameObject target)
    {
        targetEnemy = target;
        img = GetComponentInChildren<Canvas>();

        initialized = true;
    }
}
