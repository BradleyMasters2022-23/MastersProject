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

    /// <summary>
    /// The last used surface normal
    /// </summary>
    private Vector3 velocity;


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
    /// Input for special Q abilioty
    /// </summary>
    private InputAction qInput;

    #endregion

    #region Horizontal Movement Variables

    [Header("=====Player Movement=====")]

    [Header("---Movement---")]

    [Tooltip("Maximum move speed for player")]
    [SerializeField] private UpgradableFloat maxMoveSpeed;
    [Tooltip("Time it takes to reach full speed")]
    [SerializeField] private float accelerationSpeed;
    [Tooltip("Amount of drag player experiences when on the ground")]
    [SerializeField] private float drag;

    [Tooltip("Speed modifier for player when sprinting")]
    [SerializeField] private UpgradableFloat sprintModifier;
    [Tooltip("Speed modifier for player when midair")]
    [SerializeField] private UpgradableFloat airModifier;

    
    
    /// <summary>
    /// inputDirection the player is inputting
    /// </summary>
    private Vector3 inputDirection;
    /// <summary>
    /// Current target max speed
    /// </summary>
    private float targetMaxSpeed;
    /// <summary>
    /// Current speed of the player
    /// </summary>
    private float currSpeed;

    private bool sprintHeld;

    #endregion

    #region Vertical Movement Variables

    [Header("---Jumping---")]

    [Tooltip("Amount of times the player can jump")]
    [SerializeField] private UpgradableInt jumps;
    [Tooltip("How high can the player can jump")]
    [SerializeField] private float jumpForce;
    [Tooltip("Cooldown between jumps")]
    [SerializeField] private float jumpCooldown;
    [Tooltip("How long while after falling off a ledge can the player jump")]
    [SerializeField] private float kyoteTime;
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

    /// <summary>
    /// Check how long its been since jumping
    /// </summary>
    private ScaledTimer midAirTimer;

    private ScaledTimer kyoteTracker;

    private bool kyoteTimeActive = false;

    [Header("---Gravity and Ground---")]

    [Tooltip("Strength of the gravity")]
    [SerializeField] private float gravityMultiplier;
    [Tooltip("Physics layers can the player walk on")]
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Transform at the bottom of this character object")]
    [SerializeField] private Transform groundCheck;
    [Tooltip("Radius to check the ground for")]
    [SerializeField] float groundCheckRadius = 0.3f;

    [Header("---Slope---")]

    [Tooltip("How far down to check for a slope ")]
    [SerializeField] private float slopeHitDist;
    [Tooltip("Maximum angle of a slope the player can climb")]
    [SerializeField] private float maxAngle;
    [Tooltip("Force pushed onto player on slopes to help stick to them")]
    [SerializeField] private float slopeStickForce = 80f;

    /// <summary>
    /// The last time a slope was detected
    /// </summary>
    private RaycastHit slopeHit;
    /// <summary>
    /// the last angle that was detected
    /// </summary>
    private float lastSurfaceAngle;

    #endregion

    //public bool grounded;

    #region Combat Stuff

    [Tooltip("Ability to activate when pressing 'Q'")]
    [SerializeField] private Ability QAbility;
    // private bool qAbilityHold = false;

    #endregion

    #region Initialization

    /// <summary>
    /// Initialize inputs and starting variables
    /// </summary>
    private void Awake()
    {
        // Initialize upgradable variables
        maxMoveSpeed.Initialize();
        sprintModifier.Initialize();
        airModifier.Initialize();
        jumps.Initialize();

        // Initialize internal variables
        jumpTimer = new ScaledTimer(jumpCooldown, false);
        midAirTimer = new ScaledTimer(0.5f, false);
        kyoteTracker = new ScaledTimer(kyoteTime, false);
        currentJumps = jumps.Current;
        targetMaxSpeed = maxMoveSpeed.Current;

        source = GetComponent<AudioSource>();        
    }

    /// <summary>
    /// Get any references needed, perform other starting functionality
    /// </summary>
    private void Start()
    {
        // Initialize controls
        controller = GameManager.controls;
        move = controller.PlayerGameplay.Move;
        move.Enable();

        jump = controller.PlayerGameplay.Jump;
        jump.performed += Jump;
        jump.Enable();

        sprint = controller.PlayerGameplay.Sprint;
        sprint.started += ToggleSprint;
        sprint.canceled += ToggleSprint;
        sprint.Enable();

        qInput = controller.PlayerGameplay.Tactical;
        qInput.performed += ActivateQAbility;
        qInput.Enable();

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
        // Get current inputDirection based on player input
        inputDirection = move.ReadValue<Vector2>();
        inputDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
    }

    private void FixedUpdate()
    {
        // Perform state-based update functionality
        UpdateStateFunction();
        MoveSpeedThrottle();

        //if(qAbilityHold && !qInput.IsInProgress())
        //{
        //    QAbility.Cancel();
        //    qAbilityHold = false;
        //}
    }


    #region State Functionality

    /// <summary>
    /// Perform update functionality based on current state
    /// </summary>
    private void UpdateStateFunction()
    {
        velocity = rb.velocity;

        switch (currentState)
        {
            case PlayerState.GROUNDED:
                {
                    HorizontalMovement();

                    if(currentJumps != jumps.Current)
                        currentJumps = jumps.Current;

                    // If not on ground, set state to midair. Disable sprint
                    if (!Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask) && !kyoteTimeActive)
                    {
                        //ChangeState(PlayerState.MIDAIR);
                        Debug.Log("Kyote time started!");
                        kyoteTimeActive = true;
                        kyoteTracker.ResetTimer();
                    }
                    else if(!Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask) && kyoteTracker.TimerDone())
                    {
                        Debug.Log("Kyote time ended!");
                        kyoteTimeActive = false;
                        kyoteTracker.ResetTimer();
                        ChangeState(PlayerState.MIDAIR);
                    }
                    else if (kyoteTimeActive && Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask))
                    {
                        kyoteTimeActive = false;
                        Debug.Log("Kyote time canceled!");
                    }

                    if (sprintHeld)
                        ChangeState(PlayerState.SPRINTING);

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

                    if (!sprintHeld)
                        ChangeState(PlayerState.GROUNDED);

                    break;
                }
            case PlayerState.MIDAIR:
                {
                    HorizontalMovement();

                    // If player lands and is moving downward, move back to grounded state
                    if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask) 
                        && rb.velocity.y <= 0 && jumpTimer.TimerDone())
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
                    // Failsafe - set gravity on
                    rb.useGravity = false;

                    // When entering grounded state, reset jumps
                    currentJumps = jumps.Current;

                    // When entering grounded state, reset target max speed
                    targetMaxSpeed = maxMoveSpeed.Current;

                    if(currentState == PlayerState.MIDAIR && source != null && landSound != null)
                    {
                        source.PlayOneShot(landSound, 0.5f);
                    }

                    velocity.y = 0;
                    rb.velocity = velocity;

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
                    // Failsafe - set gravity on
                    rb.useGravity = true;

                    midAirTimer.ResetTimer();

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
        if (ctx.started)
        {
            sprintHeld = true;

            ChangeState(PlayerState.SPRINTING);
        }
        // If player is sprinting and canceled input, stop sprinting
        else if (ctx.canceled)
        {
            sprintHeld = false;
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
        // On slope, project the velocity and make sure the player sticks to it while moving
        if (OnSlope() && currentState != PlayerState.MIDAIR)
        {
            rb.AddForce(SlopedVector() * accelerationSpeed, ForceMode.Force);
            //Debug.Log("Applying slopped force");

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * slopeStickForce, ForceMode.Force);
        }

        // if not midair or slopped, then just use normal grounded force
        else if (currentState != PlayerState.MIDAIR)
        {
            rb.AddForce(inputDirection.normalized * accelerationSpeed, ForceMode.Force);
            //Debug.Log("Applying ground force");
        }
           

        // if midair, then add air modifier 
        else if (currentState == PlayerState.MIDAIR)
        {
            //Debug.Log("Applying midair force");
            rb.AddForce(inputDirection.normalized * accelerationSpeed * airModifier.Current, ForceMode.Force);
        }
            
        // determine when drag should be applied 
        if (currentState != PlayerState.MIDAIR)
            rb.drag = drag;
        else
            rb.drag = 0;

        // dont allow gravity on slopes 
        rb.useGravity = !OnSlope();
    }

    /// <summary>
    /// Limit the current move speed by the set maximum. Dynamic between slope and ground
    /// </summary>
    private void MoveSpeedThrottle()
    {
        if(OnSlope() && currentState != PlayerState.MIDAIR)
        {
            if(rb.velocity.magnitude > targetMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * targetMaxSpeed;
            }
        }
        else
        {
            Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if(horizontalVel.magnitude > targetMaxSpeed) 
            {
                Vector3 throttledSpeed = horizontalVel.normalized * targetMaxSpeed;
                rb.velocity = new Vector3(throttledSpeed.x, rb.velocity.y, throttledSpeed.z);
            }
        }
    }

    /// <summary>
    /// Get the current input vector adjusted for slope
    /// </summary>
    /// <returns>Movement vector adjusted for slope</returns>
    private Vector3 SlopedVector()
    {
        return Vector3.ProjectOnPlane(inputDirection, slopeHit.normal).normalized;
    }

    public float angleTheshold;

    /// <summary>
    /// Whether the player is currently on a slope
    /// </summary>
    /// <returns>Whether or not the player is on a slope</returns>
    private bool OnSlope()
    {
        Vector3 origin = groundCheck.position;

        // Determine where to check. Predictive on ground, standard mid air
        //if (currentState != PlayerState.MIDAIR)
        //    origin = groundCheck.position + rb.velocity * 0.1f;
        //else
        //    origin = groundCheck.position;

        lastSurfaceAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

        Debug.DrawLine(origin, origin + Vector3.down * slopeHitDist, Color.red);
        if (Physics.Raycast(origin, Vector3.down, out slopeHit, slopeHitDist, groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (currentState != PlayerState.MIDAIR && 
                Mathf.Abs(lastSurfaceAngle - angle) >= angleTheshold)
            {
                Debug.Log($"Major angle shift detected of {Mathf.Abs(lastSurfaceAngle - angle)}!");
                RedirectVelocity();
            }

            return (angle != 0 && angle < maxAngle);
        }

        return false;
    }

    private void RedirectVelocity()
    {
        Vector3 velocity = rb.velocity;

        velocity = Vector3.ProjectOnPlane(velocity, slopeHit.normal);
        lastSurfaceAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

        rb.velocity= velocity;
    }

    /// <summary>
    /// Launch the player upwards, if possible
    /// </summary>
    /// <param name="c">Input callback context [ignorable]</param>
    private void Jump(InputAction.CallbackContext c)
    {
        if(currentJumps > 0 && jumpTimer.TimerDone())
        {
            ChangeState(PlayerState.MIDAIR);
            rb.drag = 0;

            midAirTimer.ResetTimer();

            // Adjust jumps, and reset any jumping cooldown
            jumpTimer.ResetTimer();
            currentJumps--;

            // Prepare new velocity
            Vector3 newVelocity = rb.velocity;

            // Redirect player velocity when jumping, if enabled
            if (jumpPivot)
            {
                // Calculate inputDirection velocity, set vertical velocity to 0
                Vector3 airDir = inputDirection * Mathf.Pow((targetMaxSpeed), 2) * Time.deltaTime;

                // Set new velocity to inputDirection
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

    #endregion

    #region Combat and Ability Calls

    private void ActivateQAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && QAbility.IsReady())
        {
            QAbility.Activate();
        }

        /*
        if(ctx.performed && !qAbilityHold)
        {
            QAbility.OnHold();
            qAbilityHold = true;
        }
        else if(ctx.performed && qAbilityHold)
        {
            QAbility.Activate();
            qAbilityHold = false;
        }
        */
    }

    #endregion

    #region Getters & Setters

    /// <summary>
    /// get # of jumps
    /// </summary>
    public UpgradableInt GetJumps() {
      return jumps;
    }

    public void RefreshJumps() {
      currentJumps = jumps.Current;
    }

    public void SetMoveSpeed(float speedMultiplier)
    {
        float temp = maxMoveSpeed.Current * speedMultiplier;
        maxMoveSpeed.ChangeVal(temp);
    }

    #endregion

    #region Misc


    /// <summary>
    /// Disable inputs to prevent crashing
    /// </summary>
    private void OnDisable()
    {
        jump.performed -= Jump;
        sprint.started -= ToggleSprint;
        sprint.canceled -= ToggleSprint;
        qInput.performed-= ActivateQAbility;

        instance = null;
    }

    #endregion
}
