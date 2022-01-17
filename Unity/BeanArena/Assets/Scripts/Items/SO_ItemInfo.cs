using Polyglot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/ItemInfo")]
public class SO_ItemInfo : ScriptableObject {

    public string itemName_LKey;
    public string itemDescription_LKey;
    public SO_IconContent icon;

    public int maxLevel = 5;

    public List<ItemStatProgression> stats;

    private void OnEnable() {
        for (int i = 0; i < stats.Count; i++) {
            stats[i].soItemInfo = this;
        }
    }
}

[System.Serializable]
public class GenericItemState<T> {
    public StatType statType;
    public T[] values;
}

[System.Serializable]
public class ItemStatProgression {
    public StatType statType;
    public StatValueType valueType;
    public ItemStatProgressionType progressionType;
    public StatProgressionFunc progressionFunc;

    public Vector2Int intStartEndValue;
    public Vector2 floatStartEndValue;

    public int[] intValues;
    public float[] floatValues;

    public float roundAccuracy = 5;

    public SO_ItemInfo soItemInfo;
    [HideInInspector] public bool isFoldoutInInspector;

    public object GetValue(int lvl) {
        switch(valueType) {
            case StatValueType.Int: return intValues[lvl];
            case StatValueType.Float: return floatValues[lvl];                
        }

        return null;
    }
}

public enum StatProgressionFunc {
    Linear,
    QuadIn,
    QuadOut,
    SineIn,
    SineOut
}

public enum ItemStatProgressionType {
    Interpolate,
    Extrapolate,
    Manual
}

public enum StatValueType {
    Int,
    Float
}

public enum StatType {
    Health,
    Damage,
    Speed,
    JumpHeight,
    Duration,
}