using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using TagFighter.Dialogue;
using System.Linq;
using System.Collections.Generic;
using System;

public class DialogueView : GraphView
{
    Dialogue _dialogue;
    public Action<DialogueNodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<DialogueView, GraphView.UxmlTraits> { }
    public DialogueView() {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Dialogue/DialogueEditor.uss");
        styleSheets.Add(styleSheet);
    }


    internal void PopulateView(Dialogue dialogueCandidate) {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);

        _dialogue = dialogueCandidate;

        if (_dialogue == null) {
            return;
        }

        graphViewChanged += OnGraphViewChanged;
        CreateNodeViews();
        CreateEdges();
    }

    void CreateNodeViews() {
        foreach (var node in _dialogue.Nodes) {
            CreateNodeView(node);
        }
    }

    void CreateEdges() {
        foreach (var parentNode in _dialogue.Nodes) {
            var parentView = GetNodeByGuid(parentNode.name) as DialogueNodeView;
            foreach (var child in parentNode.GetChildren()) {
                var childView = GetNodeByGuid(child) as DialogueNodeView;
                var edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);
            }
        }
    }

    GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
        if (graphViewChange.elementsToRemove != null) {
            foreach (var elementToRemove in graphViewChange.elementsToRemove) {
                switch (elementToRemove) {
                    case DialogueNodeView dialogueNodeView:
                        _dialogue.DeleteNode(dialogueNodeView.Node);
                        break;
                    case Edge edge:
                        var parent = edge.output.node as DialogueNodeView;
                        var child = edge.input.node as DialogueNodeView;
                        parent.Node.RemoveChild(child.viewDataKey);
                        break;
                }
            }
        }

        if (graphViewChange.edgesToCreate != null) {
            foreach (var edge in graphViewChange.edgesToCreate) {
                var parent = edge.output.node as DialogueNodeView;
                var child = edge.input.node as DialogueNodeView;
                parent.Node.AddChild(child.viewDataKey);
            }
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent e) {
        // base.BuildContextualMenu(e);
        {
            e.menu.AppendAction($"Create DialogueNode", action => CreateNode(action.eventInfo.mousePosition));
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        return ports.Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    void CreateNode(UnityEngine.Vector2 position) {
        if (_dialogue == null) {
            return;
        }

        var node = _dialogue.CreateNode(null);
        node.Position = viewTransform.matrix.inverse.MultiplyPoint(this.WorldToLocal(position));
        CreateNodeView(node);
    }

    void CreateNodeView(DialogueNode node) {
        DialogueNodeView nodeView = new(node) {
            OnNodeSelected = OnNodeSelected,
        };
        AddElement(nodeView);
    }
}
