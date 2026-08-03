using UnityEngine;
using UnityEngine.InputSystem;

namespace StealthGame
{
    public class BehaviorInputReader : MonoBehaviour
    {
        public InputAction MoveAction;

        Vector2 m_MoveInput;

        public Vector2 MoveInput => m_MoveInput;

        void OnEnable()
        {
            MoveAction.Enable();
        }

        void OnDisable()
        {
            MoveAction.Disable();
        }

        void Update()
        {
            m_MoveInput = MoveAction.ReadValue<Vector2>();
        }
    }
}
