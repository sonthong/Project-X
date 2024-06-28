using UnityEngine;

namespace Platformer {
    public class DashState : BaseState {
        public DashState(PlayerController player) : base(player) { }

        public override void OnEnter() {
            Debug.Log("Enter dashState");
            Player.HandleDash();
        }
        public override void OnExit() {
            Player.ResetVelocity();
        }

        public override void FixedUpdate() {
            
            Player.HandleMovement();
        }
    }
}