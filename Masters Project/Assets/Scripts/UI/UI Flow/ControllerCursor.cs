/* 
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - July 12th, 2023
 * Last Edited - July 13th 2023 by Ben Schuster
 * Description - Virtual mouse for controller UI navigation. Do this bc its faster than redoing 
 * Unity UI navigation
 * ================================================================================================
 */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class ControllerCursor : MonoBehaviour
{
    /// <summary>
    /// Global instance to the controller cursor
    /// </summary>
    public static ControllerCursor instance;

    #region Input Variables

    /// <summary>
    /// Input that reads movement inputs
    /// </summary>
    InputAction cursorMove;
    /// <summary>
    /// Input that reads controller 'progress' inputs
    /// </summary>
    InputAction pressButton;
    /// <summary>
    /// Input that reads attempts at scrolling UI elements
    /// </summary>
    InputAction scrollUI;

    #endregion

    #region Setup Variables
    /// <summary>
    /// ref to personal rect transform
    /// </summary>
    [SerializeField] private RectTransform rectRef;
    /// <summary>
    /// ref to canvas rect transform. Should be its parent
    /// </summary>
    [SerializeField] private RectTransform canvasRectRef;
    /// <summary>
    /// Reference to the original mouse cursor
    /// </summary>
    private Mouse originalMouse;
    /// <summary>
    /// reference to the virtual mouse used in this
    /// </summary>
    private Mouse virtualMouse;

    #endregion

    #region UI Flow Variables

    [Tooltip("Channel called when the controller scheme swaps. Used to determine when to show cursor")]
    [SerializeField] ChannelControlScheme onSchemeSwap;
    /// <summary>
    /// Whether or not the game is currently in a UI state. Managed by Game Manager
    /// </summary>
    private bool inUIState;
    /// <summary>
    /// whether the cursor is currently active
    /// </summary>
    private bool cursorActive;

    #endregion

    #region Tuning Variables

    [Tooltip("The movement speed of the cursor")]
    [SerializeField] float cursorMoveSpeed = 1000f;

    [Tooltip("The speed of the scroll input")]
    [SerializeField] private float scrollSpeed = 50f;

    [Tooltip("Padding around the cursor that manages screen bounds")]
    [SerializeField] private float padding = 30f;

    #endregion

    #region Virtual Mouse Setup/Teardown Funcs

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

        // set up controls
        cursorMove = InputManager.Controls.UI.CursorMove;
        pressButton = InputManager.Controls.UI.CursorClick;
        scrollUI = InputManager.Controls.UI.CursorScroll;

        // get reference to original mouse
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

        // prepare systems that allow for swapping and clicking
        onSchemeSwap.OnEventRaised += DetermineCursorMode;
        pressButton.started += HandleInput;
    }

    /// <summary>
    /// Clear virtual mouse stuff on disable
    /// </summary>
    private void OnDisable()
    {
        // dont do disable actions if not the instance
        if (instance != this) return;

        // remove from input system.
        // Otherwise will cause lots of unity editor errors outside of playmode
        InputSystem.onAfterUpdate -= MoveCursor;
        InputSystem.onAfterUpdate -= ScrollCursor;

        instance = null;

        // if its still added, remove it
        if(virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        // unsub from remaining events
        onSchemeSwap.OnEventRaised -= DetermineCursorMode;
        pressButton.started -= HandleInput;
    }

    #endregion

    #region Mouse Functionality Funcs

    /// <summary>
    /// Move the cursor, keeping screen bounds and padding in mind
    /// </summary>
    private void MoveCursor()
    {
        Vector2 delta = cursorMove.ReadValue<Vector2>();
        delta *= Time.unscaledDeltaTime * cursorMoveSpeed;
        Vector2 newPos = virtualMouse.position.ReadValue();
        newPos += delta;

        // clamp the new position within the screen width. 
        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);
        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, delta);

        // update the position of the cursor
        AnchorCursorVisual(newPos);
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
    /// Handle clicking with the virtual mouse
    /// </summary>
    /// <param name="c"></param>
    private void HandleInput(InputAction.CallbackContext c)
    {
        Debug.Log("Calling handle input : " + c);

        if (!cursorActive) return;

        MouseButton? button = MouseButton.Left;
        var isPressed = c.control.IsPressed();
        virtualMouse.CopyState<MouseState>(out var mouseState);
        mouseState.WithButton(button.Value, isPressed);

        InputState.Change(virtualMouse, mouseState);
    }

    /// <summary>
    /// Calculate and apply scroll on cursor
    /// </summary>
    private void ScrollCursor()
    {
        Vector2 scrollDelta = scrollUI.ReadValue<Vector2>();
        scrollDelta.x *= scrollSpeed;
        scrollDelta.y *= scrollSpeed;
        InputState.Change(virtualMouse.scroll, scrollDelta);
    }

    #endregion

    #region Cursor Activation Funs

    /// <summary>
    /// Set whether the game is in a UI state or not. Will activate gamepad controller
    /// if gamepad is detected
    /// </summary>
    /// <param name="inUI">Whether the game is in a UI state</param>
    public void SetUIState(bool inUI)
    {
        // dont bother doing another check if its already in the new state
        if (inUIState == inUI) return;

        //Debug.Log("Setting UI state to " + inUI);
        inUIState = inUI;

        // If set to true and input mode is controller, enable the cursor
        if (inUIState && InputManager.CurrControlScheme == InputManager.ControlScheme.CONTROLLER)
        {
            EnableControllerCursor();
        }
        // otherwise if not in a UI state but the cursor is enabled, disable it
        else if (!inUIState && cursorActive)
        {
            DisableControllerCursor();
        }
    }

    /// <summary>
    /// Determine whether to show or hide the cursor. This assumes that its already in a UI mode
    /// </summary>
    /// <param name="scheme"></param>
    public void DetermineCursorMode(InputManager.ControlScheme scheme)
    {
        // if not in a UI state, then dont do anything. 
        if (!inUIState) return;

        // enable and disable based on the new scheme
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

        // set active and enable controls 
        cursorActive = true;
        cursorMove.Enable();
        pressButton.Enable();
        scrollUI.Enable();

        // set invisible to hide cursor and instead show custom cursor
        Cursor.visible = false;
        rectRef.gameObject.SetActive(true);

        // move the controller cursor to current location of the main mouse
        InputState.Change(virtualMouse.position, originalMouse.position.ReadValue());
        AnchorCursorVisual(virtualMouse.position.ReadValue());

        // subscribe VM to move function
        InputSystem.onAfterUpdate += MoveCursor;
        InputSystem.onAfterUpdate += ScrollCursor;
    }

    /// <summary>
    /// Deactivate the controller cursor. Will return cursor to the current position
    /// </summary>
    private void DisableControllerCursor()
    {
        // if it was never enabled, then stop
        if (!cursorActive || virtualMouse == null) return;

        // update state and disable controls
        cursorActive = false;
        cursorMove.Disable();
        pressButton.Disable();
        scrollUI.Disable();

        // set main cursor to position of the controller cursor
        originalMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        rectRef.gameObject.SetActive(false);

        // remove active virtual mouse
        InputSystem.onAfterUpdate -= MoveCursor;
        InputSystem.onAfterUpdate -= ScrollCursor;

        // show the mouse again. call to update the mouse via game manager to ensure cursor is 
        // set to its correct state
        Cursor.visible = true;
        GameManager.instance.UpdateMouseMode();
    }

    #endregion
}
