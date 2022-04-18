using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/ItemInfo")]
public class SO_ItemInfo : ScriptableObject, ITypeKey<ItemType>, IStatContainer {

    public ItemType itemType;
    public ItemCategory category;
    public SO_IconContent icon;
    public GameObject prefab;

    public int maxLevel = 5;

    public List<ItemStatProgression> stats = new List<ItemStatProgression>();

    public DropInfo dropInfo;

    public string localizationKeyName => "ITEM_NAME_" + itemType.ToString().ToUpper();
    public string localizationKeyDescr => "ITEM_DESCR_" + itemType.ToString().ToUpper();

    public ItemType GetKey() {
        return itemType;
    }

    public int statsMaxLevel => maxLevel;

    private void OnEnable() {        
        for (int i = 0; i < stats.Count; i++) {
            stats[i].maxLevel = maxLevel;
            stats[i].Init();         
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

    public bool HasStat(StatType statType) {
        for (int i = 0; i < stats.Count; i++) {
            if(stats[i].statType == statType) {
                return true;
            }
        }

        return false;
    }

    public Vector2Int GetFusePointsBounds(GD_Item data, int itemLevel) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            return new Vector2Int(-1, -1);
        }

        if(itemLevel > fuseStat.valuesCount) {
            Debug.LogError("Level is too big!!!");
            return new Vector2Int(-1, -1);
        }

        StatValue start = fuseStat.GetValue(itemLevel, StatConfig.FromItem(data));
        StatValue end;

        if(itemLevel == maxLevel - 1) {
            return new Vector2Int(start.intValue, start.intValue);
        } else {
            end = fuseStat.GetValue(itemLevel + 1, StatConfig.FromItem(data));
            return new Vector2Int(start.intValue, end.intValue);
        }        
    }

