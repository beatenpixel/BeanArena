using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/HeroInfo")]
public class SO_HeroInfo : ScriptableObject, ITypeKey<HeroType>, IStatContainer {

    public HeroType heroType;
    public ItemRareness heroRareness;
    public SO_IconContent icon;
    public GameObject prefab;

    public int maxLevel = 5;

    public List<ItemStatProgression> stats = new List<ItemStatProgression>();

    public HeroType GetKey() {
        return heroType;
    }

    public int statsMaxLevel => maxLevel; 

    private void OnEnable() {
        for (int i = 0; i < stats.Count; i++) {
            //stats[i].statContainer = this;
            stats[i].maxLevel = maxLevel;

            if (stats[i].values == null) {
                stats[i].values = new StatValue[maxLevel];
                for (int x = 0; x < stats[i].values.Length; x++) {
                    stats[i].values[x] = new StatValue();
                }
            }

            if (stats[i].values.Length != maxLevel) {
                StatValue[] prevArray = stats[i].values;
                stats[i].values = new StatValue[maxLevel];

                if (prevArray.Length > stats[i].values.Length) {
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

    private ItemStatProgression GetStat(StatType statType) {
        return stats.Find(x => x.statType == statType);
    }

    public Vector2Int GetFusePointsBounds(int itemLevel) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            return new Vector2Int(-1, -1);
        }

        if (itemLevel > fuseStat.values.Length) {
            Debug.LogError("Level is too big!!!");
            return new Vector2Int(-1, -1);
        }

        StatValue start = fuseStat.values[itemLevel];
        StatValue end;

        if (itemLevel == maxLevel - 1) {
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