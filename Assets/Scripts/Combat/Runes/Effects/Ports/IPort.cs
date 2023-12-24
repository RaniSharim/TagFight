

namespace TagFighter.Effects
{
    using System.Collections.Generic;
    public interface IPort<out PortType>
    where PortType : EffectStepNode
    {
        public string DisplayName {
            get;
            set;
        }

        public System.Type InnerType {
            get;
        }

        public bool IsAllowedConnect(EffectStepNode node);

        public PortCapacity Capacity { get; }
        public bool TryConnect(EffectStepNode node);
        public bool TryDisconnect(EffectStepNode node);

        public IEnumerable<EffectStepNode> Connections();
    }

}