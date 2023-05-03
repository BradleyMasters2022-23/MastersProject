using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIPointer : MonoBehaviour
{
    private Transform cam;
    private Image image;
    private Vector3 initScale;
    
    /// <summary>
    /// Initialize references
    /// </summary>
    private void Awake()
    {
        cam = Camera.main.transform;
        image = GetComponentInChildren<Image>();
        initScale = image.transform.localScale;
    }

    /// <summary>
    /// Apply new color to this object
    /// </summary>
    /// <param name="c"></param>
    public void UpdateColor(Color c)
    {
        if(image != null)
            image.color = c;
    }

    /// <summary>
    /// Update the rotation to point towards the target
    /// </summary>
    /// <param name="target">Target to point to</param>
    public void UpdateRotation(Transform target)
    {
        // determine rotation based on the bird-eye view rotation
        Vector3 direction = target.position - cam.position;
        Quaternion r = Quaternion.LookRotation(direction);
        r.z = -r.y;
        r.x = 0;
        r.y = 0;
        Vector3 north = new Vector3(0, 0, cam.eulerAngles.y);
        transform.rotation = r * Quaternion.Euler(north);
    }

    /// <summary>
    /// Update the scale of this pointer
    /// </summary>
    /// <param name="s">Scale modifier to apply</param>
    public void UpdateScale(float s)
    {
        image.transform.localScale = initScale * s;
    }

}
