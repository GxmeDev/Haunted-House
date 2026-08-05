using UnityEngine;

namespace Source.Player
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _characterControllerComponent;
        private Vector3 _startPosition;

        private void Awake()
        {
            _characterControllerComponent = GetComponent<CharacterController>();

            // Remember where the player began so we can respawn them there later.
            _startPosition = transform.position;
        }

        private void OnEnable()
        {
            GameEvents.Caught += OnCaught;
            GameEvents.FadeInComplete += OnFadeInComplete;
            GameEvents.FadeScreenReset += OnFadeScreenReset;
        }

        private void OnDisable()
        {
            GameEvents.Caught -= OnCaught;
            GameEvents.FadeInComplete -= OnFadeInComplete;
            GameEvents.FadeScreenReset -= OnFadeScreenReset;
        }

        private void OnCaught()
        {
            // Disabling the CharacterController freezes the player in place — the
            // behavior graph's Move calls become no-ops once the player is caught.
            _characterControllerComponent.enabled = false;
        }

        private void OnFadeInComplete()
        {
            // Teleport the player back to the start. Safe to set transform.position
            // directly: the CharacterController was disabled by OnCaught, so it
            // won't override the teleport with its cached position.
            transform.position = _startPosition;
        }

        private void OnFadeScreenReset()
        {
            // The caught screen is gone and the player has respawned — undo the
            // freeze from OnCaught so the behavior graph can move them again.
            _characterControllerComponent.enabled = true;
        }
    }
}
