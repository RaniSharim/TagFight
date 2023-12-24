// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

using System;
using System.Collections.Generic;

namespace TagFighter.Effects
{
    [Serializable]
    public class SinglePort<PortType> : IPort<PortType>
    where PortType : EffectStepNode
    {
        public PortType Node;

        public Type InnerType => typeof(PortType);

        public PortCapacity Capacity => PortCapacity.Single;

        public string DisplayName { get; set; }

        public IEnumerable<EffectStepNode> Connections() {
            if (Node != null) {
                yield return Node;
            }
        }

        public bool IsAllowedConnect(EffectStepNode node) {
            // UnityEngine.Debug.Log($"{node.GetType()} {typeof(PortType)} {node as PortType}");
            return node is PortType;
        }

        public bool TryConnect(EffectStepNode node) {
            var success = false;
            if (IsAllowedConnect(node)) {
                Node = node as PortType;
                success = true;
            }

            return success;
        }

        public bool TryDisconnect(EffectStepNode value) {
            var success = false;
            if (ReferenceEquals(Node, value)) {
                Node = null;
                success = true;
            }
            return success;
        }
    }
}