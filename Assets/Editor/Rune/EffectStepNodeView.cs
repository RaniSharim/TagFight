
namespace TagFighter.UI.Editor
{
    using System;
    using TagFighter.Effects;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class EffectStepNodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<EffectStepNodeView> OnNodeSelected;
        public EffectStepNode Node;

        public EffectStepNodeView(EffectStepNode node) {
            Node = node;
            title = Node.ShortName;
            viewDataKey = Node.Guid;

            style.left = Node.Position.x;
            style.top = Node.Position.y;


            CreateInputPorts();
            CreateOutputPorts();
            SetClasses();
        }

        void SetClasses() {
            switch (Node) {
                case IOperatorStep:
                    AddToClassList("OperatorStep");
                    break;
                case IGetterStep:
                    AddToClassList("GetterStep");
                    break;
                case ISetterStep:
                    AddToClassList("SetterStep");
                    break;
                default:
                    break;
            }
        }

        string GetPortDisplayName(System.Type portType) {
            if (portType == typeof(SequenceGetterStep)) {
                return "[]";
            }
            else if (portType == typeof(SingleGetterStep)) {
                return "";
            }
            else if (portType == typeof(SetterNode)) {
                return "()";
            }
            return null;
        }
        void CreateInputPorts() {
            var direction = Direction.Input;
            const string InPortDisplayName = "In";

            foreach (var port in Node.Inputs) {
                var portView = port.Capacity switch {
                    PortCapacity.Single => InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, port.InnerType),
                    _ => InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Multi, port.InnerType)
                };
                portView.portName = (port.DisplayName != null && port.DisplayName != "") ? port.DisplayName : InPortDisplayName;
                portView.portName += GetPortDisplayName(portView.portType);
                portView.userData = port;
                inputContainer.Add(portView);
            }
        }

        void CreateOutputPorts() {
            var direction = Direction.Output;
            var orientation = Orientation.Horizontal;
            const string OutPortDisplayName = "Out";

            var portView = Node switch {
                SequenceGetterStep => InstantiatePort(orientation, direction, Port.Capacity.Single, typeof(SequenceGetterStep)),
                SingleGetterStep => InstantiatePort(orientation, direction, Port.Capacity.Single, typeof(SingleGetterStep)),
                SetterNode => InstantiatePort(orientation, direction, Port.Capacity.Single, typeof(SetterNode)),
                _ => null,
            };

            if (portView != null) {
                portView.portName = OutPortDisplayName + GetPortDisplayName(portView.portType);
                outputContainer.Add(portView);
            }
        }

        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
        }
        public override void OnSelected() {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
    }
}
