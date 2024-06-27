using UnityEngine;

namespace Platformer {
    public abstract class BaseState : IState {
        protected readonly PlayerController Player;
        
        protected BaseState(PlayerController player) {
            this.Player = player;
        }
        
        public virtual void OnEnter() {
            // noop
        }

        public virtual void Update() {
            // noop
        }

        public virtual void FixedUpdate() {
            // noop
        }

        public virtual void OnExit() {
            // noop
        }
    }
}