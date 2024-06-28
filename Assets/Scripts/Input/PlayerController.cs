using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using Platformer;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

public class PlayerController : ValidatedMonoBehaviour {
    [Header("References")]
    [SerializeField, Self] Rigidbody2D rb;
    [SerializeField, Self] GroundChecker groundChecker;
    [SerializeField, Anywhere] InputReader input;
    
    [Header("Movement Settings")]
    [SerializeField] bool isMoving;
    [SerializeField] bool isMovingLeft;
    [SerializeField] bool isMovingRight;
    [SerializeField] float initialSpeed = 2f;
    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float currentSpeed = 2f;
    [SerializeField] float delayTime = 0.5f;
    
    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float swipeRange = 0.3f;
    
    [Header("Dash Settings")]
    [SerializeField] float dashForce = 10f;
    [SerializeField] float dashDuration = 1f;
    [SerializeField] float dashCooldown = 1f;

    const float ZeroF = 0f;
    
    [SerializeField, Anywhere] Camera mainCam;
    float _screenWidth;
    Vector2 _firstTouchPosition;
    Vector2 _lastTouchPosition;

    float _jumpVelocity;
    float _dashVelocity = 1f;


    List<Timer> _timers;
    CountdownTimer _delayTimer;
    CountdownTimer _dashTimer;
    CountdownTimer _dashCooldownTimer;
    
    StateMachine _stateMachine;

    void Awake()
    {
        SetupTimers();
        SetupStateMachine();
    }

    void SetupStateMachine() {
        // State Machine
        _stateMachine = new StateMachine();

        // Declare states
        var idleState = new IdleState(this);
        var jumpState = new JumpState(this);
        var dashState = new DashState(this);
        var moveState = new MoveState(this);

        // Define transitions
        At(idleState, jumpState, new FuncPredicate(() => !groundChecker.IsGrounded));
        At(idleState, moveState, new FuncPredicate(() => isMoving));
        At(moveState, jumpState, new FuncPredicate(()  => !groundChecker.IsGrounded));
        At(jumpState, dashState, new FuncPredicate(() => _dashTimer.IsRunning));
        At(jumpState, moveState, new FuncPredicate(() => isMoving));
        Any(idleState, new FuncPredicate(ReturnToIdleState));

        // Set initial state
        _stateMachine.SetState(idleState);
    }

    bool ReturnToIdleState() {
        return groundChecker.IsGrounded 
               && !isMoving
               && !_dashTimer.IsRunning;
    }

    void SetupTimers() {
        _delayTimer = new CountdownTimer(delayTime);

        _dashTimer = new CountdownTimer(dashDuration);
        _dashCooldownTimer = new CountdownTimer(dashCooldown);

        _dashTimer.OnTimerStart += () => _dashVelocity = dashForce;
        _dashTimer.OnTimerStop += () => {
            _dashVelocity = 1f;
            _dashCooldownTimer.Start();
        };


        _timers = new List<Timer>(3) {_delayTimer, _dashTimer, _dashCooldownTimer};
    }

    void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    void Start(){
        _screenWidth = Screen.width;
        input.EnablePlayerActions();
    }
    void OnEnable() {
        input.Jump += OnJump;
        input.Dash += OnDash;
        input.Move += OnMove;
        input.Stop += OnStop;
    }
    
    void OnDisable() {
        input.Jump -= OnJump;
        input.Dash -= OnDash;
        input.Move -= OnMove;
        input.Stop -= OnStop;
    }

    void OnMove(Vector2 touchPosition)
    {
        _delayTimer.Start();
        _firstTouchPosition = Convert.ScreenToWorld(mainCam, touchPosition);
        Debug.Log(_firstTouchPosition);
        isMoving = true;
        if (_firstTouchPosition.x * _screenWidth > _screenWidth / 2)
        {
            isMovingRight = true;
            isMovingLeft = false;
        }
        else
        {
            isMovingLeft = true;
            isMovingRight = false;
        }
    }

    void OnStop(Vector2 touchPosition)
    {
        _delayTimer.Stop();
        isMoving = false;
        _lastTouchPosition = Convert.ScreenToWorld(mainCam, touchPosition);
        Debug.Log(_lastTouchPosition);
        SwipeDetection();
        currentSpeed = initialSpeed;
    }

    void SwipeDetection(){
        Vector2 direction = _lastTouchPosition - _firstTouchPosition;
        Debug.Log("direction" + direction);
        if (direction.y > swipeRange)
        {
            ResetTouch();
            HandleJump();
        }
    }

    void ResetTouch(){
        _lastTouchPosition = Vector2.zero;
        _firstTouchPosition = Vector2.zero;
    }

    void OnJump(Vector2 touchPosition)
    {
        _firstTouchPosition = Convert.ScreenToWorld(mainCam, touchPosition);
    }
    
    void OnDash(bool performed) {
        if (performed && !_dashTimer.IsRunning && !_dashCooldownTimer.IsRunning) {
            _dashTimer.Start();
        } else if (!performed && _dashTimer.IsRunning) {
            _dashTimer.Stop();
        }
    }
    
    

    void Update() {
        _stateMachine.Update();

        HandleTimers();
    }

    void FixedUpdate() {
        _stateMachine.FixedUpdate();
    }

    void HandleTimers() {
        foreach (var timer in _timers) {
            timer.Tick(Time.deltaTime);
        }
    }

    public void HandleJump() {
        if (groundChecker.IsGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
    }

    public void HandleMovement() 
    {
        if (isMoving && !_delayTimer.IsRunning)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Vector2 movement = Vector2.zero;
            if (isMovingRight)
            {
                movement = Vector2.right;
            }
            else if (isMovingLeft)
            {
                movement = Vector2.left;
            }

            rb.velocity = new Vector2(movement.x * currentSpeed, rb.velocity.y);
        }
    }

    public void HandleDash()
    {
        if (isMovingRight)
        {
            rb.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
        }
        else{
            rb.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);
        }
    }


    public void ResetVelocity()
    {
        rb.velocity = Vector2.zero;
    }
}