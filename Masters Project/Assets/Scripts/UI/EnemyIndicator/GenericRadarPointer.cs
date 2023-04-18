/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - April 17th, 2023
 * Last Edited - April 17th, 2023 by Ben Schuster
 * Description - A generic pointer for the radar to point to any transform
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericRadarPointer : MonoBehaviour
{
    [SerializeField] private GameObject indicatorGraphic;
    [SerializeField] private GameObject maxDistGraphic;

    [Tooltip("Additional offset for radius. Useful for getting perfect bounds.")]
    [SerializeField] private float radiusMod;

    /// <summary>
    /// Reference to internal transform
    /// </summary>
    private RectTransform t;
    /// <summary>
    /// Target to represent on radar
    /// </summary>
    private Transform target;
    /// <summary>
    /// Reference to player transform
    /// </summary>
    private Transform player;
    /// <summary>
    /// Rect of the total indicator. Used for maths
    /// </summary>
    private RectTransform indicatorRect;
    /// <summary>
    /// Rect of the radar. Used for maths
    /// </summary>
    private RectTransform maxDistRect;

    private Color displayColor;

    /// <summary>
    /// The max distance of the radar. Automatically calculated
    /// </summary>
    private float maxDistance;
    /// <summary>
    /// The radius of the radar, automatically calculated
    /// </summary>
    private float radarRadius;
    
    /// <summary>
    /// Whether or not this pointer has been initialized
    /// </summary>
    private bool initialized;
    /// <summary>
    /// Used for distance tracking and calculations
    /// </summary>
    private Vector2 distRect;

    public delegate void ReturnDelegate(GenericRadarPointer p);

    public ReturnDelegate noTargetFoundFunc;

    #region Tracking Funcs

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
            return;

        // If the target to monitor is gone dont do anything.
        // The apprropriate generic radar should take it back
        if (!TargetActive())
        {
            noTargetFoundFunc.Invoke(this);
            return;
        }

        // Rotate to target on radar
        RotateToTargetFlat();
        // Check distance and monitor any distance-based images
        CheckDistance();
    }

    private void CheckDistance()
    {
        float currDist = Mathf.Abs(Vector3.Distance(player.position, target.position));

        float newPix = Mathf.Clamp01((currDist / maxDistance)) * radarRadius;
        distRect.y = newPix;

        indicatorRect.anchoredPosition = distRect;
        maxDistRect.anchoredPosition = distRect;


        if (currDist >= maxDistance && !maxDistGraphic.activeSelf)
        {
            //Debug.Log("Toggling to max range arrow");
            maxDistGraphic.SetActive(true);
            indicatorGraphic.SetActive(false);
        }
        else if (currDist < maxDistance && !indicatorGraphic.activeSelf)
        {
            //Debug.Log("Toggling to normal dot");
            indicatorGraphic.SetActive(true);
            maxDistGraphic.SetActive(false);
        }
    }

    private void RotateToTargetFlat()
    {
        Vector3 direction = target.position - player.position;

        Quaternion r = Quaternion.LookRotation(direction);
        r.z = -r.y;
        r.x = 0;
        r.y = 0;

        Vector3 north = new Vector3(0, 0, player.eulerAngles.y);
        t.rotation = r * Quaternion.Euler(north);
    }

    #endregion

    #region Pool Stuff

    public void Init(ReturnDelegate returnFunc)
    {
        initialized = false;
        noTargetFoundFunc = returnFunc;
        player = Camera.main.transform;
        t = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Set the target enemy
    /// </summary>
    /// <param name="target">target to point to</param>
    public virtual void SetTarget(Transform _target, Color color, float rad = 0, float maxDist = 0)
    {
        target = _target;
        displayColor = color;

        // apply new color to images
        UpdateColor();

        // calculate distances and radiuses
        radarRadius = rad - radiusMod;
        maxDistance = maxDist;

        // Get internal references 
        if (indicatorGraphic != null)
            indicatorRect = indicatorGraphic.GetComponent<RectTransform>();
        if (maxDistGraphic != null)
            maxDistRect = maxDistGraphic.GetComponent<RectTransform>();

        distRect = indicatorRect.anchoredPosition;
        initialized = true;
    }

    public void Return()
    {
        initialized = false;
        target = null;
        gameObject.SetActive(false);
    }

    private void UpdateColor()
    {
        Image[] t = GetComponentsInChildren<Image>(true);
        foreach (var img in t)
            img.color = displayColor;
    }

    public bool TargetActive()
    {
        return (target != null && target.gameObject.activeInHierarchy);
    }

    #endregion
}
