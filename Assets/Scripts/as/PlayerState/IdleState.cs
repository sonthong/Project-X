using UnityEngine;

namespace Platformer {
    public class IdleState : BaseState {
        public IdleState(PlayerController player) : base(player) { }
        
        public override void OnEnter() {
        }
        
        public override void FixedUpdate() {
            Player.HandleMovement();
        }
    }
}