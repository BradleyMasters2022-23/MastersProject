using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class PropScrollArea : MonoBehaviour, IDragHandler, IScrollHandler
{
    [Header("Setup")]

    [Tooltip("Prop container to rotate")]
    [SerializeField] Transform prop;
    [Tooltip("Camera that renders this UI")]
    [SerializeField] Camera renderCam;

    [Tooltip("Curve handling scale on open. Must end in 1.")]
    [SerializeField] AnimationCurve spawnScaleAnimation;

    [Tooltip("Whether or not the player can rotate the prop")]
    private bool rotate = true;
    [Tooltip("Whether or not the player can scale the prop")]
    private bool scale = true;

    [Header("Interact Tooltip")]

    [Tooltip("Textbox displaying interact tooltip")]
    [SerializeField] private TextMeshProUGUI tooltipText;
    /// <summary>
    /// Whether or not the tooltip is being displayed
    /// </summary>
    private bool showingTooltip;
    /// <summary>
    /// Whether the scroll area has been interacted with
    /// </summary>
    private bool interactedWith;
    /// <summary>
    /// Time since it was interacted with
    /// </summary>
    private float timeSinceInteract;
    /// <summary>
    /// Time threshold before showing the tooltip
    /// </summary>
    private float tooltipTimeThreshold = 6.5f;

    #region Setup

    /// <summary>
    /// on start, get original rotation
    /// </summary>
    private void Awake()
    {
        originalScale = prop.localScale.x;

        originalPitchJointRot = pitchJoint.rotation;
        originalYawJointRot = yawJoint.rotation;
        currentPitch = pitchJoint.eulerAngles.x;
        if (currentPitch >= 180)
            currentPitch -= 360;

        tooltipText.CrossFadeAlpha(0, 0f, true);
    }

    /// <summary>
    /// call to do the open animation
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(OpenZoom());
    }

    /// <summary>
    /// make sure its returned ro original rotation for next open
    /// </summary>
    private void OnDisable()
    {
        pitchJoint.rotation = originalPitchJointRot;
        yawJoint.rotation = originalYawJointRot;
        currentPitch = pitchJoint.eulerAngles.x;
        if (currentPitch >= 180)
            currentPitch -= 360;

        StopAllCoroutines();

        prop.localScale = Vector3.one * originalScale;
        front = true;
        rotate = true;
        scale = true;

        interactedWith = false;
        showingTooltip = false;
        timeSinceInteract = 0;
        tooltipText.CrossFadeAlpha(0, 0f, true);
    }

    /// <summary>
    /// Scale in the prob over some time
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenZoom()
    {
        // disable controls temporarily
        rotate = false;
        scale = false;

        // set targets and starting point
        float tgtTime = 0.4f;
        float tgtScale = prop.localScale.x;
        prop.localScale = Vector3.one * spawnScaleAnimation.Evaluate(0);

        // lerp through with animation curve
        float t = 0;
        while (t <= tgtTime)
        {
            t += Time.unscaledDeltaTime;
            prop.localScale = Vector3.one * tgtScale * spawnScaleAnimation.Evaluate(t / tgtTime);
            yield return null;
        }

        // finalize goal, reenable controls
        prop.localScale = Vector3.one * tgtScale;
        rotate = true;
        scale = true;
    }

    private void Update()
    {
        if(!interactedWith && !showingTooltip)
        {
            // if threshold not met, just track time
            if(timeSinceInteract <= tooltipTimeThreshold)
            {
                timeSinceInteract += Time.unscaledDeltaTime;
            }
            // otherwise, crossfade if not done yet
            else if(!showingTooltip)
            {
                showingTooltip = true;
                tooltipText.CrossFadeAlpha(1, 0.4f, true);
            }
        }
    }

    #endregion

    #region Drag and Rotate

    [Header("Rotation")]

    [Tooltip("Min and max angles for the pitch")]
    [SerializeField] Vector2 pitchAngleRange;
    [Tooltip("Drag sensitivity")]
    [SerializeField] float dragSensitivity;
    
    [Tooltip("Joint controlling the pitch (vertical rotation) of the prop")]
    [SerializeField] Transform pitchJoint;
    [Tooltip("Joint controlling the yaw (horizontal rotation) of the prop")]
    [SerializeField] Transform yawJoint;
    /// <summary>
    /// The current pitch of the joint
    /// </summary>
    private float currentPitch;
    /// <summary>
    /// Whether the prop is currently facing the front
    /// </summary>
    private bool front;
    /// <summary>
    /// Events to call when switching between the front and back
    /// </summary>
    private UnityEvent<bool> onFlipEvent;

    /// <summary>
    /// original rotation of pitch joint. Used to reset UI
    /// </summary>
    private Quaternion originalPitchJointRot;
    /// <summary>
    /// original rotation of yaw joint. Used to reset UI
    /// </summary>
    private Quaternion originalYawJointRot;

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
    /// on drag, rotate the prop with parameters in mind
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(!rotate) return;

        interactedWith = true;
        if (showingTooltip)
        {
            showingTooltip= false;
            tooltipText.CrossFadeAlpha(0, 0.4f, true);
        }
            

        // prepare rotational values
        float yawRot = -eventData.delta.x * dragSensitivity * Time.unscaledDeltaTime;
        float pitchRot = eventData.delta.y * dragSensitivity * Time.unscaledDeltaTime;

        // apply yaw 
        yawJoint.Rotate(yawJoint.up, yawRot, Space.World);

        // adjust pitch so it remainds in range
        if (currentPitch + pitchRot < pitchAngleRange.x)
        {
            pitchRot = pitchAngleRange.x - currentPitch;
            currentPitch = pitchAngleRange.x;
        }
        else if (currentPitch + pitchRot > pitchAngleRange.y)
        {
            pitchRot = pitchAngleRange.y - currentPitch;
            currentPitch = pitchAngleRange.y;
        }
        else
        {
            currentPitch += pitchRot;
        }

        // apply clamped pitch
        pitchJoint.Rotate(pitchJoint.right, pitchRot, Space.World);

        // trigger flip event if active
        bool newFront = Vector3.Dot(prop.forward, renderCam.transform.forward) <= 0;
        if (front != newFront)
        {
            onFlipEvent?.Invoke(newFront);
            front = newFront;
        }
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

        interactedWith = true;
        if (showingTooltip)
        {
            showingTooltip = false;
            tooltipText.CrossFadeAlpha(0, 0.4f, true);
        }

        float currScale = prop.localScale.x / originalScale;
        currScale += (eventData.scrollDelta.y * -1 * scrollSensitivity * Time.unscaledDeltaTime);
        currScale = Mathf.Clamp(currScale, scaleRange.x, scaleRange.y);
        prop.localScale = Vector3.one * currScale * originalScale;
    }

    #endregion
}
