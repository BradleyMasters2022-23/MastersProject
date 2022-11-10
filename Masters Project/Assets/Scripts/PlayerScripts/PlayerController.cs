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
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.Rendering;

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

    [Header("---Game Flow---")]
    [SerializeField] private ChannelGMStates onStateChangeChannel;

    public static GameObject instance;

    /// <summary>
    /// Current state of the player
    /// </summary>
    [SerializeField] private PlayerState currentState;
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

    [Tooltip("Center of the player")]
    [SerializeField] private Transform centerMass;
    /// <summary>
    /// Center of the player
    /// </summary>
    public Transform CenterMass
    {
        get { return centerMass;  }
    }

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

    [Tooltip("Sound when the player jumps")]
    [SerializeField] private AudioClip jumpSound;
    [Tooltip("Sound when the player lands")]
    [SerializeField] private AudioClip landSound;
    private AudioSource source;

    /// <summary>
    /// Amount of jumps remaining
    /// </summary>
    private int currentJumps;
    /// <summary>
    /// Cooldown timer between jumps
    /// </summary>
    private ScaledTimer jumpTimer;

    [Header("---Gravity and Ground---")]

    [Tooltip("Strength of the gravity")]
    [SerializeField] private float gravityMultiplier;
    [Tooltip("Physics layers can the player walk on")]
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Transform at the bottom of this character object")]
    [SerializeField] private Transform groundCheck;

    /// <summary>
    /// Radius of the ground check
    /// </summary>
    private float groundCheckRadius = 0.3f;

    /// <summary>
    /// Check how long its been since jumping
    /// </summary>
    private ScaledTimer midAirTimer;
    /// <summary>
    /// The last used surface normal
    /// </summary>
    private Vector3 lastSurfaceNormal;

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

        // Initialize upgradable variables
        maxMoveSpeed.Initialize();
        sprintModifier.Initialize();
        airModifier.Initialize();
        jumps.Initialize();

        // Initialize internal variables
        jumpTimer = new ScaledTimer(jumpCooldown, false);
        midAirTimer = new ScaledTimer(0.5f, false);
        currentJumps = jumps.Current;
        targetMaxSpeed = maxMoveSpeed.Current;

        // Initialize normal
        RaycastHit normal;
        if (Physics.Raycast(groundCheck.position, -groundCheck.up, out normal, Mathf.Infinity))
        {
            lastSurfaceNormal = normal.normal;
        }

        source = gameObject.AddComponent<AudioSource>();        
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

    private void Update()
    {
        if(currentState == PlayerState.GROUNDED)
            AdjustForSlope();
    }

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
                    AdjustForSlope();


                    // If not on ground, set state to midair. Disable sprint
                    if (!Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask))
                    {
                        ChangeState(PlayerState.MIDAIR);
                    }

                    break;
                }
            case PlayerState.SPRINTING:
                {
                    HorizontalMovement();

                    // If not on ground, set state to midair. Disable sprint
                    if (!Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask))
                    {
                        ChangeState(PlayerState.MIDAIR);
                    }


                    break;
                }
            case PlayerState.MIDAIR:
                {
                    HorizontalMovement();

                    // backup check just to make sure nothing breaks with slope
                    if (!rb.useGravity)
                        rb.useGravity = true;

                    // If player lands and is moving downward, move back to grounded state
                    if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask) 
                        && rb.velocity.y < 0)
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
                    source.PlayOneShot(landSound, 0.5f);

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
                    rb.useGravity = true;

                    // If the player went from grounded to midair, check if a jump should be removed
                    if (currentState == PlayerState.GROUNDED)
                    {
                        if (disableFirstJumpOnFall && currentJumps == jumps.Current)
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

        // apply new velocity
        rb.velocity = newVelocity;

        // === ANIMATION STUFF ===
        float _xaxis = rb.velocity.x;
        float _zaxis = rb.velocity.z;

        //animator.SetFloat("X Move", _xaxis);
        //animator.SetFloat("Y Move", _zaxis);
    }

    private void AdjustForSlope()
    {
        if (rb.velocity == Vector3.zero || !midAirTimer.TimerDone())
            return;

        //Debug.DrawLine(groundCheck.position, groundCheck.position + lastSurfaceNormal * -groundCheckRadius, Color.red, 10f);

        // Modify velocity to be slope/friendly
        RaycastHit slopeCheck;
        if (Physics.Raycast(groundCheck.position, -lastSurfaceNormal, out slopeCheck, groundCheckRadius))
        {

            // project velocity onto plane player is standing on
            Vector3 temp = Vector3.ProjectOnPlane(rb.velocity, slopeCheck.normal);

            // if on a slope, disable gravity to prevent sliding
            if (slopeCheck.normal != Vector3.up && rb.useGravity)
            {
                rb.useGravity = false;
            }
            // Else, return to normal
            else if (slopeCheck.normal == Vector3.up && !rb.useGravity)
            {
                rb.useGravity = true;
            }

            // If on flat ground and on same plane, keep the Y velocity
            if (slopeCheck.normal == Vector3.up && lastSurfaceNormal == slopeCheck.normal)
                temp.y = rb.velocity.y;

            // Update last surface normal
            lastSurfaceNormal = slopeCheck.normal;


            // apply force based on ground's normal
            rb.velocity = temp;
        }
    }


    /// <summary>
    /// Launch the player upwards, if possible
    /// </summary>
    /// <param name="c">Input callback context [ignorable]</param>
    private void Jump(InputAction.CallbackContext c)
    {
        if(currentJumps > 0 && jumpTimer.TimerDone())
        {
            midAirTimer.ResetTimer();

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

            source.PlayOneShot(jumpSound, 0.5f);
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

    public void RefreshJumps() {
      currentJumps = jumps.Current;
    }

    #endregion

    #region Misc

    private void OnEnable()
    {
        onStateChangeChannel.OnEventRaised += ToggleInputs;
    }

    /// <summary>
    /// Disable inputs to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        onStateChangeChannel.OnEventRaised -= ToggleInputs;
        
        if(move.enabled)
            move.Disable();
        if(jump.enabled)
            jump.Disable();
        if(sprint.enabled)
            sprint.Disable();
    }

    /// <summary>
    /// Toggle inputs if game pauses
    /// </summary>
    /// <param name="_newState">new state</param>
    private void ToggleInputs(GameManager.States _newState)
    {
        if (_newState == GameManager.States.GAMEPLAY
            || _newState == GameManager.States.HUB)
        {
            move.Enable();
            jump.Enable();
            sprint.Enable();
        }
        else
        {
            move.Disable();
            jump.Disable();
            sprint.Disable();
        }
    }

    #endregion
}
