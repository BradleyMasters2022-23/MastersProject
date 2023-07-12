using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;

public class TestNewScript : MonoBehaviour
{
    [Tooltip("The movement speed of the cursor")]
    [SerializeField] float cursorMoveSpeed = 1000;
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

    bool previousMouseState;

    GameControls c;

    /// <summary>
    /// Get all necessary references
    /// </summary>
    private void Awake()
    {
        c = new GameControls();
        cursorMove = c.UI.CursorMove;
        cursorMove.Enable();
    }

    /// <summary>
    /// Prepare virtual mouse system on enable
    /// </summary>
    private void OnEnable()
    {
        // Get and prepare the virtual mouse if not already
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputState.Change(virtualMouse.position, rectRef.anchoredPosition);

        // subscribe to the input move system
        InputSystem.onAfterUpdate += MoveCursor;
    }

    /// <summary>
    /// Clear virtual mouse stuff on disable
    /// </summary>
    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= MoveCursor;
        InputSystem.RemoveDevice(virtualMouse);
    }

    /// <summary>
    /// Move the cursor, keeping screen bounds and padding in mind
    /// </summary>
    private void MoveCursor()
    {
        Vector2 delta = Gamepad.current.rightStick.ReadValue();
        delta *= Time.unscaledDeltaTime * cursorMoveSpeed;
        Vector2 newPos = virtualMouse.position.ReadValue();
        newPos += delta;

        // clamp the new position within the screen width. 
        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);
        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, delta);

        AnchorCursorVisual(newPos);

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
}
