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
using static UnityEngine.UI.Image;

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
    /// Tracker for if the player can jump mid air
    /// </summary>
    private ScaledTimer kyoteTracker;
    /// <summary>
    /// whether kyote time is active
    /// </summary>
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
    private RaycastHit lastSlopeHit;
    [SerializeField] private bool grounded;

    /// <summary>
    /// the last angle that was detected
    /// </summary>
    private float lastSurfaceAngle;

    #endregion

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
        GetGroundData();

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

                    if (!grounded)
                        ChangeState(PlayerState.MIDAIR);

                    if (sprintHeld)
                        ChangeState(PlayerState.SPRINTING);

                    break;
                }
            case PlayerState.SPRINTING:
                {
                    HorizontalMovement();
                    // If not on ground, set state to midair. Disable sprint
                    if (!grounded)
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
                    if (grounded && rb.velocity.y <= 0 && jumpTimer.TimerDone())
                    {
                        ChangeState(PlayerState.GROUNDED);
                    }

                    // check if its time to remove the first jump after falling off a ledge and kyote time is done
                    if(disableFirstJumpOnFall && currentJumps == jumps.Current && kyoteTracker.TimerDone())
                    {
                        currentJumps--;
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

                    kyoteTracker.ResetTimer();


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
    /// Check the current ground status of the player
    /// </summary>
    private void GetGroundData()
    {
        RaycastHit temp = slopeHit;
        Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius, Color.red);
        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, groundCheckRadius, groundMask))
        {
            lastSlopeHit = temp;
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    /// <summary>
    /// Manage the player's horizontal movement
    /// </summary>
    private void HorizontalMovement()
    {

        // On slope, project the velocity and make sure the player sticks to it while moving
        if (OnSlope() && currentState != PlayerState.MIDAIR)
        {
            rb.AddForce(SlopedVector() * accelerationSpeed, ForceMode.Force);
            Debug.Log("Applying slopped force");

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * slopeStickForce, ForceMode.Force);
        }

        // if not midair or slopped, then just use normal grounded force
        else if (currentState != PlayerState.MIDAIR)
        {
            rb.AddForce(inputDirection.normalized * accelerationSpeed, ForceMode.Force);
            Debug.Log("Applying ground force");
        }
           

        // if midair, then add air modifier 
        else if (currentState == PlayerState.MIDAIR)
        {
            Debug.Log("Applying midair force");
            rb.AddForce(inputDirection.normalized * accelerationSpeed * airModifier.Current, ForceMode.Force);
        }

        rb.velocity = AdjustVelocityToSlope(rb.velocity);

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
        Vector3 temp = Vector3.ProjectOnPlane(inputDirection, slopeHit.normal).normalized;
        //Debug.DrawRay(transform.position, temp);
        return temp;
    }

    public float angleTheshold;

    /// <summary>
    /// Whether the player is currently on a slope
    /// </summary>
    /// <returns>Whether or not the player is on a slope</returns>
    private bool OnSlope()
    {
        Vector3 origin = groundCheck.position;

        //Determine where to check. Predictive on ground, standard mid air
        //if (currentState != PlayerState.MIDAIR)
        //    origin = groundCheck.position + rb.velocity * 0.1f;
        //else
        //    origin = groundCheck.position;
        
        if (grounded)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return (angle != 0 && angle < maxAngle && currentState != PlayerState.MIDAIR);
        }

        return false;
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        //return Vector3.ProjectOnPlane(velocity, slopeHit.normal);

        if(grounded)
        {
            //Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, slopeHit.normal);
            //Vector3 adjustedVelocity = slopeRotation * velocity;
            //Debug.DrawRay(groundCheck.position, adjustedVelocity);

            //if(adjustedVelocity.y < 0)
            //{
            //    return adjustedVelocity;
            //}

            return Vector3.ProjectOnPlane(velocity, slopeHit.normal);
        }
        return velocity;
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
