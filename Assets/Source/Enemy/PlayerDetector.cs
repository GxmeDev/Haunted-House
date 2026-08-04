using UnityEngine;

namespace Source.Enemy
{
    public class PlayerDetector : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // Only the player should trip the detector; ignore other enemies,
            // props, or anything else wandering into the trigger.
            if (!other.CompareTag("Player"))
                return;

            Debug.Log($"{name} detected the player!");
        }
    }
}
