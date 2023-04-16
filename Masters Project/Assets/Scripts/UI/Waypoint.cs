using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    [SerializeField] Transform pointToTarget;

    [SerializeField] Image t;

    [SerializeField] Transform player;

    [SerializeField] RectTransform pointerImg;

    [SerializeField] AnimationCurve scaleOverPositionDistance;

    [SerializeField] GameObject imageContainers;

    [SerializeField] float maxRange;

    /// <summary>
    /// Reference to center of the screen
    /// </summary>
    private Vector3 screenCenter;
    /// <summary>
    /// Horizontal bounds reference, radius included
    /// </summary>
    private Vector2 horBounds;
    /// <summary>
    /// Vertical bounds reference, radius included
    /// </summary>
    private Vector2 verBounds;

    private Vector3 baseScale;

    private bool inBounds;

    private void Awake()
    {
        baseScale = t.transform.localScale;
        player = Camera.main.transform;
        UpdateAllBounds();
    } 

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(pointToTarget.position, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 pointDir = (pos - screenCenter);
        inBounds = InBounds(pos);

        /*
        #region SCALE and DISTANCE

        // Determine if this should even display based on range
        float positionDist = Vector3.Distance(player.position, pointToTarget.position);
        bool show = positionDist <= maxRange;

        // Set view status. If can't show, then dont do anything else to minimize impact
        imageContainers.SetActive(show);
        if (!show) return;

        float distRatio = positionDist / maxRange;

        float totalScale = scaleOverPositionDistance.Evaluate(distRatio);
        transform.localScale = baseScale * totalScale;

        #endregion
        */

        #region POSITION

        // If behind...
        if (pos.z < 0)
        {
            // Flip it so it displays correctly
            pos *= -1;

            // Due to flip, it always displays to the player's left. Use this DOT to determine if
            // the position of it is to the left or right of the player, and lock the appropriate max (more accurate)
            float dot = Vector3.Dot((pointToTarget.position - player.position).normalized, player.right);
            if(dot > 0)
            {
                pos.x = horBounds.y;
            }
            else
            {
                pos.x = horBounds.x;
            }
        }

        // Clamp by the bounds, apply position
        pos.x = Mathf.Clamp(pos.x, horBounds.x, horBounds.y);
        pos.y = Mathf.Clamp(pos.y, verBounds.x, verBounds.y);
        t.transform.position = pos;

        #endregion

        #region ROTATION

        pointerImg.gameObject.SetActive(!inBounds);

        // dont bother updating rotation if in bounds, as it wont be shown
        if(!inBounds)
        {
            // Calculate rotation with ATAN
            float r = Mathf.Atan2(pointDir.y, pointDir.x) * Mathf.Rad2Deg;
            r -= 90;
            // Convert to full 380 rotation 
            if (r > 180)
            {
                r = 360 - r;
            }

            // apply rotation
            pointerImg.rotation = Quaternion.Euler(new Vector3(0, 0, r));
        }

        #endregion

        // update bounds for next frame
        UpdateAllBounds();
    }

    /// <summary>
    /// Update the bounds for appropriate scaling
    /// Function just incase the screen size changes post awake
    /// </summary>
    private void UpdateAllBounds()
    {
        // Get screen center
        screenCenter.x = Screen.width;
        screenCenter.y = Screen.height;
        screenCenter /= 2;

        // Get screen boundaries
        horBounds.x = t.GetPixelAdjustedRect().width / 2;
        horBounds.y = Screen.width - horBounds.x;

        verBounds.x = t.GetPixelAdjustedRect().height / 2;
        verBounds.y = Screen.height - verBounds.x;

        // Get radius of waypoint

    }

    /// <summary>
    /// Determine if a point is within bounds and infront of the player
    /// </summary>
    /// <param name="pos">point to check</param>
    /// <returns>Whether the point is in camera bounds</returns>
    private bool InBounds(Vector3 pos)
    {
        return (pos.x > horBounds.x && pos.x < horBounds.y
            && pos.y > verBounds.x && pos.y < verBounds.y)
            && pos.z > 0;
    }
}
