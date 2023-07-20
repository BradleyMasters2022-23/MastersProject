using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PropScrollArea : MonoBehaviour, IDragHandler, IBeginDragHandler, IScrollHandler
{
    [Header("Setup")]

    [Tooltip("Prop container to rotate")]
    [SerializeField] Transform prop;
    [Tooltip("Camera that renders this UI")]
    [SerializeField] Camera renderCam;

    [SerializeField] bool rotate;
    [SerializeField] bool scale;

    #region Setup

    /// <summary>
    /// on start, get original rotation
    /// </summary>
    private void Awake()
    {
        originalPropRot = prop.rotation;
        originalScale = prop.localScale.x;
    }
    /// <summary>
    /// make sure its returned ro original rotation for next open
    /// </summary>
    private void OnDisable()
    {
        prop.rotation = originalPropRot;
        prop.localScale = Vector3.one * originalScale;
        front = true;
    }

    #endregion

    #region Drag and Rotate

    [Header("Rotation")]

    [Tooltip("Min and max angles for the pitch")]
    [SerializeField] Vector2 pitchAngleRange;
    [Tooltip("Drag sensitivity")]
    [SerializeField] float dragSensitivity;
    private bool front;
    private  UnityEvent<bool> onFlipEvent;

    public void SubscribeToFlip(UnityAction<bool> act)
    {
        if(onFlipEvent == null)
            onFlipEvent= new UnityEvent<bool>();

        onFlipEvent.AddListener(act);
    }

    public void UnSubscribeToFlip(UnityAction<bool> act)
    {
        if(onFlipEvent != null)
            onFlipEvent.RemoveListener(act);
    }

    /// <summary>
    /// original rotation of the prop
    /// </summary>
    private Quaternion originalPropRot;
    /// <summary>
    /// Inversion value for pitch
    /// </summary>
    private float invertVerticalDrag;

    /// <summary>
    /// on drag, rotate the prop with parameters in mind
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(!rotate) return;

        // get rotation based on delta
        Quaternion tempRot =
            Quaternion.AngleAxis(-eventData.delta.x * dragSensitivity * Time.unscaledDeltaTime, Vector3.up) * 
            Quaternion.AngleAxis(-eventData.delta.y * invertVerticalDrag * dragSensitivity * Time.unscaledDeltaTime, Vector3.left);
        
        // apply todation
        prop.rotation *= tempRot;

        // clamp pitch rotation, prevent roll
        Vector3 eulerRot = prop.rotation.eulerAngles;
        if (eulerRot.x > 180)
            eulerRot.x -= 360;
        eulerRot.x = Mathf.Clamp(eulerRot.x, pitchAngleRange.x, pitchAngleRange.y);
        eulerRot.z = 0;
        prop.rotation = Quaternion.Euler(eulerRot);

        // trigger flip event if active
        bool newFront = Vector3.Dot(prop.forward, renderCam.transform.forward) < 0;
        if (front != newFront)
        {
            onFlipEvent?.Invoke(newFront);
            front = newFront;
        }
    }


    /// <summary>
    /// Decide how to invert the vertical rotation
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!rotate) return;

        invertVerticalDrag = (Vector3.Dot(prop.forward, renderCam.transform.forward) < 0) ? -1 : 1;
    }

    #endregion

    #region Scroll and Scale
    
    [Header("Scale")]

    [Tooltip("Min and max scale for the prop")]
    [SerializeField] Vector2 scaleRange;
    [SerializeField] float scrollSensitivity;

    /// <summary>
    /// Original scale of the prop
    /// </summary>
    private float originalScale;

    /// <summary>
    /// Modify scale on scroll
    /// </summary>
    /// <param name="eventData"></param>
    public void OnScroll(PointerEventData eventData)
    {
        if(!scale) return;

        float currScale = prop.localScale.x / originalScale;
        currScale += (eventData.scrollDelta.y * -1 * scrollSensitivity * Time.unscaledDeltaTime);
        currScale = Mathf.Clamp(currScale, scaleRange.x, scaleRange.y);
        prop.localScale = Vector3.one * currScale * originalScale;
    }

    #endregion
}
