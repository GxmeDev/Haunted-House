using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Source.Player
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "Wait For Move Input", story: "Waits for player movement", category: "Action", id: "8069ad62189b85511b87f1c041822c00")]
    public partial class WaitForMoveInputAction : Action
    {
        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");

        private BehaviorInputReader _inputReaderComponent;
        private Animator _animatorComponent;
        private AudioSource _audioSourceComponent;

        protected override void OnSetup()
        {
            _inputReaderComponent = GameObject.GetComponent<BehaviorInputReader>();
            _animatorComponent = GameObject.GetComponent<Animator>();
            _audioSourceComponent = GameObject.GetComponent<AudioSource>();
        }

        protected override Status OnStart()
        {
            _animatorComponent.SetBool(_isWalkingHash, false);
            _audioSourceComponent.Stop();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _inputReaderComponent.MoveDirection != Vector2.zero ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}
