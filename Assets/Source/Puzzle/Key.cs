using UnityEngine;

namespace Source.Puzzle
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private KeySO _keyData;

        private Collider _colliderComponent;
        private MeshRenderer _meshRendererComponent;

        private void Awake()
        {
            _colliderComponent = GetComponent<Collider>();
            _meshRendererComponent = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            GameEvents.Caught += OnCaught;
        }

        private void OnDisable()
        {
            GameEvents.Caught -= OnCaught;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only the player can pick up keys; ignore enemies or props
            // drifting into the trigger.
            if (!other.CompareTag("Player"))
                return;

            GameEvents.RaiseUnlock(_keyData);

            // The key is collected: hide it and kill the trigger so it can't
            // be picked up a second time.
            _colliderComponent.enabled = false;
            _meshRendererComponent.enabled = false;
        }

        private void OnCaught()
        {
            // Being caught resets the level: the key reappears and can be
            // collected again.
            _colliderComponent.enabled = true;
            _meshRendererComponent.enabled = true;
        }
    }
}
