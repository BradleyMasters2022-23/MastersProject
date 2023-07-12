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
    public static ControllerCursor instance;

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

    [Tooltip("Channel called when the controller scheme swaps. Used to determine when to show cursor")]
    [SerializeField] ChannelControlScheme onSchemeSwap;

    /// <summary>
    /// The previous input state
    /// </summary>
    bool previousMouseState;

    /// <summary>
    /// Reference to the original mouse cursor
    /// </summary>
    Mouse originalMouse;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }

        cursorMove = InputManager.Controls.UI.CursorMove;
        originalMouse = Mouse.current;


        onSchemeSwap.OnEventRaised += DetermineCursorMode;

        DisableControllerCursor();
    }

    /// <summary>
    /// Clear virtual mouse stuff on disable
    /// </summary>
    private void OnDisable()
    {
        // dont do disable actions if not the instance
        if (instance != this) return;
    }

    /// <summary>
    /// Move the cursor, keeping screen bounds and padding in mind
    /// </summary>
    private void MoveCursor()
    {
        Vector2 delta = Gamepad.current.leftStick.ReadValue();
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

    /// <summary>
    /// Determine whether to show or hide the cursor. This assumes that its already in a UI mode
    /// </summary>
    /// <param name="scheme"></param>
    public void DetermineCursorMode(InputManager.ControlScheme scheme)
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

    /// <summary>
    /// Activate the controller cursor. Starts from the original location of the cursor
    /// </summary>
    private void EnableControllerCursor()
    {
        if (cursorMove == null)
            cursorMove = InputManager.Controls.UI.CursorMove;
        cursorMove.Enable();

        // Get and prepare the virtual mouse if not already
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }
        //InputState.Change(virtualMouse.position, rectRef.anchoredPosition);
        // subscribe to the input move system
        InputSystem.onAfterUpdate += MoveCursor;


        Cursor.visible = false;
        rectRef.gameObject.SetActive(true);

        InputState.Change(virtualMouse.position, originalMouse.position.ReadValue());
        AnchorCursorVisual(virtualMouse.position.ReadValue());
    }

    /// <summary>
    /// Deactivate the controller cursor. Will return cursor to the current position
    /// </summary>
    private void DisableControllerCursor()
    {
        // if it was never enabled, then stop
        if (virtualMouse == null) return;

        if(cursorMove != null)
            cursorMove.Disable();

        originalMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        rectRef.gameObject.SetActive(false);

        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= MoveCursor;
        onSchemeSwap.OnEventRaised -= DetermineCursorMode;

        //Cursor.visible = true;
        GameManager.instance.UpdateMouseMode();
    }
}
