using UnityEngine;

namespace Source.Puzzle
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private KeySO _keyData;

        private bool _unlocked;

        private Collider _colliderComponent;
        private MeshRenderer _meshRendererComponent;
        private AudioSource _audioSourceComponent;

        private void Awake()
        {
            _colliderComponent = GetComponent<Collider>();
            _meshRendererComponent = GetComponent<MeshRenderer>();
            _audioSourceComponent = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            GameEvents.Unlock += OnUnlock;
        }

        private void OnDisable()
        {
            GameEvents.Unlock -= OnUnlock;
        }

        // Invoked via SendMessage from PlayerController when the player's
        // CharacterController pushes against this door.
        private void OnPlayerCollision()
        {
            // A locked door stays solid until its matching key is collected.
            if (!_unlocked)
                return;

            // "Open" the door: stop blocking the player and hide the mesh.
            _colliderComponent.enabled = false;
            _meshRendererComponent.enabled = false;

            // Only the collider and mesh are disabled above, so the source
            // stays active and the open sound plays through to the end.
            _audioSourceComponent.Play();
        }

        private void OnUnlock(KeySO keyData)
        {
            // Every door hears every pickup; only the key matching this
            // door's data unlocks it.
            if (keyData != _keyData)
                return;

            _unlocked = true;
        }
    }
}
