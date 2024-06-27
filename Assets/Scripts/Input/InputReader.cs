using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Platformer/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public Camera mainCamera;
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Dash = delegate { };

    public PlayerInputActions InputActions;
    
    public Vector2 mousePosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void OnEnable() {
        if (InputActions == null) {
            InputActions = new PlayerInputActions();
            InputActions.Player.SetCallbacks(this);
        }
    }
    
    public void EnablePlayerActions() {
        InputActions.Enable();
    }

    public void OnMove(InputAction.CallbackContext context) {
        Move.Invoke(context.ReadValue<Vector2>());
        GetMousePosition(context);
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

    public void OnJump(InputAction.CallbackContext context) {
        switch (context.phase) {
            case InputActionPhase.Started:
                Jump.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
        }
    }

    public Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        position.z = camera.nearClipPlane;
        return camera.ScreenToViewportPoint(position);
    }

    Vector2 GetMousePosition(InputAction.CallbackContext context)
    {
        return mousePosition = context.ReadValue<Vector2>();
    }
}