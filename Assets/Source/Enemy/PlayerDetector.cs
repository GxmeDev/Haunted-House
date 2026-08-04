using UnityEngine;

namespace Source.Enemy
{
    public class PlayerDetector : MonoBehaviour
    {
        // Tracks whether the player was visible on the previous sight check so
        // the detection log fires once per sighting instead of every physics step.
        private bool _isPlayerVisible;

        // The enemy body's solid collider on the parent object; the sight line
        // is shot from its center rather than this field-of-view child's pivot.
        private Collider _parentColliderComponent;

        private void Awake()
        {
            // GetComponentInParent would find this object's own trigger capsule
            // first, so ask the parent directly for the enemy's body collider.
            _parentColliderComponent = transform.parent.GetComponent<Collider>();
        }

        private void OnTriggerStay(Collider other)
        {
            // Only the player should trip the detector; ignore other enemies,
            // props, or anything else wandering into the trigger.
            if (!other.CompareTag("Player"))
                return;

            if (!HasLineOfSight(other))
            {
                _isPlayerVisible = false;
                return;
            }

            // Already spotted during this sighting; don't spam the console.
            if (_isPlayerVisible)
                return;

            _isPlayerVisible = true;
            Debug.Log($"{name} detected the player!");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            // Reset so the player is logged again on their next sighting.
            _isPlayerVisible = false;
        }

        private bool HasLineOfSight(Collider playerCollider)
        {
            Vector3 origin = _parentColliderComponent.bounds.center;
            Vector3 target = playerCollider.bounds.center;

            // Ignore trigger colliders (including this detector's own field-of-view
            // capsule) so only solid geometry can block or register the sight line.
            bool hitSomething = Physics.Linecast(origin, target, out RaycastHit hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            // Sight is clear if nothing solid is on the line, or if the first solid
            // thing hit is the player themselves; anything else (wall, prop) blocks.
            // The enemy's own capsule can't self-block: the line starts inside it,
            // and physics queries skip colliders a ray starts within.
            bool canSeePlayer = !hitSomething || hit.collider == playerCollider;

            // Debug visual in the Scene view: green when the player is visible, red
            // when blocked. Persist for one physics step so the line doesn't flicker
            // on render frames between FixedUpdate ticks.
            Debug.DrawLine(origin, target, canSeePlayer ? Color.green : Color.red, Time.fixedDeltaTime);

            return canSeePlayer;
        }
    }
}
