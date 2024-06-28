using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;
using System.Collections;


namespace Platformer {
    [CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2> Stop = delegate { };
        public event UnityAction<Vector2> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };

        PlayerInputActions _inputActions;
        
        public Vector2 TouchPosition => _inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable() {
            if (_inputActions == null) {
                _inputActions = new PlayerInputActions();
                _inputActions.Player.SetCallbacks(this);
            }
        }
        
        public void EnablePlayerActions() {
            _inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            
        }

        public void OnDash(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
            }
        }

        public void OnTouchPress(InputAction.CallbackContext context)
        {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Move.Invoke(TouchPosition);
                    break;
                case InputActionPhase.Canceled:
                    Stop.Invoke(TouchPosition);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Jump.Invoke(context.ReadValue<Vector2>());
                    break;
                case InputActionPhase.Canceled:
                    Stop.Invoke(TouchPosition);
                    break;
            }
        }
    }
}