using UnityEngine;

namespace Platformer {
    public class MoveState : BaseState {
        public MoveState(PlayerController player) : base(player) { }

        public override void OnEnter() {
            Debug.Log("Enter MoveState");
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}