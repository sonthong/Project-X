using UnityEngine;

namespace Platformer {
    public class JumpState : BaseState {
        public JumpState(PlayerController player) : base(player) { }

        public override void OnEnter() {
            Debug.Log("Enter jumpState");

        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}