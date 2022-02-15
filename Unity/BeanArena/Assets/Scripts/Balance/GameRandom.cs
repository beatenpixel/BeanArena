using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameRandom  {

    private static int[] CHEST_TYPE_CHANCE = new int[] {
        50, 5, 1
    };

    private static int[] ITEM_RARENESS_CHANCE = new int[] {
        50, 20, 10, 5, 1
    };

    private static int[] ITEM_LEVEL_CHANCE = new int[] {
        50, 15, 7, 3
    };

    private static int[] LEVELS_ARRAY = new int[] {
        0,1,2,3
    };

    private static ChestType[] chestTypes;
    private static ItemRareness[] itemRarenesses;

    public static void Init() {
        chestTypes = EnumUtils.GetValues<ChestType>().ToArray();
        itemRarenesses = EnumUtils.GetValues<ItemRareness>().ToArray();
    }

    public static GD_Chest GenerateRoundRewardChest() {
        ChestType chestType = MRandom.Get(new RandomEntry<ChestType>(ChestType.Common, 50), new RandomEntry<ChestType>(ChestType.Common, 7), new RandomEntry<ChestType>(ChestType.Common, 1));

        GD_Chest chest = new GD_Chest() {
            type = chestType
        };

        chest.info = MAssets.chestsInfo.GetAsset(chest.type);
        return chest;
    }

    public static ChestContent GenerateChestContent(GD_Chest chest) {
        ChestContent content = new ChestContent();

        content.chestType = MRandom.GetWithChance(chestTypes, CHEST_TYPE_CHANCE);
        if(MRandom.Range(0,5) > 3) {
            content.coins = MRandom.Range(20, 50);
        } else {
            content.coins = 0;
        }

        content.heroCards = new List<HeroCardsContainer>();

        if (MRandom.Range(0, 10) > -1) {            

            List<RandomEntry<HeroType>> heroesThatAreNotFull = new List<RandomEntry<HeroType>>();

            foreach (var hero in Game.data.inventory.heroes) {
                if (hero.levelID < hero.info.rarenessInfo.maxLevel) {
                    heroesThatAreNotFull.Add(new RandomEntry<HeroType>(hero.heroType, hero.info.rarenessInfo.randomDropWeight));
                }
            }

            HeroType heroCard = MRandom.Get(heroesThatAreNotFull);
            SO_HeroInfo heroInfo = MAssets.heroesInfo.GetAsset(heroCard);

            content.heroCards.Add(new HeroCardsContainer() {
                heroType = heroCard,
                amount = MRandom.Range(heroInfo.rarenessInfo.dropAmount.x, heroInfo.rarenessInfo.dropAmount.y)
            });
            //HeroType heroType = heroesThatAreNotFull MRandom.Range(0, GD_HeroItem.HeroTypeCount);
        }

        content.items = new List<GD_Item>();

        int itemCount = (int)content.chestType + MRandom.Range(1, 3);
        for (int i = 0; i < itemCount; i++) {
            ItemType itemType = MRandom.Get(new RandomEntry<ItemType>(ItemType.Pistol, 10), new RandomEntry<ItemType>(ItemType.Wheel, 5));
            SO_ItemInfo itemInfo = MAssets.itemsInfo.GetAsset(itemType);
            int level = MRandom.GetWithChance(LEVELS_ARRAY, ITEM_LEVEL_CHANCE);
            Vector2Int fusePointsBounds = itemInfo.GetFusePointsBounds(level);

            GD_Item randItem = new GD_Item() {
                info = itemInfo,
                itemType = itemType,
                fusePoints = MRandom.Range(fusePointsBounds.x, fusePointsBounds.y),
                levelID = level,
                rareness = MRandom.GetWithChance(itemRarenesses, ITEM_RARENESS_CHANCE),
            };

            content.items.Add(randItem);
        }

        Debug.Log(content.ToString());

        return content;
    }

}


public class ChestContent {
    public ChestType chestType;
    public List<GD_Item> items;
    public List<HeroCardsContainer> heroCards;
    public int coins;

    public override string ToString() {
        string s = $"ChestContent[{chestType}] COINS:{coins}";
        for (int i = 0; i < items.Count; i++) {
            s += "\n" + items[i].ToString();
        }
        return s;
    }
}