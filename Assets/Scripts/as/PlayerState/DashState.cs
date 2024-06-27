using UnityEngine;

namespace Platformer {
    public class DashState : BaseState {
        public DashState(PlayerController player) : base(player) { }

        public override void OnEnter() {
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}