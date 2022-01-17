using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Inventory : GD {

    public List<ItemData> items;

    public GD_Inventory() : base(GDType.Inventory, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void RestoreGame() {

    }

    public GD_Inventory(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        items = (List<ItemData>)info.GetValue("items", typeof(List<ItemData>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("items", items);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        items = new List<ItemData>();
        items.Add(new ItemData() {
            itemType = ItemType.Pistol,
            fusePoints = 0,
            level = 0,
            rareness = ItemRareness.Common
        });
    }

}
