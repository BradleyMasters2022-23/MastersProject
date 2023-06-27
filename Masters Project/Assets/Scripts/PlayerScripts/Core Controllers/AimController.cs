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

    [SerializeField] private float mouseSensitivity;
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

    /// <summary>
    /// Core controller map
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Action input for aiming the camera [mouse delta / joystick delta]
    /// </summary>
    private InputAction aim;

    private void Awake()
    {
        // Initialize aim controls
        StartCoroutine(InitializeControls());

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

        Debug.Log("Aim sensitivity set to : " + mouseSensitivity);
    }

    private void LateUpdate()
    {
        // Get new delta based on update
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
        sensitivity = 0;
        horizontalInversion = 1;
        verticalInversion = 1;

        if (aim.ReadValue<Vector2>() != Vector2.zero)
        {
            // TODO - Look up controller vs keyboard here for sensitivity
            lookDelta = aim.ReadValue<Vector2>();
            sensitivity = mouseSensitivity;

            if (mouseXInverted)
                horizontalInversion *= -1;
            if (mouseYInverted)
                verticalInversion *= -1;

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
        Quaternion temp = cameraLook.transform.localRotation *
            Quaternion.AngleAxis(-lookDelta.y * sensitivity * verticalInversion * Time.deltaTime, Vector3.right);

        // Make sure to clamp vertical angle first
        float newYAngle = temp.eulerAngles.x;
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
}
