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

        protected override Status OnStart()
        {
            _inputReader = GameObject.GetComponent<BehaviorInputReader>();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _inputReader.MoveDirection != Vector2.zero ? Status.Running : Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}
