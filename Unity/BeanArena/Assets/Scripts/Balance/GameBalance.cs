using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameBalance  {

    public static int REWARDS_SEED = 91201;

    public static int MAX_MMR = 10000;

    private static int[] CHEST_TYPE_CHANCE = new int[] {
        50, 5, 1
    };

    private static int[] ITEM_RARENESS_CHANCE = new int[] {
        100, 20, 7, 3, 1
    };

    private static int[] ITEM_LEVEL_CHANCE = new int[] {
        50, 7, 2
    };

    private static int[] LEVELS_ARRAY = new int[] {
        0,1,2
    };

    public static Dictionary<ItemRareness, float> rarenessToMulUp = new Dictionary<ItemRareness, float>() {
        {ItemRareness.Common, 1f },
        {ItemRareness.Uncommon, 1.1f },
        {ItemRareness.Rare, 1.2f },
        {ItemRareness.Epic, 1.3f },
        {ItemRareness.Legendary, 1.4f },
    };

    public static Dictionary<ItemRareness, float> rarenessToMulDown = new Dictionary<ItemRareness, float>() {
        {ItemRareness.Common, 1f },
        {ItemRareness.Uncommon, 0.95f },
        {ItemRareness.Rare, 0.9f },
        {ItemRareness.Epic, 0.85f },
        {ItemRareness.Legendary, 0.8f },
    };


    private static ChestType[] chestTypes;
    private static ItemRareness[] itemRarenesses;

    public static void Init() {
        chestTypes = EnumUtils.GetValues<ChestType>().ToArray();
        itemRarenesses = EnumUtils.GetValues<ItemRareness>().ToArray();
    }

    public static GD_HeroItem GenerateEnemyData_Arena() {
        int playerMMR = Game.data.player.mmr;

        GD_HeroItem data = new GD_HeroItem() {
            cardsCollected = 0,
            heroType = MRandom.Get(
                new RandomEntry<HeroType>(HeroType.DefaultBean, 10),
                new RandomEntry<HeroType>(HeroType.Skeleton, 2),
                new RandomEntry<HeroType>(HeroType.Robber, 5)
            ),
            levelID = (playerMMR / 50),
            info = MAssets.heroesInfo.GetAsset(HeroType.DefaultBean)
        };

        return data;
    }

    public static RoadmapContainer GenerateRoadmapContainer() {
        RoadmapContainer container = new RoadmapContainer();

        container.AddReward(new RoadmapReward(CurrencyType.Coin, 100).MMR(10));
        container.AddReward(new RoadmapReward(CurrencyType.Gem, 10).MMR(10));
        container.AddReward(new RoadmapReward(new HeroCardsContainer() { heroType = HeroType.Skeleton, amount = 4 }).MMR(10));
        container.AddReward(new RoadmapReward(CurrencyType.Coin, 30).MMR(20));
        container.AddReward(new RoadmapReward(CurrencyType.Coin, 30).MMR(20));
        container.AddReward(new RoadmapReward(CurrencyType.Coin, 30).MMR(20));

        return container;
    }

    public static GD_Chest GenerateRoundRewardChest() {
        ChestType chestType = MRandom.Get(new RandomEntry<ChestType>(ChestType.Common, CHEST_TYPE_CHANCE[0]), new RandomEntry<ChestType>(ChestType.Epic, CHEST_TYPE_CHANCE[1]), new RandomEntry<ChestType>(ChestType.Legendary, CHEST_TYPE_CHANCE[2]));

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

        if (MRandom.Range(0, 10) > 5) {            

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

        int itemCount = (int)content.chestType + MRandom.Range(0, 2);

        var allItemsInfo = MAssets.itemsInfo.GetAllAssets();
        List<RandomEntry<ItemType>> itemsTypes = new List<RandomEntry<ItemType>>();
        for (int x = 0; x < allItemsInfo.Count; x++) {
            itemsTypes.Add(new RandomEntry<ItemType>(allItemsInfo[x].itemType, allItemsInfo[x].dropInfo.dropWeight));
        }

        for (int i = 0; i < itemCount; i++) {           

            ItemType itemType = MRandom.Get(itemsTypes);

            SO_ItemInfo itemInfo = MAssets.itemsInfo.GetAsset(itemType);
            int level = MRandom.GetWithChance(LEVELS_ARRAY, ITEM_LEVEL_CHANCE);

            GD_Item randItem = new GD_Item() {
                info = itemInfo,
                itemType = itemType,
                fusePoints = 0,
                levelID = level,
                rareness = MRandom.GetWithChance(itemRarenesses, ITEM_RARENESS_CHANCE),
            };

            Vector2Int fusePointsBounds = itemInfo.GetFusePointsBounds(randItem, level);
            randItem.fusePoints = MRandom.Range(fusePointsBounds.x, fusePointsBounds.y);

            content.items.Add(randItem);
        }

        Debug.Log(content.ToString());

        return content;
    }

}

[System.Serializable]
public class DropInfo {
    public int dropWeight = 50;
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