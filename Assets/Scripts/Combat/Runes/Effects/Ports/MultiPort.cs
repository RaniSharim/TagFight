using System;
using System.Collections.Generic;

namespace TagFighter.Effects
{
    [Serializable]
    public class MultiPort<PortType> : IPort<PortType>
    where PortType : EffectStepNode
    {
        public List<PortType> Nodes = new();
        public Type InnerType => typeof(PortType);

        public PortCapacity Capacity => PortCapacity.Multi;

        public string DisplayName { get; set; }

        public IEnumerable<EffectStepNode> Connections() {
            return Nodes;
        }

        public bool IsAllowedConnect(EffectStepNode node) {
            return node is PortType;
        }

        public bool TryConnect(EffectStepNode node) {
            var success = false;
            if (IsAllowedConnect(node)) {
                Nodes.Add(node as PortType);
                success = true;
            }

            return success;
        }

        public bool TryDisconnect(EffectStepNode value) {
            var success = false;
            var inputNode = value as PortType;
            if (inputNode != null) {
                Nodes.Remove(inputNode);
                success = true;
            }

            return success;
        }
    }
}