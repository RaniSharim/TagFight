/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuneRef))]
public class RuneEditor : Editor
{
    EffectRef effect;
    SerializedProperty _area;
    protected void OnEnable() {
        // Debug.Log("EffectRefEditor OnEnable");
        // _area = serializedObject.FindProperty("area");
    }
    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("rune.name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rune.speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rune.manaCost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("RuneSprite"));



        Type et = typeof(IEffect);
        var effectTypes = et.Assembly.GetTypes().Where(t => et.IsAssignableFrom(t) && !t.IsInterface).Select(t => t.Name.Replace(EffectRef.EffectTypeSuffix, "")).ToArray();

        var effectField = serializedObject.FindProperty("rune.effects");
        effectField.isExpanded = EditorGUILayout.Foldout(effectField.isExpanded, effectField.displayName, true);
        if (effectField.arraySize < 1) {
            effectField.InsertArrayElementAtIndex(0);
        }
        EditorGUI.indentLevel += 1;
        if (effectField.isExpanded) {
            for (int i = 0; i < effectField.arraySize; i++) {
                var arrElem = effectField.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(effectField.GetArrayElementAtIndex(i), false);
                EditorGUI.indentLevel += 1;
                {
                    if (arrElem.isExpanded) {
                        var selectedEffectIndex = Array.IndexOf(effectTypes, arrElem.FindPropertyRelative("EffectType").stringValue);
                        if (selectedEffectIndex == -1) {
                            selectedEffectIndex = 0;
                        }
                        var chosenEffectType = effectTypes[EditorGUILayout.Popup("EffectType", selectedEffectIndex, effectTypes)];
                        arrElem.FindPropertyRelative("EffectType").stringValue = chosenEffectType;
                        EditorGUILayout.PropertyField(arrElem.FindPropertyRelative("Color"));
                        EditorGUILayout.PropertyField(arrElem.FindPropertyRelative("Multiplicative"));
                        EditorGUILayout.PropertyField(arrElem.FindPropertyRelative("MultiplicativeBeforeFlat"));
                        EditorGUILayout.PropertyField(arrElem.FindPropertyRelative("Area"));
                        EditorGUILayout.PropertyField(arrElem.FindPropertyRelative("FriendOrFoeOff"));
                    }

                }
                EditorGUI.indentLevel -= 1;
            }
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("-", "delete"), EditorStyles.miniButtonLeft, GUILayout.Width(40f))) {
            effectField.DeleteArrayElementAtIndex(effectField.arraySize - 1);
        }

        if (GUILayout.Button(new GUIContent("+", "add"), EditorStyles.miniButtonRight, GUILayout.Width(40f))) {
            effectField.InsertArrayElementAtIndex(effectField.arraySize);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();
        // Debug.Log("EffectRefEditor OnInspectorGUI");

        // Type et =  typeof(IEffect);
        // var effectTypes = et.Assembly.GetTypes().Where(t => et.IsAssignableFrom(t) && !t.IsInterface).Select(t => t.Name.Replace(EffectRef.EffectTypeSuffix, "")).ToArray();


        // EditorGUILayout.Popup("EffectType", 0, effectTypes);
        // effect.EffectType = effectTypes[];
        // effect.color = (TagColor)EditorGUILayout.EnumPopup("Color", effect.color);
        // EditorGUILayout.EnumPopup("Color", effect.color);

        // effect.multiplicative = EditorGUILayout.FloatField("Multiplicative", effect.multiplicative);
        // effect.flat = EditorGUILayout.FloatField("Flat", effect.flat);
        // effect.multiplicativeBeforeFlat = EditorGUILayout.Toggle("Multiplicative before Flat", effect.multiplicativeBeforeFlat);

        // EditorGUILayout.PropertyField(_area);
    }


}
*/