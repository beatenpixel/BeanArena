using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(ItemStatProgression))]
public class ItemStatProgressionDrawer : PropertyDrawer {

    private static float lineHeight => EditorGUIUtility.singleLineHeight + 2;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var isFoldoutInInspectorProp = property.FindPropertyRelative(nameof(ItemStatProgression.isFoldoutInInspector));
        var soItemInfoProp = property.FindPropertyRelative(nameof(ItemStatProgression.soItemInfo));
        SO_ItemInfo itemInfo = (SO_ItemInfo)soItemInfoProp.objectReferenceValue;

        return isFoldoutInInspectorProp.boolValue ? (150 + itemInfo.maxLevel * lineHeight) : lineHeight;
        //        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        var statTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.statType));
        StatType statType = (StatType)statTypeProp.enumValueIndex;
        
        var isFoldoutInInspectorProp = property.FindPropertyRelative(nameof(ItemStatProgression.isFoldoutInInspector));
        var soItemInfoProp = property.FindPropertyRelative(nameof(ItemStatProgression.soItemInfo));
        SO_ItemInfo itemInfo = (SO_ItemInfo)soItemInfoProp.objectReferenceValue;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(statType.ToString()));
        var foldoutStyle = new GUIStyle(EditorStyles.foldout) {
            fixedHeight = lineHeight
        };

        var statValueTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.valueType));
        var statProgressionTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.progressionType));

        isFoldoutInInspectorProp.boolValue = EditorGUI.Foldout(position, isFoldoutInInspectorProp.boolValue, "Open", false, foldoutStyle);

        position.height = lineHeight;

        if (isFoldoutInInspectorProp.boolValue) {
            /*Show folded subproperty*/
            position = AddLine(position);            

            statTypeProp.enumValueIndex = (int)(StatType)EditorGUI.EnumPopup(position, (StatType)statTypeProp.enumValueIndex);
            position = AddLine(position);

            StatValueType statValueType = (StatValueType)EditorGUI.EnumPopup(position, (StatValueType)statValueTypeProp.enumValueIndex);
            statValueTypeProp.enumValueIndex = (int)statValueType;
            position = AddLine(position);

            Rect progressionTypeRect = position;
            progressionTypeRect.width *= 0.5f;

            ItemStatProgressionType progressionType = (ItemStatProgressionType)EditorGUI.EnumPopup(progressionTypeRect, (ItemStatProgressionType)statProgressionTypeProp.enumValueIndex);
            statProgressionTypeProp.enumValueIndex = (int)progressionType;

            Rect progressionFuncRect = position;
            progressionFuncRect.width *= 0.5f;
            progressionFuncRect.x += progressionFuncRect.width;

            //EditorGUILayout.PropertyField(property.FindPropertyRelative(nameof(ItemStatProgression.progressionType)));
            var progressionFuncProp = property.FindPropertyRelative(nameof(ItemStatProgression.progressionFunc));
            StatProgressionFunc progressionFunc = (StatProgressionFunc)EditorGUI.EnumPopup(progressionFuncRect, (StatProgressionFunc)progressionFuncProp.enumValueIndex);
            progressionFuncProp.enumValueIndex = (int)progressionFunc;

            position = AddLine(position);

            if (itemInfo != null) {

                if (statValueType == StatValueType.Int) {
                    if (progressionType == ItemStatProgressionType.Interpolate) {
                        var intStartEndProp = property.FindPropertyRelative(nameof(ItemStatProgression.intStartEndValue));
                        Vector2Int intStartEnd = EditorGUI.Vector2IntField(position, GUIContent.none, intStartEndProp.vector2IntValue);
                        intStartEndProp.vector2IntValue = intStartEnd;

                        position = AddLine(position);

                        var roundAccuracyProp = property.FindPropertyRelative(nameof(ItemStatProgression.roundAccuracy));
                        GUI.Label(position, new GUIContent("Acc"));
                        Rect roundAccRect = position; roundAccRect.x += 30;
                        float roundAccuracy = EditorGUI.FloatField(roundAccRect, roundAccuracyProp.floatValue, new GUIStyle(EditorStyles.numberField) {
                            fixedWidth = 30
                        });

                        if(Mathf.Abs(roundAccuracy) < 0.001f) {
                            roundAccuracy = 1;
                        }

                        roundAccuracyProp.floatValue = roundAccuracy;

                        position = AddLine(position);

                        for (int i = 0; i < itemInfo.maxLevel; i++) {
                            float p = i / (float)(itemInfo.maxLevel - 1);
                            float p2;

                            switch(progressionFunc) {
                                case StatProgressionFunc.QuadIn:
                                    p2 = Easing.Quadratic.In(p);
                                    break;
                                case StatProgressionFunc.QuadOut:
                                    p2 = Easing.Quadratic.Out(p);
                                    break;
                                case StatProgressionFunc.SineIn:
                                    p2 = Easing.Sine.In(p);
                                    break;
                                case StatProgressionFunc.SineOut:
                                    p2 = Easing.Sine.Out(p);
                                    break;
                                default:
                                    p2 = Easing.Linear(p);
                                    break;
                            }

                            int interpValue = Mathf.RoundToInt(MMath.RoundToAccuracy(Mathf.Lerp(intStartEnd.x, intStartEnd.y, p2), roundAccuracy, false));

                            EditorGUI.LabelField(position, new GUIContent(interpValue.ToString()));
                            position = AddLine(position);
                        }
                    } else if (progressionType == ItemStatProgressionType.Extrapolate) {

                    }
                } else if(statValueType == StatValueType.Float) {
                    if (progressionType == ItemStatProgressionType.Interpolate) {

                    } else if (progressionType == ItemStatProgressionType.Extrapolate) {

                    }
                }

                /*
                for (int i = 0; i < itemInfo.maxLevel; i++) {
                    if (GUI.Button(position, new GUIContent("Press"), new GUIStyle(EditorStyles.miniButton) {
                        fixedWidth = 50,
                        alignment = TextAnchor.UpperLeft
                    })) {

                    }

                    position.position = position.position.SetY(position.position.y + lineHeight);
                } 
                */
            }
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }

    private Rect AddLine(Rect position) {
        position.position = position.position.SetY(position.position.y + lineHeight);
        return position;
    }


private void SetProperty(SerializedProperty property, bool value) {
        var propRelative = property.FindPropertyRelative(nameof(FloatReference.useConstant));
        propRelative.boolValue = value;
        property.serializedObject.ApplyModifiedProperties();
    }

}
