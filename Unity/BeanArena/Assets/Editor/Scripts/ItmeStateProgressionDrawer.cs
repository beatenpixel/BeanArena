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
        int maxLevel = GetStatsMaxLevel(property);

        return isFoldoutInInspectorProp.boolValue ? (150 + maxLevel * lineHeight) : lineHeight;
        //        return base.GetPropertyHeight(property, label);
    }

    public int GetStatsMaxLevel(SerializedProperty property) {
        if (false) {
            //var soItemInfoProp = property.FindPropertyRelative(nameof(ItemStatProgression.statContainer));
            //SO_ItemInfo itemInfo = (SO_ItemInfo)soItemInfoProp.objectReferenceValue;
            //return itemInfo.maxLevel;
            return 10;
        } else {
            var maxLevelProp = property.FindPropertyRelative(nameof(ItemStatProgression.maxLevel));            
            return maxLevelProp.intValue;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        var statTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.statType));
        StatType statType = (StatType)statTypeProp.enumValueIndex;
        
        var isFoldoutInInspectorProp = property.FindPropertyRelative(nameof(ItemStatProgression.isFoldoutInInspector));

        int statsMaxLevel = GetStatsMaxLevel(property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(statType.ToString()));
        var foldoutStyle = new GUIStyle(EditorStyles.foldout) {
            fixedHeight = lineHeight
        };

        var statValueTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.valueType));
        var statProgressionTypeProp = property.FindPropertyRelative(nameof(ItemStatProgression.progressionType));
        //var statIconProp = property.FindPropertyRelative(nameof(ItemStatProgression.statIcon));

        isFoldoutInInspectorProp.boolValue = EditorGUI.Foldout(position, isFoldoutInInspectorProp.boolValue, "Open", false, foldoutStyle);

        position.height = lineHeight;

        if (isFoldoutInInspectorProp.boolValue) {
            /*Show folded subproperty*/
            position = AddLine(position);

            Rect statTypeRect = position;
            statTypeRect.width *= 0.5f;

            statTypeProp.enumValueIndex = (int)(StatType)EditorGUI.EnumPopup(statTypeRect, (StatType)statTypeProp.enumValueIndex);

            statTypeRect.x += statTypeRect.width;

            StatValueType statValueType = (StatValueType)EditorGUI.EnumPopup(statTypeRect, (StatValueType)statValueTypeProp.enumValueIndex);
            statValueTypeProp.enumValueIndex = (int)statValueType;
            position = AddLine(position);

            /*
            TMProIcon statIcon = (TMProIcon)EditorGUI.EnumPopup(position, (TMProIcon)statIconProp.enumValueIndex);
            statIconProp.enumValueIndex = (int)statValueType;
            position = AddLine(position);
            */

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
            Rect upgradeDirectionRect = position;
            progressionTypeRect.width *= 0.5f;

            var upgradeDirectionProp = property.FindPropertyRelative(nameof(ItemStatProgression.upgradeDirection));
            ItemStatUpgradeDirection upgradeDirection = (ItemStatUpgradeDirection)EditorGUI.EnumPopup(upgradeDirectionRect, (ItemStatUpgradeDirection)upgradeDirectionProp.enumValueIndex);
            upgradeDirectionProp.enumValueIndex = (int)upgradeDirection;

            position = AddLine(position);

            bool isInt = statValueType == StatValueType.Int;

            if (progressionType == ItemStatProgressionType.Interpolate) {
                var intStartEndProp = property.FindPropertyRelative(nameof(ItemStatProgression.intStartEndValue));
                var floatStartEndProp = property.FindPropertyRelative(nameof(ItemStatProgression.floatStartEndValue));

                Vector2 startEnd;

                if (isInt) {
                    Vector2Int intStartEnd = EditorGUI.Vector2IntField(position, GUIContent.none, intStartEndProp.vector2IntValue);
                    intStartEndProp.vector2IntValue = intStartEnd;
                    startEnd = new Vector2(intStartEnd.x, intStartEnd.y);
                } else {
                    Vector2 floatStartEnd = EditorGUI.Vector2Field(position, GUIContent.none, floatStartEndProp.vector2Value);
                    floatStartEndProp.vector2Value = floatStartEnd;
                    startEnd = floatStartEnd;
                }

                position = AddLine(position);

                var roundAccuracyProp = property.FindPropertyRelative(nameof(ItemStatProgression.decimalsCount));

                GUI.Label(position, new GUIContent("Acc"));
                Rect roundAccRect = position; roundAccRect.x += 30;
                int roundAccuracy = EditorGUI.IntField(roundAccRect, roundAccuracyProp.intValue, new GUIStyle(EditorStyles.numberField) {
                    fixedWidth = 60
                });

                roundAccuracyProp.intValue = roundAccuracy;

                position = AddLine(position);

                var valuesArrayProp = property.FindPropertyRelative(nameof(ItemStatProgression.values));

                valuesArrayProp.arraySize = statsMaxLevel;

                for (int i = 0; i < statsMaxLevel; i++) {
                    var elemProp = valuesArrayProp.GetArrayElementAtIndex(i);
                    var manual = elemProp.FindPropertyRelative(nameof(StatValue.manual));
                    var elemIntValue = elemProp.FindPropertyRelative(nameof(StatValue.intValue));
                    var elemFloatValue = elemProp.FindPropertyRelative(nameof(StatValue.floatValue));

                    var elemValueTypeValue = elemProp.FindPropertyRelative(nameof(StatValue.valueType));
                    elemValueTypeValue.enumValueIndex = (int)statValueType;

                    float p = i / (float)(statsMaxLevel - 1);
                    float p2;

                    switch (progressionFunc) {
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

                    float interpValue = MMath.RoundToAccuracy(Mathf.Lerp(startEnd.x, startEnd.y, Mathf.Clamp01(p2)), Mathf.Pow(10, -roundAccuracy), false);                    

                    if (!manual.boolValue) {
                        if (isInt) {
                            elemIntValue.intValue = Mathf.RoundToInt(interpValue);
                        } else {
                            elemFloatValue.floatValue = interpValue;
                        }
                    } else {
                        if (isInt) {
                            interpValue = elemIntValue.intValue;
                        } else {
                            interpValue = elemFloatValue.floatValue;
                        }
                    }

                    Rect labelRect = position;

                    labelRect.width = 15;

                    EditorGUI.LabelField(labelRect, new GUIContent($"{i}"));

                    labelRect.width = 50;
                    labelRect.x += 15;

                    if (isInt) {
                        EditorGUI.LabelField(labelRect, new GUIContent($"{interpValue.ToString("F0")}"));
                    } else {
                        EditorGUI.LabelField(labelRect, new GUIContent($"{interpValue}"));
                    }

                    Rect toggleRect = labelRect;
                    toggleRect.width = 15;
                    toggleRect.x += 70;

                    manual.boolValue = EditorGUI.Toggle(toggleRect, manual.boolValue);

                    if (manual.boolValue) {
                        toggleRect.width = 50;
                        toggleRect.x += 20;

                        if (isInt) {
                            elemIntValue.intValue = EditorGUI.IntField(toggleRect, elemIntValue.intValue);
                        } else {
                            elemFloatValue.floatValue = EditorGUI.FloatField(toggleRect, elemFloatValue.floatValue);
                        }
                    }

                    position = AddLine(position);
                }
            }

            if (statValueType == StatValueType.Int) {

            } else if (statValueType == StatValueType.Float) {
                if (progressionType == ItemStatProgressionType.Interpolate) {

                } else if (progressionType == ItemStatProgressionType.Extrapolate) {

                }
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
