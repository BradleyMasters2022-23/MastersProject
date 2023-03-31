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
    private LayerMask hitLayers;

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

    public bool inMinRange;

    RaycastHit hitInfo;

    private void Awake()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    }

    /// <summary>
    /// initialize the shoot camera with necessary variables
    /// </summary>
    /// <param name="_defaultTarget">The default target point if the hitscan fails</param>
    /// <param name="_layersToIgnore">Layers should the hitscan ignore</param>
    /// <param name="_minRange">The minimum range for thie hitscan to work</param>
    public void Initialize(Transform _defaultTarget, LayerMask _layersToIgnore, float _minRange)
    {
        defaultTarget = _defaultTarget;
        hitLayers = _layersToIgnore;
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
        // Fire hitscan
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, Mathf.Infinity, hitLayers))
        {
            // If distance is below minimum range, fire at default target instead
            if(hitInfo.distance < minRange)
            {
                inMinRange = true;
                targetPos = defaultTarget.position;
            }
            // Otherwise, update target position with hit
            else
            {
                inMinRange = false;
                targetPos = hitInfo.point;
            }
        }
        else
        {
            // If no hit, use default target
            inMinRange = false;
            targetPos = defaultTarget.position;
        }
    }

    /// <summary>
    /// Whether or not a target was hit with the last raycast
    /// </summary>
    /// <returns></returns>
    public bool TargetHit()
    {
        Transform parent = hitInfo.transform;
        Target target;

        if (parent == null)
            return false;

        // continually escelate up for a targetable reference
        while (!parent.TryGetComponent<Target>(out target) && parent.parent != null)
        {
            parent = parent.parent;
        }

        // Check if the target can be damaged
        if (target != null)
        {
            // Damage target, prevent multi damaging
            return true;
        }
        return false;
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
