using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_HeroItem : GD {

    public HeroType heroType;
    public int levelID;
    public bool isEquiped;
    [NonSerialized] public SO_HeroInfo info;

    public GD_HeroItem() : base(GDType.ItemData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {

    }

    public GD_HeroItem(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        heroType = (HeroType)info.GetByte("heroType");
        levelID = info.GetInt32("level");
        isEquiped = info.GetBoolean("isEquiped");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("heroType", heroType);
        info.AddValue("level", levelID);
        info.AddValue("isEquiped", isEquiped);        
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        heroType = HeroType.None;
        levelID = 0;
        isEquiped = false;
    }

}

public enum HeroType {
    None,
    DefaultBean,
    Shark,
    Skeleton,
    Clown
}