
namespace TagFighter.UI.Editor
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using UnityEditor.Experimental.GraphView;
    using System;
    using TagFighter.Effects;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class RuneEffectView : GraphView
    {
        RuneEffect _runeEffect;
        public Action<EffectStepNodeView> OnNodeSelected;

        public RuneEffectView() {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Rune/RuneEditor.uss");
            styleSheets.Add(styleSheet);
        }

        internal void PopulateView(RuneEffect runeEffect) {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);

            _runeEffect = runeEffect;

            if (_runeEffect == null) {
                return;
            }

            graphViewChanged += OnGraphViewChanged;
            CreateNodeViews();
            CreateEdges();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            return ports.Where(
                endPort => endPort.direction != startPort.direction
                && endPort.node != startPort.node
                && endPort.portType == startPort.portType
                ).ToList();
        }

        void CreateNodeViews() {
            foreach (var node in _runeEffect.Nodes) {
                CreateNodeView(node);
            }
        }

        void CreateEdges() {
            foreach (var inputNode in _runeEffect.Nodes) {
                var inputView = GetNodeByGuid(inputNode.Guid) as EffectStepNodeView;

                foreach (var inputPort in inputNode.Inputs) {
                    foreach (var outputNode in inputPort.Connections()) {
                        var outputView = GetNodeByGuid(outputNode.Guid) as EffectStepNodeView;
                        var onputPortView = outputView.outputContainer.Children().FirstOrDefault(portView => portView is Port) as Port;
                        var inputPortView = inputView.inputContainer.Children().FirstOrDefault(portView => portView.userData == inputPort) as Port;
                        var edge = onputPortView.ConnectTo(inputPortView);
                        AddElement(edge);
                    }
                }
            }
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
            if (graphViewChange.elementsToRemove != null) {
                foreach (var elementToRemove in graphViewChange.elementsToRemove) {
                    switch (elementToRemove) {
                        case EffectStepNodeView effectStepNodeView:
                            _runeEffect.DeleteNode(effectStepNodeView.Node);
                            AssetDatabase.RemoveObjectFromAsset(effectStepNodeView.Node);
                            break;
                        case Edge edge:
                            var outputNodeView = edge.output.node as EffectStepNodeView;
                            var inputPortView = edge.input.userData as IPort<EffectStepNode>;
                            inputPortView?.TryDisconnect(outputNodeView.Node);
                            break;
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null) {
                foreach (var edge in graphViewChange.edgesToCreate) {
                    var outputNodeView = edge.output.node as EffectStepNodeView;
                    var inputPortView = edge.input.userData as IPort<EffectStepNode>;
                    inputPortView?.TryConnect(outputNodeView.Node);
                }
            }
            return graphViewChange;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent e) {
            // base.BuildContextualMenu(e);
            {
                foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(IGetterStep))) {
                    e.menu.AppendAction($"Get/{type.Name}", action => AddNode(type, action.eventInfo.mousePosition));
                }
                foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(IOperatorStep))) {
                    e.menu.AppendAction($"Operator/{type.Name}", action => AddNode(type, action.eventInfo.mousePosition));
                }

                foreach (var type in TypeCache.GetTypesDerivedFrom(typeof(ISetterStep))) {
                    e.menu.AppendAction($"Set/{type.Name}", action => AddNode(type, action.eventInfo.mousePosition));
                }

                e.menu.AppendAction("Root Node", action => AddNode(typeof(EffectEndNode), action.eventInfo.mousePosition));
            }
        }

        UnityEngine.Vector2 CalculatePositionOnGraph(UnityEngine.Vector2 position) {
            return viewTransform.matrix.inverse.MultiplyPoint(this.WorldToLocal(position));
        }

        void AddNode(System.Type type, UnityEngine.Vector2 position) {
            if (_runeEffect == null) {
                return;
            }
            var node = CreateNode(type, position);
            _runeEffect.AddNode(node);

            AssetDatabase.AddObjectToAsset(node, _runeEffect);

            CreateNodeView(node);
        }

        EffectStepNode CreateNode(System.Type type, UnityEngine.Vector2 position) {
            if (_runeEffect == null) {
                return null;
            }

            var node = ScriptableObject.CreateInstance(type) as EffectStepNode;
            node.name = type.Name;
            node.Guid = Guid.NewGuid().ToString();
            node.Position = CalculatePositionOnGraph(position);

            return node;
        }
        void CreateNodeView(EffectStepNode node) {
            EffectStepNodeView nodeView = new(node) {
                OnNodeSelected = OnNodeSelected,
            };
            AddElement(nodeView);
        }
    }
}