using TagFighter.Dialogue;
using UnityEngine;
// using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

public class DialogueNodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<DialogueNodeView> OnNodeSelected;
    public DialogueNode Node;
    public Port Input;
    public Port Output;

    public DialogueNodeView(DialogueNode node) {
        Node = node;
        title = Node.name;
        viewDataKey = Node.name;
        name = Node.name;

        style.left = Node.Position.x;
        style.top = Node.Position.y;

        CreateTextField();
        CreateInputPorts();
        CreateOutputPorts();
    }

    void CreateTextField() {
        var textField = new TextField {
            value = Node.Text
        };
        textField.RegisterValueChangedCallback(OnDialogueNodeTextChanged);
        textField.RegisterCallback<FocusInEvent>(OnTextFieldFocusIn);
        mainContainer.Add(textField);
    }


    void OnTextFieldFocusIn(FocusInEvent e) {
        OnSelected();
    }

    void OnDialogueNodeTextChanged(ChangeEvent<string> e) {
        Node.Text = e.newValue;
    }

    void CreateInputPorts() {
        Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));

        if (Input != null) {
            Input.portName = "";
            inputContainer.Add(Input);
        }
    }

    void CreateOutputPorts() {
        Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

        if (Output != null) {
            Output.portName = "";
            outputContainer.Add(Output);
        }
    }


    public override void SetPosition(Rect newPosition) {
        base.SetPosition(newPosition);
        Node.Position = newPosition.position;
    }

    public override void OnSelected() {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
}
