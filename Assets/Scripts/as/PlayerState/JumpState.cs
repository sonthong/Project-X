using UnityEngine;

namespace Platformer {
    public class JumpState : BaseState {
        public JumpState(PlayerController player) : base(player) { }

        public override void OnEnter() {
        }

        public override void FixedUpdate() {
            Player.HandleJump();
            Player.HandleMovement();
        }
    }
}