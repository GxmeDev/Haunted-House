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
        private BehaviorInputReader _inputReader;

        protected override Status OnStart()
        {
            _inputReader = GameObject.GetComponent<BehaviorInputReader>();
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            return _inputReader.MoveDirection != Vector2.zero ? Status.Success : Status.Running;
        }

        protected override void OnEnd()
        {
        }
    }
}
