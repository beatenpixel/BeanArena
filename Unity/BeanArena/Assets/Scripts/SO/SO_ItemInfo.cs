using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/ItemInfo")]
public class SO_ItemInfo : ScriptableObject, ITypeKey<ItemType>, IStatContainer {

    public ItemType itemType;
    public ItemCategory category;
    public string itemName_LKey;
    public string itemDescription_LKey;
    public SO_IconContent icon;
    public GameObject prefab;

    public int maxLevel = 5;

    public List<ItemStatProgression> stats = new List<ItemStatProgression>();

    public ItemType GetKey() {
        return itemType;
    }

    public int statsMaxLevel => maxLevel;

    private void OnEnable() {        
        for (int i = 0; i < stats.Count; i++) {
            stats[i].maxLevel = maxLevel;

            if (stats[i].values == null) {
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

    private void OnValidate() {
        for (int i = 0; i < stats.Count; i++) {
            stats[i].maxLevel = maxLevel;
        }
    }

    public ItemStatProgression GetStat(StatType statType) {
        return stats.Find(x => x.statType == statType);
    }

    public Vector2Int GetFusePointsBounds(int itemLevel) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            return new Vector2Int(-1, -1);
        }

        if(itemLevel > fuseStat.values.Length) {
            Debug.LogError("Level is too big!!!");
            return new Vector2Int(-1, -1);
        }

        StatValue start = fuseStat.values[itemLevel];
        StatValue end;

        if(itemLevel == maxLevel - 1) {
            return new Vector2Int(start.intValue, start.intValue);
        } else {
            end = fuseStat.values[itemLevel + 1];
            return new Vector2Int(start.intValue, end.intValue);
        }        
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

public interface IStatContainer {
    int statsMaxLevel { get; }
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
    public ItemStatUpgradeDirection upgradeDirection = ItemStatUpgradeDirection.HigherIsBetter;

    public Vector2Int intStartEndValue;
    public Vector2 floatStartEndValue;

    public StatValue[] values;

    public int decimalsCount = 2;

    public int maxLevel;
    [HideInInspector] public bool isFoldoutInInspector;

    public int GetLevelByValue(object value) {
        if (valueType == StatValueType.Int) {
            int intValue = (int)value;

            for (int i = 0; i < values.Length - 1; i++) {
                if(intValue >= values[i].intValue && intValue < values[i + 1].intValue) {
                    return i;
                }
            }

            return maxLevel - 1;
        } else if(valueType == StatValueType.Float) {
            float floatValue = (float)value;

            for (int i = 0; i < values.Length - 1; i++) {
                if (floatValue > values[i].floatValue && floatValue < values[i + 1].floatValue) {
                    return i;
                }
            }

            return maxLevel - 1;
        }

        return -1;
    }

    public string GetDifferenceStr(int levelA, int levelB) {
        string str = "";

        string posStr = MFormat.TextColorTag(MAssets.colors["STAT_UPGRADE_COLOR"]);
        string negStr = MFormat.TextColorTag(MAssets.colors["STAT_DOWNGRADE_COLOR"]);

        if (valueType == StatValueType.Int) {
            StatValue valueA = values[levelA];
            StatValue valueB = values[levelB];

            str += valueA.intValue;

            int diff = valueB.intValue - valueA.intValue;
            int diffSign = MMath.SignInt(diff);

            int signedUpgradeDirection = (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter ? 1 : -1);

            if(diff != 0) {
                int posOrNeg = diffSign * signedUpgradeDirection;
                str += $"{((posOrNeg == 1) ? posStr : negStr)} ({MFormat.GetSignStr((MFormat.Sign)diffSign)}{Mathf.Abs(diff)}){MFormat.TextColorTagEnd}";
            }
        } else if (valueType == StatValueType.Float) {
            StatValue valueA = values[levelA];
            StatValue valueB = values[levelB];

            str += Math.Round(valueA.decimalValue, decimalsCount);

            decimal diff = Math.Round(valueB.decimalValue - valueA.decimalValue, decimalsCount);
            int diffSign = Math.Sign(diff);

            int signedUpgradeDirection = (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter ? 1 : -1);

            if (diff != 0) {
                int posOrNeg = diffSign * signedUpgradeDirection;
                str += $"{((posOrNeg == 1) ? posStr : negStr)} ({MFormat.GetSignStr((MFormat.Sign)diffSign)}{Math.Abs(diff)}){MFormat.TextColorTagEnd}";
            }
        } 
        
        /*
        else if(valueType == StatValueType.Float) {
            StatValue valueA = values[levelA];
            StatValue valueB = values[levelB];

            str += valueA.floatValue;

            float diff = valueB.floatValue - valueA.floatValue;
            int diffSign = MMath.SignInt(diff);

            int signedUpgradeDirection = (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter ? 1 : -1);

            if (diff != 0) {
                int posOrNeg = diffSign * signedUpgradeDirection;
                str += $"{((posOrNeg == 1) ? posStr : negStr)} ({MFormat.GetSignStr((MFormat.Sign)diffSign)}{Mathf.Abs(diff)}){MFormat.TextColorTagEnd}";
            }
        }
        */

        return str;
    }

    public string GetValueStr(int lvl) {
        switch (valueType) {
            case StatValueType.Int:                
                return values[lvl].intValue.ToString();
            case StatValueType.Float:
                return Math.Round(values[lvl].decimalValue, decimalsCount).ToString();
        }

        return null;
    }

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
    public decimal decimalValue => (decimal)floatValue;
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

public enum ItemStatUpgradeDirection {
    LowerIsBetter = 0,
    HigherIsBetter = 1
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