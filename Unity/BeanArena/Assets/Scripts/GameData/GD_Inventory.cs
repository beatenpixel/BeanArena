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


        List<SO_HeroInfo> allHeroesInfo = MAssets.heroesInfo.GetAllAssets();

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
        chests = new List<GD_Chest>();
        heroes = new List<GD_HeroItem>();

        heroes.Add(new GD_HeroItem() { heroType = HeroType.DefaultBean, isEquiped = true, cardsCollected = 1 });

        bool isRichStart = false;

        if (isRichStart) {
            var allItemsInfo = MAssets.itemsInfo.GetAllAssets();

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

            var allHeroesInfo = MAssets.heroesInfo.GetAllAssets();

            foreach (var hero in allHeroesInfo) {
                heroes.Add(new GD_HeroItem() {
                    heroType = hero.heroType,
                    levelID = 0,
                    cardsCollected = 1000,
                    info = MAssets.heroesInfo.GetAsset(hero.heroType)
                });
            }
        } else {
            items.Add(new GD_Item() {
                itemType = ItemType.Weapon_Pistol,
                fusePoints = 0,
                isEquiped = true,                
            });
        }
    }

}
