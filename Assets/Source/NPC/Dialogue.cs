using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.NPC
{
    public class Dialogue : MonoBehaviour
    {
        [SerializeField] private string _characterName;
        [SerializeField] private Color _characterNameColor;
        [SerializeField] private List<string> _dialogueText;

        private InputAction _interactAction;
        private bool _isPlayerInside;

        private void Awake()
        {
            _interactAction = InputSystem.actions.FindAction("Player/Interact");
        }

        private void OnEnable()
        {
            GameEvents.ExitDialogue += OnExitDialogue;
        }

        private void OnDisable()
        {
            GameEvents.ExitDialogue -= OnExitDialogue;

            // If this NPC is disabled or destroyed while the player is still
            // inside the trigger, the handler would dangle on the shared
            // Interact action. Unsubscribing when not subscribed is harmless.
            _interactAction.performed -= OnInteractPerformed;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Only the player can start a conversation; ignore enemies or
            // props drifting into the trigger.
            if (!other.CompareTag("Player"))
                return;

            _isPlayerInside = true;
            _interactAction.performed += OnInteractPerformed;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _isPlayerInside = false;
            _interactAction.performed -= OnInteractPerformed;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // One-shot: once dialogue starts, stop listening so mashing E can't
            // re-raise StartDialogue mid-conversation. Re-entering the trigger
            // re-subscribes.
            _interactAction.performed -= OnInteractPerformed;
            GameEvents.RaiseStartDialogue(_characterName, _characterNameColor, _dialogueText);
        }

        private void OnExitDialogue()
        {
            // Every NPC hears this global event; only the one the player is
            // actually standing next to should re-arm its Interact handler.
            if (!_isPlayerInside)
                return;

            _interactAction.performed += OnInteractPerformed;
        }
    }
}
