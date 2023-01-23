/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - December 6th, 2022
 * Last Edited - December 6th, 2022 by Ben Schuster
 * Description - Library of functions commonly used in AI/Enemy programming
 * ================================================================================================
 */
using UnityEngine;

namespace Masters.AI
{
    public static class EnemyFuncs
    {
        #region Utility

        /// <summary>
        /// Determines if the source has clear line of sight of the target
        /// </summary>
        /// <param name="source">source of the line of sight</param>
        /// <param name="target">target line of sight</param>
        /// <param name="visionLayer">what physics layers are included in this check</param>
        /// <param name="width">width of the raycast</param>
        /// <returns>Whether the source has line of sight on target</returns>
        public static bool HasLineOfSight(this Transform source, Transform target, LayerMask visionLayer, float width = 0)
        {
            // determine direction to aim raycast
            Vector3 dir = (target.position + Vector3.up) - (source.position + Vector3.up);
            Ray rayData = new Ray(source.position + Vector3.up, dir);

            RaycastHit hit;
            bool landed;

            // Debug.DrawRay(source.position + Vector3.up, dir, Color.red);

            // if no width, use raycast
            if(width <= 0)
            {
                landed = Physics.Raycast(rayData, out hit, Mathf.Infinity, visionLayer);

            }
            // otherwise, use spherecast
            else
            {
                landed = Physics.SphereCast(rayData, width, out hit, Mathf.Infinity, visionLayer);
            }

            // Determine if it landed on the target
            if(landed)
            {
                //Debug.Log($"Landed! hit {hit.collider.name} with tag {hit.collider.tag} vs {target.tag}");
                return hit.collider.tag == target.tag;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Determines if the target is within a frontal cone with a look radius. Horizontal only
        /// </summary>
        /// <param name="source">Source to originate the view cone</param>
        /// <param name="target">Target to check if within vision cone</param>
        /// <param name="lookRadius">Radius of vision cone</param>
        /// <returns>Whether the target is within the source's frontal vision cone</returns>
        public static bool InVisionCone(this Transform source, Transform target, float lookRadius)
        {
            Vector3 targetPos = new Vector3(target.position.x, source.position.y, target.position.z);
            Vector3 temp = (targetPos - source.position).normalized;
            float angle = Vector3.SignedAngle(temp, source.forward, Vector3.up);
            return (Mathf.Abs(angle) <= lookRadius);
        }

        #endregion

        #region Movement | Rotation

        /// <summary>
        /// Rotate towards the target given rotation speed. Call this in update.
        /// </summary>
        /// <param name="source">Transform calling this function to be rotated</param>
        /// <param name="target">Target transform to rotate towards</param>
        /// <param name="rotationSpeed">rotation speed</param>
        public static void RotateToInUpdate(this Transform source, Transform target, float rotationSpeed)
        {
            source.RotateToInUpdate((target.position - source.position).normalized, rotationSpeed);
        }

        /// <summary>
        /// Rotate towards the target given rotation speed. Call this in update.
        /// </summary>
        /// <param name="source">Transform calling this function to be rotated</param>
        /// <param name="direction">Direction to look towards</param>
        /// <param name="rotationSpeed">rotation speed</param>
        public static void RotateToInUpdate(this Transform source, Vector3 direction, float rotationSpeed)
        {
            // temp, adjust this to be better later
            if (direction == Vector3.zero)
                return;

            // Get target rotation
            Quaternion rot = Quaternion.LookRotation(direction);
            rot = Quaternion.Euler(0, rot.eulerAngles.y, 0);

            // rotate towards the target, limited by rotation
            float deltaAngle = Mathf.DeltaAngle(source.rotation.eulerAngles.y, rot.eulerAngles.y);
            float clampedAngle = Mathf.Clamp(deltaAngle, -rotationSpeed, rotationSpeed);

            // Check if its finished. used because of some of the rotation scaling wont match when done
            if (clampedAngle == 0)
            {
                return;
            }

            // Adjust angle for timestop, apply
            float nextYAng = clampedAngle * TimeManager.WorldTimeScale;
            source.rotation = Quaternion.Euler(0, (source.rotation.eulerAngles.y + nextYAng) % 360, 0);
        }

        #endregion

    }
}
