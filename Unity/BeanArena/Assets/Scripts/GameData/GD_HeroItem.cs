
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_HeroItem : GD {

    public HeroType heroType;
    public int cardsCollected;
    public int levelID;
    public bool isEquiped;
    [NonSerialized] public SO_HeroInfo info;

    public bool isUnlocked => levelID > 0 || cardsCollected > 0;

    public bool enoughCardsToUpgrade {
        get {
            if(levelID < info.rarenessInfo.maxLevel - 1) {
                if(cardsCollected >= info.rarenessInfo.levelsInfo[levelID].cardsToLevel) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
    }

    public void Upgrade() {
        HeroLevelInfo levelInfo = info.rarenessInfo.levelsInfo[levelID];

        if (Economy.inst.HasCurrency(CurrencyType.Coin, levelInfo.coinsToLevel)) {
            cardsCollected -= levelInfo.cardsToLevel;
            Economy.inst.AddCurrency(CurrencyType.Coin, -levelInfo.coinsToLevel);
            levelID++;
        }
    }

    public GD_HeroItem() : base(GDType.ItemData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        
    }

    public GD_HeroItem(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        heroType = (HeroType)info.GetByte("heroType");
        cardsCollected = info.GetInt32("cardsCollected");
        levelID = info.GetInt32("levelID");
        isEquiped = info.GetBoolean("isEquiped");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("heroType", heroType);
        info.AddValue("cardsCollected", cardsCollected);
        info.AddValue("levelID", levelID);
        info.AddValue("isEquiped", isEquiped);        
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        heroType = HeroType.None;
        cardsCollected = 0;
        levelID = 0;
        isEquiped = false;
    }

    public StatValue GetStatValue(StatType statType) {
        return info.GetStat(statType).values[levelID];
    }

    private static HeroType[] heroTypes;
    public static int HeroTypeCount {
        get {
            if(heroTypes == null) {
                heroTypes = EnumUtils.GetValues<HeroType>().ToArray();
            }

            return heroTypes.Length;
        }
    }

}

public enum HeroType {
    None = -1,
    DefaultBean = 100,
    Shark = 101,
    Skeleton = 102,
    Clown = 103,
    NakedMan = 104,
    Terminator = 105,
    Pickle = 106,
    GoldBean = 107,
    Finger = 108,
    Dinosaur = 109,
    Robber = 110
}

public struct HeroCardsContainer {
    public HeroType heroType;
    public int amount;
}