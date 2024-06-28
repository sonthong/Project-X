using UnityEngine;

namespace Platformer {
    public class GroundChecker : MonoBehaviour {
        [SerializeField] float groundDistance = 0.08f;
        [SerializeField] LayerMask groundLayers;
        [SerializeField] Vector2 bottomOffset;

        public bool IsGrounded;

        void Update() {
            IsGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, groundDistance, groundLayers);
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var positions = new Vector2[] { bottomOffset };

            Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, groundDistance);
        }
    }
}