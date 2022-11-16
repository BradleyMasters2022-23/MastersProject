/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - November 4th, 2022
 * Last Edited - November 4th, 2022 by Ben Schuster
 * Description - Manages an individual enemy pointer indicator
 * ================================================================================================
 */
using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPointer : MonoBehaviour
{
    /// <summary>
    /// reference to image component
    /// </summary>
    private GameObject img;

    /// <summary>
    /// Target enemy to monitor
    /// </summary>
    private Transform targetEnemy;
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

        // Hide if enemy is not enabled
        if (!targetEnemy.gameObject.activeInHierarchy)
        {
            if(img.activeInHierarchy)
                img.SetActive(false);

            return;
        }
        else if (targetEnemy.gameObject.activeInHierarchy && !img.activeInHierarchy)
        {
            img.SetActive(true);
        }

        //Rotate arrow towards target
        transform.LookAt(targetEnemy.position);

        // limit vertical rotation (x-axis)
        float angX = transform.rotation.eulerAngles.x;
        if (angX > 180)
            angX -= 360;

        transform.rotation = Quaternion.Euler(
            Mathf.Clamp(angX, -maxXAngle, maxXAngle),
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z);

        // Check how 'infront' the arrow is pointing, check if it should hide
        float angleYDiff = (transform.parent.rotation.eulerAngles.y % 360) - (transform.rotation.eulerAngles.y % 360);

        if (Mathf.Abs(angleYDiff) <= hideYRadius && img.activeInHierarchy == true)
        {
            img.SetActive(false);
        }
        else if (Mathf.Abs(angleYDiff) > hideYRadius && img.activeInHierarchy == false)
        {
            img.SetActive(true);
        }
    }

    /// <summary>
    /// Set the target enemy
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(GameObject target)
    {
        targetEnemy = target.GetComponent<EnemyBase>().centerMass;
        img = transform.GetChild(0).gameObject;

        initialized = true;
    }
}
