using TagFighter.Dialogue;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueEditor : EditorWindow
{
    DialogueView _dialogueView;
    [SerializeField]
    Dialogue _selectedDialogue;

    [MenuItem("TagFighter/DialogueEditor")]
    public static void ShowEditorWindow() {
        var window = GetWindow<DialogueEditor>();
        window.titleContent = new GUIContent("DialogueEditor");
        window.UpdateEditorView();

    }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line) {
        var dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        if (dialogue != null) {
            ShowEditorWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Dialogue/DialogueEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Dialogue/DialogueEditor.uss");
        root.styleSheets.Add(styleSheet);

        _dialogueView = root.Q<DialogueView>();
        _dialogueView.OnNodeSelected = OnNodeSelectionChanged;
        UpdateEditorView();
    }

    void UpdateEditorView() {
        _dialogueView.PopulateView(_selectedDialogue);
    }

    void OnSelectionChanged() {
        // AssetDatabase.CanOpenAssetInEditor(dialogueCandidate.GetInstanceID())
        switch (Selection.activeObject) {
            case Dialogue dialogue:

                _selectedDialogue = dialogue;
                UpdateEditorView();
                break;
            case DialogueNode node:
                var dialogueCandidate = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(Selection.activeObject)) as Dialogue;
                if (_selectedDialogue != dialogueCandidate) {
                    _selectedDialogue = dialogueCandidate;
                    UpdateEditorView();
                }
                break;
            default:
                if (_selectedDialogue == null) {
                    UpdateEditorView();
                }
                break;
        }
    }

    void OnNodeSelectionChanged(DialogueNodeView nodeView) {
        Selection.activeObject = nodeView.Node;
    }

    void OnUndoRedo() {
        _dialogueView.PopulateView(_selectedDialogue);
    }

    void OnEnable() {
        Undo.undoRedoPerformed += OnUndoRedo;
        Selection.selectionChanged += OnSelectionChanged;
    }

    void OnDisable() {
        Undo.undoRedoPerformed -= OnUndoRedo;
        Selection.selectionChanged -= OnSelectionChanged;
    }
}