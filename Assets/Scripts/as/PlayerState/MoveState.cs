using UnityEngine;

namespace Platformer {
    public class MoveState : BaseState {
        public MoveState(PlayerController player) : base(player) { }

        public override void OnEnter() {
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}