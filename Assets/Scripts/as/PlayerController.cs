using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;

namespace Platformer {
    public class PlayerController : ValidatedMonoBehaviour {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Anywhere] InputReader input;
        
        [Header("Movement Settings")]
        [SerializeField] bool isMovingLeft;
        [SerializeField] bool isMovingRight;
        [SerializeField] float initialSpeed = 6f;
        [SerializeField] float acceleration = 0.2f;
        [SerializeField] float maxSpeed = 10f;
        private float _currentSpeed;
        
        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float swipeRange = 50f;
        
        [Header("Dash Settings")]
        [SerializeField] float dashForce = 10f;
        [SerializeField] float dashDuration = 1f;
        [SerializeField] float dashCooldown = 2f;

        const float ZeroF = 0f;
        
        Transform _mainCam;
        float _screenWidth;
        Vector2 _currentTouchPosition;
        Vector2 _lastTouchPosition;

        float _jumpVelocity;
        float _dashVelocity = 1f;


        List<Timer> _timers;
        CountdownTimer _jumpTimer;
        CountdownTimer _jumpCooldownTimer;
        CountdownTimer _dashTimer;
        CountdownTimer _dashCooldownTimer;
        
        StateMachine _stateMachine;

        void Awake() {            
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
            At(idleState, moveState, new FuncPredicate(() => isMovingRight ||isMovingLeft));
            At(moveState, jumpState, new FuncPredicate(()  => !groundChecker.IsGrounded));
            At(jumpState, dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            Any(idleState, new FuncPredicate(ReturnToIdleState));

            // Set initial state
            _stateMachine.SetState(idleState);
        }

        bool ReturnToIdleState() {
            return groundChecker.IsGrounded 
                   && !_jumpTimer.IsRunning 
                   && !_dashTimer.IsRunning;
        }

        void SetupTimers() {
            _jumpTimer = new CountdownTimer(jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            _jumpTimer.OnTimerStart += () => _jumpVelocity = jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();

            _dashTimer = new CountdownTimer(dashDuration);
            _dashCooldownTimer = new CountdownTimer(dashCooldown);

            _dashTimer.OnTimerStart += () => _dashVelocity = dashForce;
            _dashTimer.OnTimerStop += () => {
                _dashVelocity = 1f;
                _dashCooldownTimer.Start();
            };


            _timers = new List<Timer>(4) {_jumpTimer, _jumpCooldownTimer, _dashTimer, _dashCooldownTimer};
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
            _currentTouchPosition = ScreenToWorld(_mainCam, touchPosition);
            if (_currentTouchPosition.x * _screenWidth > _screenWidth / 2)
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
            _lastTouchPosition = ScreenToWorld(_mainCam, touchPosition);
        }

        void SwipeDetection(){
            Vector2 Direction = _lastTouchPosition - _currentTouchPosition;
            if (distance.y > swipeRange && Mathf.Abs(distance.x) < swipeRange)
            {
                ResetTouch();
                HandleJump();
            }
        }

        void ResetTouch(){
            _lastTouchPosition = Vector2.Zero;
            _currentTouchPosition = Vector2.Zero;
        }

        void OnJump(bool performed) {
            if (performed && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning && groundChecker.IsGrounded) {
                _jumpTimer.Start();
            } else if (!performed && _jumpTimer.IsRunning) {
                _jumpTimer.Stop();
            }
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        public void HandleMovement() {
            if (movingRight || movingLeft)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Vector3 movement = Vector3.zero;
            if (isMovingRight)
            {
                movement = Vector3.right;
            }
            else if (isMovingLeft)
            {
                movement = Vector3.left;
            }

            rb.velocity = movement * currentSpeed;
        }
        }

    }

    void HandleDash(){
        if (isMovingRight)
        {
            Vector2 velocity = Vector2.right * dashVelocity * Time.fixedDeltaTime;
        }
        else{
            Vector2 velocity = Vector2.right * dashVelocity * Time.fixedDeltaTime;
        }
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
    }
}