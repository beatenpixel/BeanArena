using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GD_Inventory : GD {

    public List<GD_Item> items;
    public List<GD_Chest> chests;
    public List<GD_HeroItem> heroes;
    public List<GD_RoadmapReward> roadMapClaimedRewards;

    public GD_Inventory() : base(GDType.Inventory, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {
        for (int i = 0; i < items.Count; i++) {
            items[i].info = MAssets.inst.itemsInfo.GetAsset(items[i].itemType);
        }

        for (int i = 0; i < chests.Count; i++) {
            chests[i].info = MAssets.inst.chestsInfo.GetAsset(chests[i].type);
        }

        List<SO_HeroInfo> allHeroesInfo = MAssets.inst.heroesInfo.GetAllAssets();

        if (heroes.Count < allHeroesInfo.Count) {
            for (int i = 0; i < allHeroesInfo.Count; i++) {
                var r = heroes.Where(x => x.heroType == allHeroesInfo[i].heroType);
                if (r == null || r.Count() <= 0) {
                    heroes.Add(new GD_HeroItem() {
                        heroType = allHeroesInfo[i].heroType,
                    });
                }
            }
        }

        for (int i = 0; i < heroes.Count; i++) {
            heroes[i].info = MAssets.inst.heroesInfo.GetAsset(heroes[i].heroType);
        }
    }

    public GD_Inventory(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        items = (List<GD_Item>)info.GetValue("items", typeof(List<GD_Item>));
        chests = info.GetValueSafe<List<GD_Chest>>("chests");
        heroes = info.GetValueSafe<List<GD_HeroItem>>("heroes");
        roadMapClaimedRewards = info.GetValueSafe<List<GD_RoadmapReward>>("roadmapRewards");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("items", items);
        info.AddValue("chests", chests);
        info.AddValue("heroes", heroes);
        info.AddValue("roadmapRewards", roadMapClaimedRewards);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        items = new List<GD_Item>();
        chests = new List<GD_Chest>();
        heroes = new List<GD_HeroItem>();
        roadMapClaimedRewards = new List<GD_RoadmapReward>();

        heroes.Add(new GD_HeroItem() { heroType = HeroType.DefaultBean, isEquiped = true, cardsCollected = 1 });

        bool isRichStart = true;

        if (isRichStart) {
            var allItemsInfo = MAssets.inst.itemsInfo.GetAllAssets();

            foreach (var item in allItemsInfo) {
                items.Add(new GD_Item() {
                    itemType = item.itemType,
                    fusePoints = 200
                });
                items.Add(new GD_Item() {
                    itemType = item.itemType,
                    fusePoints = 100
                });
            }

            chests.Add(new GD_Chest());
            chests.Add(new GD_Chest() {
                type = ChestType.Epic
            });
            chests.Add(new GD_Chest() {
                type = ChestType.Legendary
            });
            chests.Add(new GD_Chest() {
                type = ChestType.Legendary
            });

            var allHeroesInfo = MAssets.inst.heroesInfo.GetAllAssets();

            foreach (var hero in allHeroesInfo) {
                if (hero.heroType != HeroType.DefaultBean) {
                    heroes.Add(new GD_HeroItem() {
                        heroType = hero.heroType,
                        levelID = 0,
                        cardsCollected = 1000,
                        info = MAssets.inst.heroesInfo.GetAsset(hero.heroType)
                    });
                }
            }
        } else {
            GD_Item pistol = GD_Item.FromRarenessAndLevel(ItemType.Weapon_Pistol, ItemRareness.Common, 3);
            pistol.isEquiped = true;
            items.Add(pistol);
        }
    }

}
