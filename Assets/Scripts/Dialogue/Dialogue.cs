using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TagFighter.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> _nodes = new();

        [SerializeField]
        Vector2 _newNodeOffset = new(250, 0);

        public List<DialogueNode> Nodes {
            get { return _nodes; }
        }

#if UNITY_EDITOR        

        public DialogueNode CreateNode(DialogueNode parent) {
            var node = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(node, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(node);
            return node;
        }

        void AddNode(DialogueNode node) {
            _nodes.Add(node);
        }

        DialogueNode MakeNode(DialogueNode parent) {
            var node = CreateInstance<DialogueNode>();
            node.name = Guid.NewGuid().ToString();
            if (parent != null) {
                node.Position = parent.Position + _newNodeOffset;
                node.IsPlayerSpeaking = !parent.IsPlayerSpeaking;

                parent.AddChild(node.name);
            }

            return node;
        }

        public void DeleteNode(DialogueNode nodeToDelete) {
            Undo.RegisterCreatedObjectUndo(this, "Remove Dialogue Node");
            _nodes.Remove(nodeToDelete);
            foreach (var node in _nodes) {
                node.RemoveChild(nodeToDelete.name);
            }
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
#endif

        public void OnBeforeSerialize() {
#if UNITY_EDITOR
            if (AssetDatabase.GetAssetPath(this) != "") {
                foreach (var node in _nodes.Where(node => AssetDatabase.GetAssetPath(node) == "")) {
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
#endif
        }

        public void OnAfterDeserialize() {
        }
    }
}