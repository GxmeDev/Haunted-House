using UnityEngine;
using UnityEngine.InputSystem;

namespace Source.Player
{
    public class BehaviorInputReader : MonoBehaviour
    {
        public Vector2 MoveDirection { get; private set; }

        private InputAction _moveAction;

        private void OnEnable()
        {
            _moveAction = InputSystem.actions.FindAction("Player/Move");
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
            GameEvents.Caught += OnCaught;
            GameEvents.FadeScreenReset += OnFadeScreenReset;
        }

        private void OnDisable()
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
            GameEvents.Caught -= OnCaught;
            GameEvents.FadeScreenReset -= OnFadeScreenReset;
        }

        private void OnCaught()
        {
            // Once caught, clear the current input and stop listening to the Move
            // action so the behavior graph no longer sees any movement input.
            MoveDirection = Vector2.zero;
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
        }

        private void OnFadeScreenReset()
        {
            // The respawn finished — start listening to the Move action again so
            // the player regains control (OnCaught removed these handlers).
            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveDirection = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveDirection = Vector2.zero;
        }
    }
}
