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

    public bool isUnlocked => cardsCollected > 0;

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
    None,
    DefaultBean,
    Shark,
    Skeleton,
    Clown,
    NakedMan
}

public struct HeroCardsContainer {
    public HeroType heroType;
    public int amount;
}