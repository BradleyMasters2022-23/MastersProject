/*
 * ================================================================================================
 * Author - Ben Schuster
 * Date Created - October 21th, 2022
 * Last Edited - October 25, 2022 by Soma Hannon - add getters section and 1 getter for upgrade tests
 * Description - Manage the movement for the player
 * ================================================================================================
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        GROUNDED,
        SPRINTING,
        MIDAIR,
        FROZEN
    }

    #region Core References

    /// <summary>
    /// Current state of the player
    /// </summary>
    private PlayerState currentState;
    /// <summary>
    /// Current state of the player
    /// </summary>
    public PlayerState CurrentState
    {
        get { return currentState; }
    }

    /// <summary>
    /// Player's rigidbody
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// Main character animator
    /// </summary>
    private Animator animator;

    #endregion

    #region Inputs

    /// <summary>
    /// Core controller map
    /// </summary>
    private GameControls controller;
    /// <summary>
    /// Input for moving horizontally
    /// </summary>
    private InputAction move;
    /// <summary>
    /// Input for jumping
    /// </summary>
    private InputAction jump;
    /// <summary>
    /// Input for sprinting
    /// </summary>
    private InputAction sprint;
    /// <summary>
    /// Input for restarting the scene
    /// </summary>
    private InputAction debugRestart;

    #endregion

    #region Horizontal Movement Variables

    [Header("=====Player Movement=====")]

    [Header("---Movement---")]

    [Tooltip("Maximum move speed for player")]
    [SerializeField] private UpgradableFloat maxMoveSpeed;
    [Tooltip("Speed modifier for player when sprinting")]
    [SerializeField] private UpgradableFloat sprintModifier;
    [Tooltip("Speed modifier for player when midair")]
    [SerializeField] private UpgradableFloat airModifier;
    [Tooltip("Time it takes to reach full speed")]
    [SerializeField][Range(0, 1)] private float accelerationTime;
    [Tooltip("Minimum speed the player starts at when initially inputting a direction. % of Max Move Speed.")]
    [SerializeField][Range(0, 1)] private float startingSpeedPercentage;
    [Tooltip("Time it takes to fully stop")]
    [SerializeField][Range(0, 1)] private float decelerationTime;

    /// <summary>
    /// Direction the player is inputting
    /// </summary>
    private Vector3 direction;
    /// <summary>
    /// Current target max speed
    /// </summary>
    private float targetMaxSpeed;
    /// <summary>
    /// Current speed of the player
    /// </summary>
    private float currSpeed;
    /// <summary>
    /// Tracker for acceleration lerping
    /// </summary>
    private float accelerateLerp;
    /// <summary>
    /// Tracker for decelerate lerping
    /// </summary>
    private float decelerateLerp;

    #endregion

    #region Vertical Movement Variables

    [Header("---Jumping---")]

    [Tooltip("Amount of times the player can jump")]
    [SerializeField] private UpgradableInt jumps;
    [Tooltip("How high can the player can jump")]
    [SerializeField] private float jumpForce;
    [Tooltip("Cooldown between jumps")]
    [SerializeField] private float jumpCooldown;
    [Tooltip("Whether the player loses their first jump when falling off a ledge")]
    [SerializeField] private bool disableFirstJumpOnFall;
    [Tooltip("Whether the player can pivot movement when jumping")]
    [SerializeField] private bool jumpPivot;

    /// <summary>
    /// Amount of jumps remaining
    /// </summary>
    private int currentJumps;
    /// <summary>
    /// Cooldown timer between jumps
    /// </summary>
    private ScaledTimer jumpTimer;

    [Header("---Gravity---")]

    [Tooltip("Strength of the gravity")]
    [SerializeField] private float gravityMultiplier;
    [Tooltip("Physics layers can the player walk on")]
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Transform at the bottom of this character object")]
    [SerializeField] private Transform groundCheck;
    /// <summary>
    /// Radius of the ground check
    /// </summary>
    private const float GroundCheckRadius = 0.1f;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize inputs and starting variables
    /// </summary>
    private void Awake()
    {
        // Initialize controls
        controller = new GameControls();
        move = controller.PlayerGameplay.Move;
        move.Enable();

        jump = controller.PlayerGameplay.Jump;
        jump.Enable();
        jump.performed += Jump;

        sprint = controller.PlayerGameplay.Sprint;
        sprint.started += ToggleSprint;
        sprint.canceled += ToggleSprint;
        sprint.Enable();

        debugRestart = controller.PlayerGameplay.Reset;
        debugRestart.performed += DebugRestartScene;
        debugRestart.Enable();

        // Initialize upgradable variables
        maxMoveSpeed.Initialize();
        sprintModifier.Initialize();
        airModifier.Initialize();
        jumps.Initialize();

        // Initialize internal variables
        jumpTimer = new ScaledTimer(jumpCooldown, false);
        currentJumps = jumps.Current;
        targetMaxSpeed = maxMoveSpeed.Current;
    }

    /// <summary>
    /// Get any references needed, perform other starting functionality
    /// </summary>
    private void Start()
    {
        // Get initial references
        rb = GetComponent<Rigidbody>();
        //animator = GetComponentInChildren<Animator>();

        // If the gravity modifier has not already been applied, apply it now
        if (Physics.gravity.y >= -10)
            Physics.gravity *= gravityMultiplier;
    }

    #endregion

    private void FixedUpdate()
    {
        // Get current direction based on player input
        direction = move.ReadValue<Vector2>();
        direction = transform.right * direction.x + transform.forward * direction.y;

        // Perform state-based update functionality
        UpdateStateFunction();

        // Limit any velocity to prevent player going too fast
        LimitVelocity();
    }

    #region State Functionality

    /// <summary>
    /// Perform update functionality based on current state
    /// </summary>
    private void UpdateStateFunction()
    {
        switch (currentState)
        {
            case PlayerState.GROUNDED:
                {
                    HorizontalMovement();

                    // If not on ground, set state to midair. Disable sprint
                    if (!Physics.CheckSphere(groundCheck.position, GroundCheckRadius, groundMask))
                    {
                        ChangeState(PlayerState.MIDAIR);
                    }

                    break;
                }
            case PlayerState.SPRINTING:
                {
                    HorizontalMovement();

                    // If not on ground, set state to midair. Disable sprint
                    if (!Physics.CheckSphere(groundCheck.position, GroundCheckRadius, groundMask))
                    {
                        ChangeState(PlayerState.MIDAIR);
                    }


                    break;
                }
            case PlayerState.MIDAIR:
                {
                    HorizontalMovement();

                    // If player lands, set state to grounded. Enable sprint
                    if(Physics.CheckSphere(groundCheck.position, GroundCheckRadius, groundMask))
                    {
                        ChangeState(PlayerState.GROUNDED);
                    }

                    break;
                }
            case PlayerState.FROZEN:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// Change the current state, perform any state-change functionality
    /// </summary>
    /// <param name="_newState">new state to move to</param>
    private void ChangeState(PlayerState _newState)
    {
        switch(_newState)
        {
            case PlayerState.GROUNDED:
                {
                    // When entering grounded state, reset jumps
                    currentJumps = jumps.Current;

                    // When entering grounded state, reset target max speed
                    targetMaxSpeed = maxMoveSpeed.Current;

                    break;
                }
            case PlayerState.SPRINTING:
                {
                    // When entering sprint state, increase target max move speed
                    targetMaxSpeed = maxMoveSpeed.Current * sprintModifier.Current;

                    break;
                }
            case PlayerState.MIDAIR:
                {
                    // If the player went from grounded to midair, check if a jump should be removed
                    if(currentState == PlayerState.GROUNDED)
                    {
                        if(disableFirstJumpOnFall && currentJumps == jumps.Current)
                        {
                            currentJumps--;
                        }
                    }

                    break;
                }
            case PlayerState.FROZEN:
                {
                    break;
                }
        }

        currentState = _newState;
    }

    /// <summary>
    /// Change states based on sprint input
    /// </summary>
    /// <param name="ctx">Input callback context [ignorable]</param>
    private void ToggleSprint(InputAction.CallbackContext ctx)
    {
        // If player is grounded and started input, start sprinting
        if (currentState == PlayerState.GROUNDED && ctx.started)
        {
            ChangeState(PlayerState.SPRINTING);
        }
        // If player is sprinting and canceled input, stop sprinting
        else if (currentState == PlayerState.SPRINTING && ctx.canceled)
        {
            ChangeState(PlayerState.GROUNDED);
        }
    }

    #endregion

    #region Player Movement

    /// <summary>
    /// Manage the player's horizontal movement
    /// </summary>
    private void HorizontalMovement()
    {
        // If not inputing, try decelerating
        if (direction == Vector3.zero)
        {
            Decelerate();

            // Reset lerp for acceleration
            accelerateLerp = accelerationTime * startingSpeedPercentage;
        }
        // If inputting, try accelerating
        else if (direction != Vector3.zero)
        {
            Accelerate();

            // Reset lerp for deceleration
            decelerateLerp = 0;
        }

        // Prepare new velocity
        Vector3 newVelocity;
        if (currentState != PlayerState.MIDAIR)
        {
            // If any input, set new velocity to the direction of the player input
            if (direction != Vector3.zero)
            {
                newVelocity = Mathf.Pow(currSpeed, 2) * Time.deltaTime * direction;
            }
            // If no input, set new velocity to the direction the rb is already moving towards
            else
            {
                newVelocity = Mathf.Pow(currSpeed, 2) * Time.deltaTime * rb.velocity.normalized;
            }
        }
        else
        {
            // If midair, add the modified input to the existing velocity, instead of overriding it
            newVelocity = Mathf.Pow((currSpeed * airModifier.Current), 2) * Time.deltaTime * direction;
            newVelocity += rb.velocity;
        }

        // Use the existing vertical velocity, as that is handled by gravity
        newVelocity.y = rb.velocity.y;

        // Assign new velocity
        rb.velocity = newVelocity;


        // === ANIMATION STUFF ===
        float _xaxis = rb.velocity.x;
        float _zaxis = rb.velocity.z;

        //animator.SetFloat("X Move", _xaxis);
        //animator.SetFloat("Y Move", _zaxis);
    }

    /// <summary>
    /// Launch the player upwards, if possible
    /// </summary>
    /// <param name="c">Input callback context [ignorable]</param>
    private void Jump(InputAction.CallbackContext c)
    {
        if(currentJumps > 0 && jumpTimer.TimerDone())
        {
            // Adjust jumps, and reset any jumping cooldown
            jumpTimer.ResetTimer();
            currentJumps--;

            // Prepare new velocity
            Vector3 newVelocity = rb.velocity;

            // Redirect player velocity when jumping, if enabled
            if (jumpPivot)
            {
                // Calculate direction velocity, set vertical velocity to 0
                Vector3 airDir = direction * Mathf.Pow((targetMaxSpeed), 2) * Time.deltaTime;

                // Set new velocity to direction
                newVelocity = Vector3.zero + airDir;
            }

            // Apply new horizontal velocity and reset vertical velocity
            newVelocity.y = 0;
            rb.velocity = newVelocity;

            // Apply vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Limit the player's velocity to the current max speed
    /// </summary>
    private void LimitVelocity()
    {
        // Get the current x and z velocity. y does not need to be limited.
        Vector2 horizontalVelocity = new Vector2(rb.velocity.x, rb.velocity.z);

        // Get the current max velocity
        float currentMax;
        if (currentState == PlayerState.MIDAIR)
        {
            // Not sure why, but this needs to be divided by 2. Otherwise, player's movespeed is doubled mid air.
            currentMax = targetMaxSpeed / 2;
        }
        else if (currentState == PlayerState.SPRINTING)
        {
            currentMax = targetMaxSpeed * sprintModifier.Current;
        }
        else
        {
            currentMax = targetMaxSpeed;
        }

        // If magnitude is greater than max, set to max speed instead
        if (horizontalVelocity.magnitude > currentMax)
        {
            // Reset velocity to current max, apply to rigidbody
            horizontalVelocity = horizontalVelocity.normalized * currentMax;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.y);

        }
    }

    /// <summary>
    /// Lerp current speed to target speed
    /// </summary>
    private void Accelerate()
    {
        // Increment lerp
        if(accelerateLerp < accelerationTime)
        {
            accelerateLerp += Time.deltaTime;
            accelerateLerp = Mathf.Clamp(accelerateLerp, 0, accelerationTime);
        }

        // If current has not reached target, continue lerping
        if (currSpeed != targetMaxSpeed)
        {
            currSpeed = Mathf.Lerp(0, targetMaxSpeed, accelerateLerp/accelerationTime);
        }
    }

    /// <summary>
    /// Lerp current speed to 0
    /// </summary>
    private void Decelerate()
    {
        // Increment lerp
        if(decelerateLerp < decelerationTime)
        {
            decelerateLerp += Time.deltaTime;
            decelerateLerp = Mathf.Clamp(decelerateLerp, 0, decelerationTime);
        }

        // If current has not reached 0, continue lerping
        if (currSpeed > 0)
        {
            currSpeed = Mathf.Lerp(targetMaxSpeed, 0, decelerateLerp / decelerationTime);
        }
    }

    #endregion

    #region Getters
    /// <summary>
    /// get # of jumps
    /// </summary>
    public UpgradableInt GetJumps() {
      return jumps;
    }

    #endregion

    #region Misc

    /// <summary>
    /// Disable inputs to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        debugRestart.Disable();
        sprint.Disable();
    }

    /// <summary>
    /// Reload the current scene, reset timescale. TEMP
    /// </summary>
    /// <param name="ctx"></param>
    private void DebugRestartScene(InputAction.CallbackContext ctx)
    {
        Time.timeScale = 1;
    }

    #endregion
}
