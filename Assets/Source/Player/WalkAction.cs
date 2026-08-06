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

        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

        private BehaviorInputReader _inputReaderComponent;
        private CharacterController _characterControllerComponent;
        private Animator _animatorComponent;
        private AudioSource _audioSourceComponent;

        protected override void OnSetup()
        {
            _inputReaderComponent = GameObject.GetComponent<BehaviorInputReader>();
            _characterControllerComponent = GameObject.GetComponent<CharacterController>();
            _animatorComponent = GameObject.GetComponent<Animator>();
            _audioSourceComponent = GameObject.GetComponent<AudioSource>();
        }

        protected override Status OnStart()
        {
            _animatorComponent.SetBool(_isWalkingHash, true);
            _audioSourceComponent.Play();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            Vector2 input = _inputReaderComponent.MoveDirection;

            if (input == Vector2.zero)
            {
                return Status.Success;
            }

            Vector3 direction = new(input.x, 0f, input.y);
            _characterControllerComponent.Move(direction * (Speed * Time.deltaTime));

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
