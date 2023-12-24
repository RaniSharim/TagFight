// using TagFighter.Dialogue;
using System.Collections.Generic;
using System.Linq;
using TagFighter.Effects;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace TagFighter.UI.Editor
{
    public class RuneEditor : EditorWindow
    {
        RuneEffectView _runeView;
        ListView _runeEffects;
        public RuneRef SelectedRuneRef;

        [SerializeField]
        RuneEffect _selectedRuneEffect;

        [SerializeField]
        EffectStepNodeView _selectedNodeView;

        [MenuItem("TagFighter/RuneEditor")]
        public static void ShowEditorWindow() {
            var window = GetWindow<RuneEditor>();
            window.titleContent = new GUIContent("RuneEditor");
            window.SelectedRuneRef = Selection.activeObject as RuneRef;
            window.UpdateEditorView();
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) {
            var selectedRuneRef = EditorUtility.InstanceIDToObject(instanceID) as RuneRef;
            if (selectedRuneRef != null) {
                ShowEditorWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Rune/RuneEditor.uss");
            root.styleSheets.Add(styleSheet);

            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            var leftPane = new VisualElement();
            splitView.Add(leftPane);
            var rightPane = new VisualElement();
            splitView.Add(rightPane);

            var runeEffectsControl = new VisualElement() {
                name = "runeEffectsControl"
            };
            leftPane.Add(runeEffectsControl);

            var addRuneEffect = new Button() {
                name = "addRuneEffect",
                text = "+",
            };
            addRuneEffect.clicked += OnAddRuneEffect;
            runeEffectsControl.Add(addRuneEffect);

            var removeRuneEffect = new Button() {
                name = "removeRuneEffect",
                text = "-",
            };
            removeRuneEffect.clicked += OnRemoveRuneEffect;
            runeEffectsControl.Add(removeRuneEffect);


            _runeEffects = new ListView() {
                itemsSource = null,
                makeItem = () => new Label(),
                bindItem = (e, i) => (e as Label).text = SelectedRuneRef.RuneEffects[i].Guid,
            };
            _runeEffects.onSelectionChange += OnRuneEffectSelectionChanged;
            _runeEffects.reorderable = true;

            leftPane.Add(_runeEffects);

            _runeView = new RuneEffectView();
            rightPane.Add(_runeView);


            var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);


            scrollView.Add(splitView);

            root.Add(scrollView);

            _runeView.OnNodeSelected = OnNodeSelectionChanged;
            UpdateEditorView();
        }

        void OnRuneEffectSelectionChanged(IEnumerable<object> enumerable) {
            Selection.activeObject = enumerable.First() as RuneEffect;
        }

        void OnRemoveRuneEffect() {
            var runeEffect = _runeEffects.selectedItem as RuneEffect;
            if (runeEffect == null) {
                return;
            }


            if (SelectedRuneRef.RuneEffects.Remove(runeEffect)) {
                foreach (var step in runeEffect.Nodes) {
                    AssetDatabase.RemoveObjectFromAsset(step);
                }
                AssetDatabase.RemoveObjectFromAsset(runeEffect);
                UpdateEditorView();
            }
        }

        void OnAddRuneEffect() {
            var runeEffect = ScriptableObject.CreateInstance<RuneEffect>();
            runeEffect.Guid = System.Guid.NewGuid().ToString();
            runeEffect.name = runeEffect.Guid;
            SelectedRuneRef.RuneEffects.Add(runeEffect);
            AssetDatabase.AddObjectToAsset(runeEffect, SelectedRuneRef);
            _runeEffects.RefreshItems();
        }

        void UpdateEditorView() {
            if (SelectedRuneRef != null) {
                _runeEffects.itemsSource = SelectedRuneRef.RuneEffects;
                _runeEffects.RefreshItems();
                _selectedRuneEffect = _runeEffects.selectedItem as RuneEffect;
                if (_selectedRuneEffect == null && SelectedRuneRef.RuneEffects.Count > 0) {
                    _runeEffects.SetSelection(0);
                    _selectedRuneEffect = _runeEffects.selectedItem as RuneEffect;
                }
            }
            _runeView.PopulateView(_selectedRuneEffect);
        }

        void OnSelectionChanged() {
            // AssetDatabase.CanOpenAssetInEditor(dialogueCandidate.GetInstanceID())

            switch (Selection.activeObject) {
                case RuneRef rune:
                    SelectedRuneRef = rune;
                    UpdateEditorView();
                    break;
                case EffectStepNode node: {
                        var runeRefCandidate = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(node)) as RuneRef;
                        if (SelectedRuneRef != runeRefCandidate) {
                            SelectedRuneRef = runeRefCandidate;
                            UpdateEditorView();
                        }

                    }
                    break;
                case RuneEffect runeEffect: {
                        var runeRefCandidate = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(runeEffect)) as RuneRef;
                        if (SelectedRuneRef != runeRefCandidate) {
                            SelectedRuneRef = runeRefCandidate;
                        }
                        _selectedRuneEffect = runeEffect;
                        UpdateEditorView();
                    }
                    break;
                default:
                    if (SelectedRuneRef == null) {
                        UpdateEditorView();
                    }
                    break;
            }
        }

        void OnNodeSelectionChanged(EffectStepNodeView nodeView) {
            if (_selectedNodeView != null) {
                _selectedNodeView.Node.EffectStepValidated -= OnEffectStepValidated;
            }
            _selectedNodeView = nodeView;
            _selectedNodeView.Node.EffectStepValidated += OnEffectStepValidated;
            Selection.activeObject = nodeView.Node;
        }

        void OnEffectStepValidated(object sender, EffectStepValidatedEventArgs e) {
            _selectedNodeView.title = e.DisplayName;
        }

        void OnUndoRedo() {
            _runeView.PopulateView(_runeEffects.selectedItem as RuneEffect);
        }

        void OnEnable() {
            Undo.undoRedoPerformed += OnUndoRedo;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnDisable() {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Selection.selectionChanged -= OnSelectionChanged;
            if (_selectedNodeView != null) {
                _selectedNodeView.Node.EffectStepValidated -= OnEffectStepValidated;
            }
        }
    }

}
