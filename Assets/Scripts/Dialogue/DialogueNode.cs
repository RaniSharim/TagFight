using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TagFighter.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        bool _isPlayerSpeaking = false;
        [SerializeField]
        string _text = new("");
        [SerializeField]
        List<string> _children = new();
        [SerializeField]
        Rect _rect = new(0, 0, 200, 100);

#if UNITY_EDITOR
        public bool IsPlayerSpeaking {
            get { return _isPlayerSpeaking; }
            set {
                Undo.RecordObject(this, "Change Dialogue Speaker");
                _isPlayerSpeaking = value;
                EditorUtility.SetDirty(this);
            }
        }
#endif

        public string Text {
            get { return _text; }
#if UNITY_EDITOR
            set {
                if (_text != value) {
                    Undo.RecordObject(this, "Update Dialogue Text");
                    _text = value;
                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        public Rect Rect {
            get => _rect;
#if UNITY_EDITOR
            set {
                Undo.RecordObject(this, "Update Node Position");
                _rect = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public Vector2 Position {
            get => _rect.position;
#if UNITY_EDITOR
            set {
                Undo.RecordObject(this, "Update Node Position");
                _rect.position = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public Vector2 Size {
            get => _rect.size;
#if UNITY_EDITOR
            set {
                Undo.RecordObject(this, "Update Node Resize");
                _rect.size = value;
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public IEnumerable<string> GetChildren() {
            return _children;
        }

        public DialogueNode AddChild(string childID) {
            Undo.RecordObject(this, "Link Child");
            _children.Add(childID);
            EditorUtility.SetDirty(this);
            return this;
        }
        public DialogueNode RemoveChild(string childID) {
            Undo.RecordObject(this, "Unlink Child");
            _children.Remove(childID);
            EditorUtility.SetDirty(this);
            return this;
        }
    }
}
