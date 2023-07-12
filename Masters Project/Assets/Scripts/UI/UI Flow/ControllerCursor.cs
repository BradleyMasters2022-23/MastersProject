/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 12thm 2023
 * Last Edited - July 12thm 2023 by Ben Schuster
 * Description - Virtual mouse for controller UI navigation. Do this bc its faster than redoing 
 * Unity UI navigation
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ControllerCursor : MonoBehaviour
{
    [Tooltip("The movement speed of the cursor")]
    [SerializeField] float cursorMoveSpeed = 3;
    /// <summary>
    /// Input that reads 
    /// </summary>
    InputAction cursorMove;
    /// <summary>
    /// ref to personal rect transform
    /// </summary>
    [SerializeField] private RectTransform rectRef;
    /// <summary>
    /// ref to canvas rect transform. Should be its parent
    /// </summary>
    [SerializeField] private RectTransform canvasRectRef;
    /// <summary>
    /// reference to the virtual mouse used in this
    /// </summary>
    private Mouse virtualMouse;
    /// <summary>
    /// padding used for edges of the screen
    /// </summary>
    public float padding = 30f;
    /// <summary>
    /// previous press state of the virtual mouse
    /// </summary>
    private bool previousMouseState;

    [SerializeField] ChannelControlScheme onSchemeSwap;

    Mouse originalMouse;

    /// <summary>
    /// Get all necessary references
    /// </summary>
    private void Awake()
    {
        cursorMove = InputManager.Controls.UI.CursorMove;
        cursorMove.Enable();
    }

    /// <summary>
    /// Prepare virtual mouse system on enable
    /// </summary>
    private void OnEnable()
    {
        originalMouse = Mouse.current;

        // Get and prepare the virtual mouse if not already
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        // subscribe to the input move system
        InputSystem.onAfterUpdate += MoveCursor;

        onSchemeSwap.OnEventRaised += DetermineCursorMode;
    }

    /// <summary>
    /// Clear virtual mouse stuff on disable
    /// </summary>
    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= MoveCursor;

        onSchemeSwap.OnEventRaised -= DetermineCursorMode;
    }

    /// <summary>
    /// Move the cursor, keeping screen bounds and padding in mind
    /// </summary>
    private void MoveCursor()
    {
        Vector2 delta = Gamepad.current.rightStick.ReadValue();

        // dont bother with calculations if there is no input
        if (delta.magnitude > 0)
        {
            delta *= Time.unscaledDeltaTime * cursorMoveSpeed;
            Vector2 newPos = virtualMouse.position.ReadValue();
            newPos += delta;

            // clamp the new position within the screen width. 
            newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
            newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);
            InputState.Change(virtualMouse.position, newPos);
            InputState.Change(virtualMouse.delta, delta);

            AnchorCursorVisual(newPos);
        }

        // if an input is done, tell the 'virtual mouse' to activate
        bool aButtPressed = Gamepad.current.aButton.isPressed;
        if (previousMouseState != aButtPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtPressed;
        }
    }

    /// <summary>
    /// Update the position of the visual rect for the cursor
    /// </summary>
    /// <param name="position">Position to move to</param>
    private void AnchorCursorVisual(Vector2 position)
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectRef, position, null, out anchoredPos);
        rectRef.anchoredPosition = anchoredPos;
    }

    private void DetermineCursorMode(InputManager.ControlScheme scheme)
    {
        switch (scheme)
        {
            case InputManager.ControlScheme.KEYBOARD:
                {
                    DisableControllerCursor();
                    break;
                }
            case InputManager.ControlScheme.CONTROLLER:
                {
                    EnableControllerCursor(); 
                    break;
                }
        }
    }

    private void DisableControllerCursor()
    {
        originalMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        rectRef.gameObject.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }


    private void EnableControllerCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rectRef.gameObject.SetActive(true);

        InputState.Change(virtualMouse.position, originalMouse.position.ReadValue());
        AnchorCursorVisual(virtualMouse.position.ReadValue());
    }
}
