using UnityEngine;

namespace Source.Player
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _characterControllerComponent;

        private void Awake()
        {
            _characterControllerComponent = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            GameEvents.Caught += OnCaught;
        }

        private void OnDisable()
        {
            GameEvents.Caught -= OnCaught;
        }

        private void OnCaught()
        {
            // Disabling the CharacterController freezes the player in place — the
            // behavior graph's Move calls become no-ops once the player is caught.
            _characterControllerComponent.enabled = false;
        }
    }
}
