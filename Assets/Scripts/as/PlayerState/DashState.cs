using UnityEngine;

namespace Platformer {
    public class DashState : BaseState {
        public DashState(PlayerController player) : base(player) { }

        public override void OnExit() {
            Player.rb.velocity = new Vector2(0, 0);
        }

        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}