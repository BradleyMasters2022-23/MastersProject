using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
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

    [SerializeField] GameObject imageContainer;
    [SerializeField] RectTransform pointerImg;
    [SerializeField] Image iconImage;

    [Header("Functionality")]

    [Tooltip("Max range for this tooltip can appear as. Overwritten by others.")]
    [SerializeField] float maxRange;
    [Tooltip("Max range for this tooltip can appear as")]
    [SerializeField] AnimationCurve scaleOverPositionDistance;
    [Tooltip("Color of the tooltip")]
    [SerializeField] Color displayColor;
    
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


    [SerializeField] private float horizontalBuffer;
    [SerializeField] private float verticalBuffer;
    private void Awake()
    {
        t = GetComponent<RectTransform>();

        baseScale = t.transform.localScale;
        cam = Camera.main.transform;


        // TESTING
        AssignTarget(target, displayColor, maxRange, null);
    } 

    /// <summary>
    /// Assign a waypoint
    /// </summary>
    /// <param name="target">target to point to</param>
    /// <param name="displayColor">Color of object</param>
    /// <param name="maxRange"></param>
    public void AssignTarget(Transform target, Color displayColor, float maxRange, Sprite iconSprite)
    {
        this.target = target;
        this.displayColor = displayColor;
        this.maxRange = maxRange;

        // Load in any icon if possible
        if(iconSprite != null)
        {
            iconImage.sprite = iconSprite;
            iconImage.enabled = true;
        }
        // Disable otherwise
        else
        {
            iconImage.enabled = false;
        }

        UpdateColor(displayColor);
    }

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(target.position);
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

        pointerImg.gameObject.SetActive(!inBounds);

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
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach(var img in images)
            img.color= c;
    }
}
