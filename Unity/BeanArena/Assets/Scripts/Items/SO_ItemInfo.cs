using Polyglot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/ItemInfo")]
public class SO_ItemInfo : ScriptableObject, ITypeKey<ItemType> {

    public ItemType itemType;
    public ItemCategory category;
    public string itemName_LKey;
    public string itemDescription_LKey;
    public SO_IconContent icon;
    public GameObject prefab;
    
    public int maxLevel = 5;

    public List<ItemStatProgression> stats;

    public ItemType GetKey() {
        return itemType;
    }

    private void OnEnable() {        
        for (int i = 0; i < stats.Count; i++) {
            stats[i].soItemInfo = this;

            if(stats[i].values == null) {
                stats[i].values = new StatValue[maxLevel];
                for (int x = 0; x < stats[i].values.Length; x++) {
                    stats[i].values[x] = new StatValue();
                }
            }

            if(stats[i].values.Length != maxLevel) {
                StatValue[] prevArray = stats[i].values;
                stats[i].values = new StatValue[maxLevel];

                if(prevArray.Length > stats[i].values.Length) {
                    Array.Copy(prevArray, stats[i].values, stats[i].values.Length);
                } else {
                    Array.Copy(prevArray, stats[i].values, prevArray.Length);
                }
            }            
        }
    }

    private ItemStatProgression GetStat(StatType statType) {
        return stats.Find(x => x.statType == statType);
    }

    public bool GetFusePointsPercent(int fusePoints, int level, out float fuseP) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            fuseP = -1;
            return false;
        }

        if (level == maxLevel - 1) {
            fuseP = 1;
            return true;
        } else {
            StatValue start = fuseStat.values[level];
            StatValue end = fuseStat.values[level + 1];

            fuseP = (fusePoints - start.intValue) / (float)(end.intValue - start.intValue);
            return true;
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
    public StatType statType = StatType.Health;
    public StatValueType valueType = StatValueType.Int;
    public ItemStatProgressionType progressionType = ItemStatProgressionType.Interpolate;
    public StatProgressionFunc progressionFunc = StatProgressionFunc.SineIn;

    public Vector2Int intStartEndValue;
    public Vector2 floatStartEndValue;

    public StatValue[] values;

    public float roundAccuracy = 5;

    public SO_ItemInfo soItemInfo;
    [HideInInspector] public bool isFoldoutInInspector;

    public object GetValue(int lvl) {
        switch(valueType) {
            case StatValueType.Int: return values[lvl].intValue;
            case StatValueType.Float: return values[lvl].floatValue;                
        }

        return null;
    }
}

[System.Serializable]
public class StatValue {
    public int intValue;
    public float floatValue;
    public bool manual;
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
    FusePoints
}