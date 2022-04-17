using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(menuName = "BeanArena/HeroInfo")]
public class SO_HeroInfo : ScriptableObject, ITypeKey<HeroType>, IStatContainer {

    public HeroType heroType;    
    public SO_HeroRarenessInfo rarenessInfo;
    public SO_IconContent icon;
    public GameObject prefab;

    public int maxLevelsCount {
        get {
            if (rarenessInfo == null) {
                return 0;
            } else {
                return rarenessInfo.maxLevel;
            }
        }
    }

    public List<ItemStatProgression> stats = new List<ItemStatProgression>();

    public HeroModel heroModel;

    public HeroType GetKey() {
        return heroType;
    }

    public int statsMaxLevel => maxLevelsCount; 

    private void OnEnable() {
        for (int i = 0; i < stats.Count; i++) {
            stats[i].maxLevel = maxLevelsCount;
            stats[i].Init();
        }
    }

    private void OnValidate() {
        for (int i = 0; i < stats.Count; i++) {
            stats[i].maxLevel = maxLevelsCount;
        }
    }

    public ItemStatProgression GetStat(StatType statType) {
        return stats.Find(x => x.statType == statType);
    }

    public Vector2Int GetFusePointsBounds(GD_HeroItem data, int itemLevel) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            return new Vector2Int(-1, -1);
        }

        if (itemLevel > fuseStat.valuesCount) {
            Debug.LogError("Level is too big!!!");
            return new Vector2Int(-1, -1);
        }

        StatValue start = fuseStat.GetValue(itemLevel, StatConfig.FromHero(data));
        StatValue end;

        if (itemLevel == maxLevelsCount - 1) {
            return new Vector2Int(start.intValue, start.intValue);
        } else {
            end = fuseStat.GetValue(itemLevel + 1, StatConfig.FromHero(data));
            return new Vector2Int(start.intValue, end.intValue);
        }
    }

    public bool GetFusePointsPercent(GD_HeroItem data, int fusePoints, int level, out float fuseP) {
        ItemStatProgression fuseStat = GetStat(StatType.FusePoints);

        if (fuseStat == null) {
            fuseP = -1;
            return false;
        }

        if (level == maxLevelsCount - 1) {
            fuseP = 1;
            return true;
        } else {
            StatValue start = fuseStat.GetValue(level, StatConfig.FromHero(data));
            StatValue end = fuseStat.GetValue(level + 1, StatConfig.FromHero(data));

            fuseP = (fusePoints - start.intValue) / (float)(end.intValue - start.intValue);
            return true;
        }
    }
}

[System.Serializable]
public class HeroModel {



}