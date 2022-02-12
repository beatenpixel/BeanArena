using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Inventory : GD {

    public List<GD_Item> items;
    public List<GD_Chest> chests;
    public List<GD_HeroItem> heroes;

    public GD_Inventory() : base(GDType.Inventory, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        for (int i = 0; i < items.Count; i++) {
            items[i].info = MAssets.itemsInfo.GetAsset(items[i].itemType);
        }

        for (int i = 0; i < chests.Count; i++) {
            chests[i].info = MAssets.chestsInfo.GetAsset(chests[i].type);
        }

        for (int i = 0; i < heroes.Count; i++) {
            heroes[i].info = MAssets.heroesInfo.GetAsset(heroes[i].heroType);
        }
    }

    public GD_Inventory(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        items = (List<GD_Item>)info.GetValue("items", typeof(List<GD_Item>));
        chests = info.GetValueSafe<List<GD_Chest>>("chests");
        heroes = info.GetValueSafe<List<GD_HeroItem>>("heroes");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("items", items);
        info.AddValue("chests", chests);
        info.AddValue("heroes", heroes);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        items = new List<GD_Item>();
        items.Add(new GD_Item() {
            itemType = ItemType.Pistol,
            fusePoints = 40,
            levelID = 1,
            rareness = ItemRareness.Common,
        });
        items.Add(new GD_Item() {
            itemType = ItemType.WaterPistol,
            fusePoints = 150,
            levelID = 2,
            rareness = ItemRareness.Epic
        });
        items.Add(new GD_Item() {
            itemType = ItemType.Wheel,
            fusePoints = 666,
            levelID = 3,
            rareness = ItemRareness.Legendary
        });
        items.Add(new GD_Item() {
            itemType = ItemType.Wheel,
            fusePoints = 0,
            levelID = 0,
            rareness = ItemRareness.Common
        });

        chests = new List<GD_Chest>();
        chests.Add(new GD_Chest());
        chests.Add(new GD_Chest() {
            type = ChestType.Epic
        });
        chests.Add(new GD_Chest() {
            type = ChestType.Legendary
        });

        heroes = new List<GD_HeroItem>();
        heroes.Add(new GD_HeroItem() { heroType = HeroType.DefaultBean });
        heroes.Add(new GD_HeroItem() { heroType = HeroType.Shark });
        heroes.Add(new GD_HeroItem() { heroType = HeroType.Skeleton });
        heroes.Add(new GD_HeroItem() { heroType = HeroType.Clown });
    }

}