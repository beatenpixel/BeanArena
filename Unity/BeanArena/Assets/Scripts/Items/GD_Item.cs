using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Item : GD {

    public ItemType itemType;
    public ItemRareness rareness;
    public int level;
    public int fusePoints;

    public GD_Item() : base(GDType.ItemData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {

    }

    public GD_Item(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        itemType = (ItemType)info.GetByte("itemType");
        rareness = (ItemRareness)info.GetByte("rareness");
        level = info.GetInt32("level");
        fusePoints = info.GetInt32("fusePoints");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("itemType", (byte)itemType);
        info.AddValue("rareness", (byte)rareness);
        info.AddValue("level", level);
        info.AddValue("fusePoints", fusePoints);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        itemType = ItemType.None;
        rareness = ItemRareness.Common;
        level = 1;
        fusePoints = 0;        
    }

}

public class Item {
    public GD_Item data;
    public SO_ItemInfo info;

    public Item(SO_ItemInfo _reference) {
        info = _reference;
        data = new GD_Item();
    }
}

public enum ItemType : byte {
    None,
    Pistol,
    WaterPistol,
}

public enum ItemRareness : byte {
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary
}
