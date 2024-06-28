using UnityEngine;

namespace Platformer {
    public class IdleState : BaseState {
        public IdleState(PlayerController player) : base(player) { }
        
        public override void OnEnter() {
            Debug.Log("Enter idleState");
        }
        
        public override void FixedUpdate() {
        }
    }
}