using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(VariableReference<>))]
public class ContainerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {        
        EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent(fieldInfo.Name + " " + fieldInfo.FieldType.GetGenericArguments()[0].FullName));
    }
}
