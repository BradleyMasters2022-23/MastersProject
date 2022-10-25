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
    [SerializeField] private float lookSensitivity;
    [Tooltip("The minimum and maximum rotation for the camera's vertical rotation.")]
    [SerializeField] private Vector2 angleClamp;
    [Tooltip("Primary point the camera looks at and pivots from. Should be around the player's shoulders.")]
    [SerializeField] private Transform cameraLook;

    /// <summary>
    /// Internal tracker for the current vertical rotation
    /// </summary>
    private float verticalLookRotation;

    /// <summary>
    /// Core controller map
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Action input for aiming the camera [mouse delta / joystick delta]
    /// </summary>
    private InputAction aim;

    private Vector2 lookDelta;

    private void Awake()
    {
        // Initialize aim controls
        controller = new GameControls();
        aim = controller.PlayerGameplay.Aim;
        aim.Enable();

        // TODO - Link to settings, load in player preferences
        verticalLookRotation = cameraLook.localRotation.x;
    }

    private void Update()
    {
        // Get new delta based on update
        lookDelta = aim.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        ManageCamera();
    }

    /// <summary>
    /// Update the camera's rotation
    /// </summary>
    private void ManageCamera()
    {
        // look left and right
        transform.Rotate(new Vector3(0, lookDelta.x, 0) * Time.deltaTime * lookSensitivity);

        // Look up and down, clamping within range 
        verticalLookRotation -= lookDelta.y * Time.deltaTime * lookSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, angleClamp.x, angleClamp.y);
        cameraLook.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    /// <summary>
    /// Disable any inputs to prevent crashes
    /// </summary>
    private void OnDisable()
    {
        aim.Disable();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