    public bool GetFusePointsPercent(GD_Item data, int fusePoints, int level, out float fuseP) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            fuseP = -1;
            return false;
        }

        if (level == maxLevel - 1) {
            fuseP = 1;
            return true;
        } else {
            StatValue start = fuseStat.GetValue(level, StatConfig.FromItem(data));
            StatValue end = fuseStat.GetValue(level + 1, StatConfig.FromItem(data));

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
    public StatProgressionFunc progressionFunc = StatProgressionFunc.Pow17;
    public ItemStatUpgradeDirection upgradeDirection = ItemStatUpgradeDirection.HigherIsBetter;

    public Vector2Int intStartEndValue;
    public Vector2 floatStartEndValue;

    [SerializeField] private StatValue[] values;

    public int decimalsCount = 2;

    public int maxLevel;
    [HideInInspector] public bool isFoldoutInInspector;

    public int valuesCount => values.Length;

    public void Init() {
        if (values == null) {
            values = new StatValue[maxLevel];
            for (int x = 0; x < values.Length; x++) {
                values[x] = new StatValue();
            }
        }

        if (values.Length != maxLevel) {
            StatValue[] prevArray = values;
            values = new StatValue[maxLevel];

            if (prevArray.Length > values.Length) {
                Array.Copy(prevArray, values, values.Length);
            } else {
                Array.Copy(prevArray, values, prevArray.Length);
            }
        }
    }

    public int GetLevelByValue(object value, StatConfig config) {
        if (valueType == StatValueType.Int) {
            int intValue = (int)value;

            if(intValue < GetValue(0, config).intValue) {
                return 0;
            }

            for (int i = 0; i < values.Length - 1; i++) {
                if(intValue >= GetValue(i, config).intValue && intValue < GetValue(i + 1, config).intValue) {
                    return i;
                }
            }

            return maxLevel - 1;
        } else if(valueType == StatValueType.Float) {
            float floatValue = (float)value;

            if (floatValue < GetValue(0, config).floatValue) {
                return 0;
            }

            for (int i = 0; i < values.Length - 1; i++) {
                if (floatValue > GetValue(i, config).floatValue && floatValue < GetValue(i + 1, config).floatValue) {
                    return i;
                }
            }

            return maxLevel - 1;
        }

        return -1;
    }

    public string GetDifferenceStr(int levelA, int levelB, StatConfig config) {
        string str = "";

        string posStr = MFormat.TextColorTag(MAssets.inst.colors["STAT_UPGRADE_COLOR"]);
        string negStr = MFormat.TextColorTag(MAssets.inst.colors["STAT_DOWNGRADE_COLOR"]);

        if (valueType == StatValueType.Int) {
            StatValue valueA = GetValue(levelA, config);
            StatValue valueB = GetValue(levelB, config);

            str += valueA.intValue;

            int diff = valueB.intValue - valueA.intValue;
            int diffSign = MMath.SignInt(diff);

            int signedUpgradeDirection = (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter ? 1 : -1);

            if(diff != 0) {
                int posOrNeg = diffSign * signedUpgradeDirection;
                str += $"{((posOrNeg == 1) ? posStr : negStr)} ({MFormat.GetSignStr((MFormat.Sign)diffSign)}{Mathf.Abs(diff)}){MFormat.TextColorTagEnd}";
            }
        } else if (valueType == StatValueType.Float) {
            StatValue valueA = GetValue(levelA, config);
            StatValue valueB = GetValue(levelB, config);

            str += Math.Round(valueA.decimalValue, decimalsCount);

            decimal diff = Math.Round(valueB.decimalValue - valueA.decimalValue, decimalsCount);
            int diffSign = Math.Sign(diff);

            int signedUpgradeDirection = (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter ? 1 : -1);

            if (diff != 0) {
                int posOrNeg = diffSign * signedUpgradeDirection;
                str += $"{((posOrNeg == 1) ? posStr : negStr)} ({MFormat.GetSignStr((MFormat.Sign)diffSign)}{Math.Abs(diff)}){MFormat.TextColorTagEnd}";
            }
        } 

        return str;
    }

    public string GetValueStr(int lvl, StatConfig config) {
        StatValue value = GetValue(lvl, config);

        switch (valueType) {
            case StatValueType.Int:                
                return value.intValue.ToString();
            case StatValueType.Float:
                return Math.Round(value.decimalValue, decimalsCount).ToString();
        }

        return null;
    }

    public StatValue GetValue(int lvl, StatConfig config) {
        StatValue statValue = values[lvl];
        StatValue newStatValue = new StatValue(statValue);

        float rarenessMul = 1f;

        if (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter) {
            rarenessMul = GameBalance.rarenessToMulUp[config.rareness];
        } else if (upgradeDirection == ItemStatUpgradeDirection.LowerIsBetter) {
            rarenessMul = GameBalance.rarenessToMulDown[config.rareness];
        }

        switch (statType) {
            case StatType.Damage:
                //rarenessMul = Mathf.Pow(rarenessMul, 0.8f);
                break;
            case StatType.Health:
                //rarenessMul = Mathf.Pow(rarenessMul, 1.1f);
                break;
            case StatType.FusePoints:
                rarenessMul = 1f;
                break;
        }

        if (upgradeDirection == ItemStatUpgradeDirection.HigherIsBetter) {           

            if (valueType == StatValueType.Int) {
                newStatValue.intValue = Mathf.RoundToInt(statValue.intValue * rarenessMul);
            } else {
                newStatValue.floatValue = statValue.floatValue * rarenessMul;
            }
        } else if (upgradeDirection == ItemStatUpgradeDirection.LowerIsBetter) {            

            if (valueType == StatValueType.Int) {
                newStatValue.intValue = Mathf.RoundToInt(statValue.intValue * rarenessMul);
            } else {
                newStatValue.floatValue = statValue.floatValue * rarenessMul;
            }
        }

        return newStatValue;
    }
}

public class StatConfig {
    public ItemRareness rareness;

    public static StatConfig FromHero(GD_HeroItem heroData) {
        return new StatConfig() {
            rareness = ItemRareness.Common
        };
    }

    public static StatConfig FromItem(GD_Item itemData) {
        return new StatConfig() {
            rareness = itemData.rareness
        };
    }

    public static StatConfig Rareness(ItemRareness r) {
        return new StatConfig() {
            rareness = r
        };
    }
}

[System.Serializable]
public class StatValue {
    public StatValueType valueType;
    public int intValue;
    public float floatValue;
    public decimal decimalValue => (decimal)floatValue;
    public bool manual;

    public StatValue() {

    }

    public StatValue(StatValue original) {
        intValue = original.intValue;
        floatValue = original.floatValue;
        manual = original.manual;
    }

    public static StatValue operator +(StatValue a, StatValue b) {
        a.intValue += b.intValue;
        a.floatValue += b.floatValue;
        return a;
    }

}

public enum StatProgressionFunc {
    Linear,
    QuadIn,
    QuadOut,
    SineIn,
    SineOut,
    Pow17
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
    FusePoints,
    Armor
}