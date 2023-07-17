using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utils;
using Editor.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(SlotsUiAdapter))]
public class SlotsUiAdapterEditor : UnityEditor.Editor
{
    private bool _slotsFoldout;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _slotsFoldout =
            EditorGUILayout.Foldout(_slotsFoldout, Utils.GetDisplayName(nameof(SlotsUiAdapter.SlotsUiLookup)));
        
        if (_slotsFoldout)
        {
            BaseEditor.DrawEnumDict<ItemCategory, SlotUi>(serializedObject.FindProperty(nameof(SlotsUiAdapter.SlotsUiLookup)), DrawValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawValue(SerializedProperty pairProperty)
    {
        SerializedProperty valueProperty = pairProperty.FindPropertyRelative(BaseEditor.ValueName);

        EditorGUILayout.PropertyField(valueProperty.FindPropertyRelative(nameof(SlotUi.Image).GetPropertyName()));
        
        EditorGUILayout.PropertyField(valueProperty.FindPropertyRelative(nameof(SlotUi.UnEquipButton).GetPropertyName()));
    }
}
