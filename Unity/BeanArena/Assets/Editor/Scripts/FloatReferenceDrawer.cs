using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        bool useConstant = property.FindPropertyRelative(nameof(FloatReference.useConstant)).boolValue;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var rect = new Rect(position.position + new Vector2(2,2), Vector2.one * 14);

        if (EditorGUI.DropdownButton(rect, new GUIContent(MEditorAssets.inst.GetTexture("square_outlined")), FocusType.Keyboard, new GUIStyle() {
            fixedWidth = 50, border = new RectOffset(1, 1, 1, 1)
        })) {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Constant"), useConstant, () => SetProperty(property, true));
            menu.AddItem(new GUIContent("Variable"), !useConstant, () => SetProperty(property, false));
            menu.ShowAsContext();
        }

        int iconWidth = 20;
        position.position += Vector2.right * iconWidth;
        float value = property.FindPropertyRelative("constantValue").floatValue;

        if(useConstant) {
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            style.fixedWidth = 100;
            string newValue = EditorGUI.TextField(position, value.ToString(), style);
            float.TryParse(newValue, out value);
            property.FindPropertyRelative("constantValue").floatValue = value;
        } else {
            UnityEngine.Object floatVariableObj = property.FindPropertyRelative(nameof(FloatReference.variable)).objectReferenceValue;

            int floatPreviewSize = 60;

            if (floatVariableObj != null) {
                Rect labelPos = position;
                labelPos.width = floatPreviewSize;
                FloatVariable floatVariable = (FloatVariable)floatVariableObj;
                EditorGUI.LabelField(labelPos, new GUIContent(floatVariable.value.ToString()));
            }

            position.position = new Vector2(position.position.x + floatPreviewSize, position.position.y);
            
            Rect objRect = position;
            objRect.width -= iconWidth + floatPreviewSize;
            EditorGUI.ObjectField(objRect, property.FindPropertyRelative("variable"), GUIContent.none);
        }

        EditorGUI.EndProperty();
    }

    private void SetProperty(SerializedProperty property, bool value) {
        var propRelative = property.FindPropertyRelative(nameof(FloatReference.useConstant));
        propRelative.boolValue = value;
        property.serializedObject.ApplyModifiedProperties();
    } 

}
