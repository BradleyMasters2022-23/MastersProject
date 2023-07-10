/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 24th, 2022 by Ben Schuster
 * Description - Manage the camera aim for the player
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [Header("---Camera---")]

    [Tooltip("Channel to check for settings change")]
    [SerializeField] private ChannelVoid onSettingsChangedChannel;
    [SerializeField] private ChannelVoid resetPlayerLook;

    private float mouseSensitivity;
    private bool mouseXInverted;
    private bool mouseYInverted;
    
    private float controllerSensitivity;
    private bool controllerXInverted;
    private bool controllerYInverted;
    
    [Tooltip("The minimum and maximum rotation for the camera's vertical rotation.")]
    [SerializeField] private Vector2 angleClamp;
    [Tooltip("Primary point the camera looks at and pivots from. Should be around the player's shoulders.")]
    [SerializeField] private Transform cameraLook;

    [Header("---Game flow---")]
    [Tooltip("Channel that watches the game manager states")]
    [SerializeField] private ChannelGMStates onStateChangedChannel;
    [SerializeField] private ChannelVoid onSceneChangeChannel;

    [SerializeField] private ChannelControlScheme onControllerSwap;

    /// <summary>
    /// Core controller map
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Action input for aiming the camera [mouse delta / joystick delta]
    /// </summary>
    private InputAction aim;

    [Header("---Aim Assist---")]
    [Tooltip("Target layers that can be aimed towards")]
    [SerializeField] LayerMask targetLayers;
    [Tooltip("Layers that block line of sight. Other targets should be included")]
    [SerializeField] LayerMask blockLayers;
    [Tooltip("The radius around the forward direction to find enemies")]
    [SerializeField] float assistRadius;
    [Tooltip("The max distance the assist can target")]
    [SerializeField] float assistDistance;
    [Tooltip("The default strength of the aim assist")]
    [SerializeField] float assistStrength;
    [Tooltip("The cone threshold needed to assist. Higher values means they need to be more centered.")]
    [SerializeField, Range(0, 1)] float assistConeThreshold;
    [Tooltip("The magnitude of player input and an assist strength modifier. Value is multiplied with assist strength")]
    [SerializeField] AnimationCurve inputMagnitudeToAssist;
    [Tooltip("The accuracy of the current target and an assist strength modifier. " +
        "Value is a % based on its DOT accuracy rating and the assist cone threshold. " +
        "The value is multiplied with assist strength.")]
    [SerializeField] AnimationCurve accuracyToAssist;
    [Tooltip("Additional modifier applied (multiplicatively) to horizontal aim assist")]
    [SerializeField] float horizontalAssistModifier = 1;
    [Tooltip("Additional modifier applied (multiplicatively) to vertical aim assist")]
    [SerializeField] float verticalAsssitModifier = 1;

    /// <summary>
    /// The center of the target that aim assist is looking at. Null if no target. 
    /// </summary>
    private Transform targetCenter;
    /// <summary>
    /// The DOT accuracy towards the current target
    /// </summary>
    private float targetDot;

    private void Awake()
    {
        // Initialize aim controls
        StartCoroutine(InitializeControls());
        aimAssistOptions = new List<Target>();

        // Load in settings
        UpdateSettings();
    }

    private IEnumerator InitializeControls()
    {
        while (GameManager.controls == null)
            yield return null;

        controller = GameManager.controls;

        aim = controller.PlayerGameplay.Aim;
        aim.Enable();

        yield return null;
    }

    private void UpdateSettings()
    {
        mouseSensitivity = Settings.mouseSensitivity;
        mouseXInverted = Settings.mouseInvertX;
        mouseYInverted = Settings.mouseInvertY;

        controllerSensitivity = Settings.controllerSensitivity;
        controllerXInverted = Settings.controllerInvertX;
        controllerYInverted = Settings.controllerInvertY;

        //Debug.Log("Aim sensitivity set to : " + mouseSensitivity);
        UpdateInputSettings(InputManager.CurrControlScheme);
    }

    private void LateUpdate()
    {
        // Get new delta based on update
        SearchForTarget();
        ManageCamera();
    }

    // Figure out which control input to use, adjust sensitivity values
    Vector2 lookDelta;
    float sensitivity;
    float horizontalInversion = 1;
    float verticalInversion = 1;

    private void Update()
    {
        if (aim == null)
            return;

        lookDelta = Vector2.zero;

        if (aim.ReadValue<Vector2>() != Vector2.zero)
        {
            lookDelta = aim.ReadValue<Vector2>();
        }
    }

    /// <summary>
    /// update sensitivity and inversion settings based on the input method
    /// </summary>
    /// <param name="controlScheme">Type of input being swapped to</param>
    private void UpdateInputSettings(InputManager.ControlScheme controlScheme)
    {
        switch(controlScheme)
        {
            case InputManager.ControlScheme.KEYBOARD:
                {
                    sensitivity = mouseSensitivity;

                    if (mouseXInverted)
                        horizontalInversion *= -1;
                    if (mouseYInverted)
                        verticalInversion *= -1;

                    break;
                }
            case InputManager.ControlScheme.CONTROLLER:
                {
                    sensitivity = controllerSensitivity;

                    if (controllerXInverted)
                        horizontalInversion *= -1;
                    if (controllerYInverted)
                        verticalInversion *= -1;

                    break;
                }
            default: // by default, use M&K settings
                {
                    sensitivity = mouseSensitivity;

                    if (mouseXInverted)
                        horizontalInversion *= -1;
                    if (mouseYInverted)
                        verticalInversion *= -1;
                    break;
                }
        }
    }

    /// <summary>
    /// Update the camera's rotation
    /// </summary>
    private void ManageCamera()
    {
        // Manage horizontal rotation
        transform.rotation *= Quaternion.AngleAxis(lookDelta.x * sensitivity * horizontalInversion * Time.deltaTime, Vector3.up);
        
        // Manage vertical rotaiton
        Quaternion newVerticalRot = cameraLook.transform.localRotation *
            Quaternion.AngleAxis(-lookDelta.y * sensitivity * verticalInversion * Time.deltaTime, Vector3.right);

        //Debug.Log("Delta detected : " + lookDelta.magnitude);
        float assistMag = inputMagnitudeToAssist.Evaluate(lookDelta.magnitude);
        lookDeltaMag = lookDelta.magnitude;
        // calculate aim assist rotations, apply
        if (targetCenter != null && assistMag > 0)
        {
            // calculate the mag modifier based on the % of the dot, getting stronger the more center it is
            float distBasedMag = 1 - assistConeThreshold;
            distBasedMag = (targetDot - assistConeThreshold) / distBasedMag;
            distBasedMag = accuracyToAssist.Evaluate(distBasedMag);
            assistMag *= distBasedMag;

            // calculate the target rotation
            Quaternion tgtRot = Quaternion.LookRotation(targetCenter.position - cameraLook.position);
            //Debug.DrawLine(cameraLook.position, cameraLook.position + (targetCenter.position - cameraLook.position), Color.green);

            // make sure its only applying horizontal rotation
            Vector3 tempEuler = Quaternion.Slerp(transform.rotation, tgtRot, assistStrength * assistMag * horizontalAssistModifier * Time.deltaTime).eulerAngles;
            tempEuler.x = 0;
            tempEuler.z = 0;
            transform.rotation = Quaternion.Euler(tempEuler);

            // do same for vertical rotation using the correct pivot
            tempEuler = Quaternion.Slerp(newVerticalRot, tgtRot, assistStrength * assistMag * verticalAsssitModifier * Time.deltaTime).eulerAngles;
            tempEuler.y = 0;
            tempEuler.z = 0;
            newVerticalRot = Quaternion.Euler(tempEuler);
        }

        // Make sure to clamp vertical angle first
        float newYAngle = newVerticalRot.eulerAngles.x;
        if (newYAngle > 180)
        {
            newYAngle -= 360;
        }
        float clampedAngle = Mathf.Clamp(newYAngle, angleClamp.x, angleClamp.y);
        cameraLook.transform.localRotation = Quaternion.Euler(clampedAngle, 0, 0);
    }

    /// <summary>
    /// Subscribe channels 
    /// </summary>
    private void OnEnable()
    {
        onStateChangedChannel.OnEventRaised += ToggleInputs;

        onSettingsChangedChannel.OnEventRaised += UpdateSettings;

        resetPlayerLook.OnEventRaised += ResetLook;

        onSceneChangeChannel.OnEventRaised += ResetLook;

        onControllerSwap.OnEventRaised += UpdateInputSettings;
    }

    /// <summary>
    /// Disable any inputs and subcscriptions to prevent crashes
    /// </summary>
    private void OnDisable()
    {
        onStateChangedChannel.OnEventRaised -= ToggleInputs;

        onSettingsChangedChannel.OnEventRaised -= UpdateSettings;

        resetPlayerLook.OnEventRaised -= ResetLook;

        onSceneChangeChannel.OnEventRaised -= ResetLook;

        onControllerSwap.OnEventRaised -= UpdateInputSettings;

        if (aim.enabled)
            aim.Disable();
    }

    /// <summary>
    /// Toggle inputs if game pauses
    /// </summary>
    /// <param name="_newState">new state</param>
    private void ToggleInputs(GameManager.States _newState)
    {
        if(_newState == GameManager.States.GAMEPLAY
            || _newState == GameManager.States.HUB)
        {
            aim.Enable();
        }
        else
        {
            aim.Disable();
        }
    }

    private void ResetLook()
    {
        cameraLook.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public float lookDeltaMag;

    private List<Target> aimAssistOptions;
    /// <summary>
    /// search for an aim assist target
    /// </summary>
    private void SearchForTarget()
    {
        // get all targets forwards 
        RaycastHit[] allTargets = Physics.SphereCastAll(cameraLook.position, assistRadius, cameraLook.forward, assistDistance, targetLayers);
        //Debug.DrawLine(cameraLook.position, cameraLook.position + (cameraLook.forward * (assistDistance + assistRadius)), Color.red);

        targetCenter = null;
        targetDot = 0;

        if (allTargets.Length > 0) // if targets found, determine the most 'centered' target
        {
            // prepare comparison variables
            float highestDot = assistConeThreshold;
            Vector3 forwardDir = cameraLook.forward.normalized;

            // first, filter each raycast hit into target references. Prevents multi-target options
            aimAssistOptions.Clear();
            foreach(var tgt in allTargets)
            {
                // check for a direct reference
                Target t = tgt.collider.GetComponent<Target>();
                // if no direct reference, try getting indirect reference (target hitbox) 
                if (t == null)
                {
                    t = tgt.collider.GetComponent<TargetHitbox>()?.Target();

                    // if still no target, move onto the next option
                    if (t == null) continue;
                }

                // make sure no dupes in list
                if (!aimAssistOptions.Contains(t))
                    aimAssistOptions.Add(t);
            }    

            // check each target. Make sure its a target and if so, utilize its center transform
            foreach (var tgt in aimAssistOptions)
            {
                // get dot product of normalized directions.
                // target with the highest DOT product is the most 'center'  
                float newDot = Vector3.Dot(forwardDir, (tgt.Center.position - cameraLook.position).normalized);
                if (newDot > highestDot && newDot > assistConeThreshold)
                {
                    // check for direct line of site using blocker layers. only thing it should hit is the target
                    RaycastHit blockCheck;
                    if (Physics.Raycast(cameraLook.position, (tgt.Center.position - cameraLook.position), out blockCheck, assistDistance + assistRadius, blockLayers))
                    {
                        Target blocker = blockCheck.collider.GetComponent<Target>();
                        // if no direct reference, try getting indirect reference (target hitbox) 
                        if (blocker == null)
                        {
                            blocker = blockCheck.collider.GetComponent<TargetHitbox>()?.Target();
                        }

                        // the target is blocked if it was unable to hit itself
                        if (blocker == null || blocker != tgt)
                            continue;
                    }

                    highestDot = newDot;
                    targetCenter = tgt.Center;
                    targetDot = newDot;
                }
            }
        }
        else // if no targets found, no target
        {
            targetCenter = null;
            targetDot = 0;
        }
    }
}
