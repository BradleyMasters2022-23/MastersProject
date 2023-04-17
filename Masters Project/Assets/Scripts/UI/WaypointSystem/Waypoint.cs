/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 16th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - Concrete controller for a UI world waypoint
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
    #region Variables 

    [Header("Setup")]

    /// <summary>
    /// Target transform to point to
    /// </summary>
    [SerializeField] private Transform target;
    /// <summary>
    /// Reference to personal rect transform
    /// </summary>
    private RectTransform t;
    /// <summary>
    /// Main camera reference
    /// </summary>
    private Transform cam;

    [Tooltip("Game object containing every image")]
    [SerializeField] GameObject imageContainer;
    [Tooltip("Transform containing the pointer image")]
    [SerializeField] RectTransform pointerImg;
    [Tooltip("Image holding the icon")]
    [SerializeField] Image iconImage;

    [Tooltip("Additional buffer added to the horizontal bounds")]
    [SerializeField] private float horizontalBuffer;
    [Tooltip("Additional buffer added to the vertical bounds")]
    [SerializeField] private float verticalBuffer;

    [Tooltip("Offset for displaying directly above the target")]
    [SerializeField] private float offset;
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
    /// <summary>
    /// Base scale for the waypoint
    /// </summary>
    private Vector3 baseScale;
    /// <summary>
    /// Whether or not the target is in camera bounds
    /// </summary>
    private bool inBounds;


    [Header("Functionality")]

    [Tooltip("Scale of the tooltip based on player position")]
    [SerializeField] AnimationCurve scaleOverPositionDistance;

    /// <summary>
    /// Max range this waypoint can be viewable at
    /// </summary>
    private float maxRange = 15f;
    /// <summary>
    /// Color thats applied to the waypoint 
    /// </summary>
    private Color displayColor = Color.white;

    #endregion

    #region Pool Functions

    /// <summary>
    /// Initialize. Called by pooler after creation.
    /// </summary>
    public void Init()
    {
        t = GetComponent<RectTransform>();
        baseScale = t.transform.localScale;
        cam = Camera.main.transform;

    }

    /// <summary>
    /// Assign a waypoint. Called by pooler
    /// </summary>
    /// <param name="target">target to point to</param>
    /// <param name="displayColor">Color of object</param>
    /// <param name="maxRange"></param>
    public void AssignTarget(Transform target, WaypointData data)
    {
        this.target = target;
        displayColor = data.displayColor;
        maxRange = data.maxRange;

        // Load in any icon if possible
        if(data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = true;
        }
        // Disable otherwise
        else
        {
            iconImage.enabled = false;
        }

        UpdateColor(displayColor);
    }

    /// <summary>
    /// Reset values
    /// </summary>
    public void ReturnToPool()
    {
        target = null;
        maxRange = 0f;
        t.transform.localScale = baseScale;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    #endregion

    private void Update()
    {
        // If no target, turn off and stop
        if (target == null)
        {
            imageContainer.SetActive(false);
            return;
        }
        
        // apply offset first
        Vector3 tarPos = target.position + target.up * offset;
        Vector3 pos = Camera.main.WorldToScreenPoint(tarPos);
        Vector3 pointDir = (pos - screenCenter);
        inBounds = InBounds(pos);

        #region SCALE and DISTANCE

        // Determine if this should even display based on range
        float positionDist = Vector3.Distance(cam.position, target.position);
        bool show = positionDist <= maxRange;

        // Set view status. If can't show, then dont do anything else to minimize impact
        imageContainer.SetActive(show);
        if (!show) return;

        float distRatio = positionDist / maxRange;

        float totalScale = scaleOverPositionDistance.Evaluate(distRatio);
        transform.localScale = baseScale * totalScale;

        #endregion
        
        #region POSITION
        
        // If behind...
        if (pos.z < 0)
        {
            // Flip it so it displays correctly
            pos *= -1;

            // Due to flip, it always displays to the cam's left. Use this DOT to determine if
            // the position of it is to the left or right of the cam, and lock the appropriate max (more accurate)
            float dot = Vector3.Dot((target.position - cam.position).normalized, cam.right);
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

        // only update rotation if object is out of bounds
        if(!inBounds)
        {
            // recalculate now that its been flipped
            pointDir = (pos - screenCenter);

            // dont bother updating rotation if in bounds, as it wont be shown
            // Calculate rotation with ATAN
            float r = Mathf.Atan2(pointDir.y, pointDir.x) * Mathf.Rad2Deg;
            r -= 90;
            // Convert to full 380 rotation 
            if (r > 180)
            {
                r = 360 - r;
            }

            pointerImg.rotation = Quaternion.Euler(new Vector3(0, 0, r));
        }
        else // if in bounds, just rotate down
        {
            pointerImg.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
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

        // Get screen boundaries with radius
        horBounds.x = t.rect.width / 3 + horizontalBuffer;
        horBounds.y = Screen.width - horBounds.x;

        verBounds.x = t.rect.height / 3 + verticalBuffer;
        verBounds.y = Screen.height - verBounds.x;
    }

    /// <summary>
    /// Determine if a point is within bounds and infront of the cam
    /// </summary>
    /// <param name="pos">point to check</param>
    /// <returns>Whether the point is in camera bounds</returns>
    private bool InBounds(Vector3 pos)
    {
        return (pos.x > horBounds.x && pos.x < horBounds.y
            && pos.y > verBounds.x && pos.y < verBounds.y)
            && pos.z > 0;
    }

    /// <summary>
    /// Set the color of all images in this transform
    /// </summary>
    /// <param name="c">Color to set to</param>
    private void UpdateColor(Color c)
    {
        Image[] images = GetComponentsInChildren<Image>(false);
        foreach(var img in images)
            img.color= c;
    }
}
