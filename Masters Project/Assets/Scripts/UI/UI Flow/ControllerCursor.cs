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

    /// <summary>
    /// Whether or not the game is currently in a UI state. Managed by Game Manager
    /// </summary>
    private bool inUIState;

    private bool cursorActive;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        cursorMove = InputManager.Controls.UI.CursorMove;
        originalMouse = Mouse.current;

        // Prepare the virtual mouse if its not already prepared
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

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

        instance = null;

        if(virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        onSchemeSwap.OnEventRaised -= DetermineCursorMode;
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
            Debug.Log("A button pressed detected");
            mouseState.WithButton(MouseButton.Left, !aButtPressed);
            

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
        // if not in a UI state, then dont do anything. 
        if (!inUIState) return;

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
        if (cursorActive) return;

        cursorActive = true;
        cursorMove.Enable();

        // set invisible to hide cursor and instead show custom cursor
        Cursor.visible = false;
        rectRef.gameObject.SetActive(true);

        // move the controller cursor to current location of the main mouse
        InputState.Change(virtualMouse.position, originalMouse.position.ReadValue());
        AnchorCursorVisual(virtualMouse.position.ReadValue());

        // subscribe VM to move function
        InputSystem.onAfterUpdate += MoveCursor;
    }

    /// <summary>
    /// Deactivate the controller cursor. Will return cursor to the current position
    /// </summary>
    private void DisableControllerCursor()
    {
        // if it was never enabled, then stop
        if (!cursorActive || virtualMouse == null) return;

        // update states
        cursorActive = false;
        cursorMove.Disable();

        // set main cursor to position of the controller cursor
        originalMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        rectRef.gameObject.SetActive(false);

        // remove active virtual mouse
        InputSystem.onAfterUpdate -= MoveCursor;

        // show the mouse again. call to update the mouse via game manager to ensure cursor is 
        // set to its correct state
        Cursor.visible = true;
        GameManager.instance.UpdateMouseMode();
    }

    /// <summary>
    /// Set whether the game is in a UI state or not. Will activate gamepad controller
    /// if gamepad is detected
    /// </summary>
    /// <param name="inUI">Whether the game is in a UI state</param>
    public void SetUIState(bool inUI)
    {
        // dont bother doing another check if its already in the new state
        if(inUIState == inUI) return;

        //Debug.Log("Setting UI state to " + inUI);
        inUIState = inUI;

        // If set to true and input mode is controller, enable the cursor
        if(inUIState && InputManager.CurrControlScheme == InputManager.ControlScheme.CONTROLLER)
        {
            EnableControllerCursor();
        }
        // otherwise if not in a UI state but the cursor is enabled, disable it
        else if (!inUIState && cursorActive)
        {
            DisableControllerCursor();
        }
    }
}
