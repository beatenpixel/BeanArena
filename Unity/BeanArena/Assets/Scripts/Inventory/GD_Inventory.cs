using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Inventory : GD {

    public List<GD_Item> items;

    public GD_Inventory() : base(GDType.Inventory, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void RestoreInventory() {
        for (int i = 0; i < items.Count; i++) {
            items[i].info = MAssets.itemsInfo.GetAsset(items[i].itemType);
        }
    }

    public GD_Inventory(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        items = (List<GD_Item>)info.GetValue("items", typeof(List<GD_Item>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("items", items);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        items = new List<GD_Item>();
        items.Add(new GD_Item() {
            itemType = ItemType.Pistol,
            fusePoints = 40,
            levelID = 1,
            rareness = ItemRareness.Common
        });
        items.Add(new GD_Item() {
            itemType = ItemType.WaterPistol,
            fusePoints = 150,
            levelID = 2,
            rareness = ItemRareness.Epic
        });
        items.Add(new GD_Item() {
            itemType = ItemType.JumpyBoots,
            fusePoints = 100,
            levelID = 1,
            rareness = ItemRareness.Uncommon
        });
        items.Add(new GD_Item() {
            itemType = ItemType.RocketBoots,
            fusePoints = 666,
            levelID = 3,
            rareness = ItemRareness.Legendary
        });
        items.Add(new GD_Item() {
            itemType = ItemType.RocketBoots,
            fusePoints = 666,
            levelID = 3,
            rareness = ItemRareness.Legendary
        });
    }

}
