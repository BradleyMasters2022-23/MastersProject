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
    [Tooltip("Look sensitivity for the camera.")]
    [SerializeField] private float mouseSensitivity;

    [Tooltip("Look sensitivity for the controller.")]
    [SerializeField] private float controllerSensitivity;

    [SerializeField] private bool mouseYInverted;
    [SerializeField] private bool mouseXInverted;

    [SerializeField] private bool controllerYInverted;
    [SerializeField] private bool controllerXInverted;

    [Tooltip("The minimum and maximum rotation for the camera's vertical rotation.")]
    [SerializeField] private Vector2 angleClamp;
    [Tooltip("Primary point the camera looks at and pivots from. Should be around the player's shoulders.")]
    [SerializeField] private Transform cameraLook;


    [Header("---Game flow---")]
    [Tooltip("Channel that watches the game manager states")]
    [SerializeField] private ChannelGMStates onStateChangedChannel;

    /// <summary>
    /// Core controller map
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Action input for aiming the camera [mouse delta / joystick delta]
    /// </summary>
    private InputAction aim;
    private InputAction controllerAim;

    private void Awake()
    {
        // Initialize aim controls
        controller = new GameControls();
        aim = controller.PlayerGameplay.Aim;
        aim.Enable();

        controllerAim = controller.PlayerGameplay.ControllerAim;
        controllerAim.Enable();

        // TODO - Link to settings, load in player preferences

    }

    private void LateUpdate()
    {
        // Get new delta based on update
        ManageCamera();
    }

    /// <summary>
    /// Update the camera's rotation
    /// </summary>
    private void ManageCamera()
    {
        // Figure out which control input to use, adjust sensitivity values
        Vector2 lookDelta;
        float sensitivity;
        float horizontalInversion = 1;
        float verticalInversion = 1;

        if (aim.ReadValue<Vector2>() != Vector2.zero)
        {
            lookDelta = aim.ReadValue<Vector2>();
            sensitivity = mouseSensitivity;

            if (mouseXInverted)
                horizontalInversion *= -1;
            if (mouseYInverted)
                verticalInversion *= -1;

        }
        else if (controllerAim.ReadValue<Vector2>() != Vector2.zero)
        {
            lookDelta = controllerAim.ReadValue<Vector2>();
            sensitivity = controllerSensitivity;

            if (controllerXInverted)
                horizontalInversion *= -1;
            if (controllerYInverted)
                verticalInversion *= -1;
        }
        else
        {
            return;
        }


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
    }

    /// <summary>
    /// Disable any inputs and subcscriptions to prevent crashes
    /// </summary>
    private void OnDisable()
    {
        onStateChangedChannel.OnEventRaised -= ToggleInputs;

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
}
