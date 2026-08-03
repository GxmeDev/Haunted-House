using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Source.Player
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Walk", story: "Moves gameobject at a speed of [Speed] and rotation of [RotationSpeed]", category: "Action", id: "39fca2f4d47686bbad09a603eb0ba48d")]
    public partial class WalkAction : Action
    {
        [SerializeReference] public BlackboardVariable<float> Speed = new(2f);
        [SerializeReference] public BlackboardVariable<float> RotationSpeed = new(500f);

        private BehaviorInputReader _inputReader;
        private CharacterController _characterController;

        protected override Status OnStart()
        {
            _inputReader = GameObject.GetComponent<BehaviorInputReader>();
            _characterController = GameObject.GetComponent<CharacterController>();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector2 input = _inputReader.MoveDirection;

            if (input == Vector2.zero)
            {
                return Status.Success;
            }

            Vector3 direction = new(input.x, 0f, input.y);
            _characterController.Move(direction * (Speed * Time.deltaTime));

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            GameObject.transform.rotation = Quaternion.RotateTowards(
                GameObject.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

            return Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}
