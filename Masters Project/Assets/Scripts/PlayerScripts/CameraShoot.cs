/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 23th, 2022
 * Last Edited - October 23th, 2022 by Ben Schuster
 * Description - Manages the hitscan for the third person shooting
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShoot : MonoBehaviour
{
    /// <summary>
    /// The default target point if the hitscan fails
    /// </summary>
    private Transform defaultTarget;

    /// <summary>
    /// Layers should the hitscan ignore
    /// </summary>
    private LayerMask layersToIgnore;

    /// <summary>
    /// The minimum range for thie hitscan to work
    /// </summary>
    private float minRange;

    /// <summary>
    /// Current target position.
    /// </summary>
    private Vector3 targetPos;
    /// <summary>
    /// Current target position.
    /// </summary>
    public Vector3 TargetPos
    {
        get { return targetPos; }
    }

    // Planes for the camera
    private Plane[] planes;

    [Tooltip("How accurate is the 'in camera' system. 0 is perfect accuracy")]
    [SerializeField] private float viewPlanesTolerance;

    /// <summary>
    /// initialize the shoot camera with necessary variables
    /// </summary>
    /// <param name="_defaultTarget">The default target point if the hitscan fails</param>
    /// <param name="_layersToIgnore">Layers should the hitscan ignore</param>
    /// <param name="_minRange">The minimum range for thie hitscan to work</param>
    public void Initialize(Transform _defaultTarget, LayerMask _layersToIgnore, float _minRange)
    {
        defaultTarget = _defaultTarget;
        layersToIgnore = _layersToIgnore;
        minRange = _minRange;

        targetPos = _defaultTarget.position;
    }

    private void Update()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        // Dont fire if not initialized
        if(defaultTarget != null)
        {
            Fire();
        }
    }

    /// <summary>
    /// Fire a raycast, update the target
    /// </summary>
    private void Fire()
    {
        // Prepare hitscan info
        RaycastHit hitInfo;

        // Fire hitscan
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, ~layersToIgnore))
        {
            // If distance is below minimum range, fire at default target instead
            if(hitInfo.distance < minRange)
            {
                targetPos = defaultTarget.position;
            }
            // Otherwise, update target position with hit
            else
            {
                targetPos = hitInfo.point;
            }
        }
        else
        {
            // If no hit, use default target
            targetPos = defaultTarget.position;
        }
    }

    /// <summary>
    /// Check if a point is in vision of the camera
    /// </summary>
    /// <param name="pos">point to check</param>
    /// <returns>In camera vision</returns>
    public bool InCamVision(Vector3 pos)
    {
        foreach (Plane plane in planes)
        {
            if (plane.GetDistanceToPoint(pos) <= viewPlanesTolerance)
            {
                return false;
            }
        }

        return true;
    }
}
