using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AlertContainer : MonoBehaviour
{
    [Tooltip("Pointer to utilize")]
    [SerializeField] private AlertUIPointer pointerPrefab;

    [Space(5)]
    [Tooltip("Hide the alert when the source is directly in camera view")]
    [SerializeField] bool hideWhenInView;

    [Tooltip("Hide the alert when theres no line of sight from ANY direction")]
    [SerializeField] bool hideNoLineOfSight;
    [HideIf("@this.hideNoLineOfSight == false")]
    [Tooltip("Terrain layers that affect line of sight")]
    [SerializeField] LayerMask lineOfSightLayers;

    [Tooltip("Whether the alert has any functions based on distance")]
    [SerializeField] bool distanceFunctionality;
    [HideIf("@this.distanceFunctionality == false")]
    [Tooltip("The minimum and maximum range this alert displays in")]
    [SerializeField] Vector2 displayDistanceRange = new Vector2(0, 999);
    [HideIf("@this.distanceFunctionality == false")]
    [Tooltip("The graph determining scale over distance")]
    [SerializeField] AnimationCurve scaleOverDistance;
    [HideIf("@this.distanceFunctionality == false")]
    [Tooltip("Graph determining color over distance")]
    [SerializeField] Gradient colorOverDistance;

    [HideIf("@this.distanceFunctionality == true")]
    [Tooltip("Color to display")]
    [SerializeField] private Color displayColor = Color.white;
    [HideIf("@this.distanceFunctionality == true")]
    [Tooltip("Scale modifier to apply to the pointer")]
    [SerializeField] private float scaleMod = 1;

    /// <summary>
    /// Reference to personal pointer
    /// </summary>
    AlertUIPointer pointer;
    /// <summary>
    /// Reference to player
    /// </summary>
    private Transform player;
    /// <summary>
    /// Camera
    /// </summary>
    private CameraShoot cam;

    /// <summary>
    /// Get all references needed
    /// </summary>
    private void OnEnable()
    {
        // set target to main camera 
        player = Camera.main.transform;
        cam = player.GetComponent<CameraShoot>();

        // Spawn and use pointer
        pointer = Instantiate(pointerPrefab.gameObject, AlertUIManager.display).GetComponent<AlertUIPointer>();
        pointer.UpdateColor(GetColor());
    }

    /// <summary>
    /// Destroy pointer and set to null
    /// </summary>
    private void OnDisable()
    {
        if(pointer != null)
            Destroy(pointer.gameObject);
        pointer = null;
    }

    private void Update()
    {
        // Set display status. If not active, then exit early
        pointer.gameObject.SetActive(Display());
        if (!pointer.isActiveAndEnabled) return;

        // Update rotation
        pointer.UpdateRotation(transform);

        // Update scale
        if(distanceFunctionality)
        {
            pointer.UpdateScale(GetScale());
            pointer.UpdateColor(GetColor());
        }
    }

    /// <summary>
    /// Check whether this alert can display. Go through all conditions, cheapest first
    /// </summary>
    /// <returns>Whether this alert should display</returns>
    public virtual bool Display()
    {
        // Check if within distance min/max range
        if(distanceFunctionality)
        {
            // Check if within distance range
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist < displayDistanceRange.x || dist > displayDistanceRange.y)
                return false;
        }

        // Check if current in view of the camera
        if(hideWhenInView)
        {
            if(cam.InCamVision(transform.position))
            {
                return false;
            }
        }

        // Check if theres any line of sight from any direction
        if(hideNoLineOfSight)
        {
            RaycastHit h;
            if(Physics.Raycast(transform.position, (player.position - transform.position), out h, Mathf.Infinity, lineOfSightLayers))
            { 
                // If it didn't hit the player, then sometimes is blocking line of sight
                if(!h.collider.CompareTag("Player"))
                    return false;
            }
        }
         
        // If all conditions passed, return true
        return true;
    }

    /// <summary>
    /// Get the scale modifier for the pointer
    /// </summary>
    /// <returns>Scale modifier to use</returns>
    public float GetScale()
    {
        if(distanceFunctionality)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            float distRatio = (dist - displayDistanceRange.x) / (displayDistanceRange.y - displayDistanceRange.x);
            return scaleOverDistance.Evaluate(distRatio);
        }
        else
        {
            return scaleMod;
        }
    }

    /// <summary>
    /// Get the color for the pointer
    /// </summary>
    /// <returns>New color to use</returns>
    public Color GetColor()
    {
        if (distanceFunctionality)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            float distRatio = (dist - displayDistanceRange.x) / (displayDistanceRange.y - displayDistanceRange.x);
            return colorOverDistance.Evaluate(distRatio);
        }
        else
        {
            return displayColor;
        }
    }
}
