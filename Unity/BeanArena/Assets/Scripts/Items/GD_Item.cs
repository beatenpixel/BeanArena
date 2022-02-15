using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class GD_Item : GD {

    public ItemType itemType;
    public ItemRareness rareness;
    public int levelID;
    public int fusePoints;
    public bool isEquiped;
    [NonSerialized] public SO_ItemInfo info;
    public string itemGUID;
    public bool isNew;

    public GD_Item() : base(GDType.ItemData, GDLoadOrder.Default) {
        SetDefaults(default);
    }

    public void Restore() {

    }

    public GD_Item(SerializationInfo info, StreamingContext sc) : base(info, sc) {
        itemType = (ItemType)info.GetByte("itemType");
        rareness = (ItemRareness)info.GetByte("rareness");
        levelID = info.GetInt32("level");
        fusePoints = info.GetInt32("fusePoints");
        isEquiped = info.GetBoolean("isEquiped");
        itemGUID = info.GetString("itemGUID");
        isNew = info.GetBoolean("isNew");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
        info.AddValue("itemType", (byte)itemType);
        info.AddValue("rareness", (byte)rareness);
        info.AddValue("level", levelID);
        info.AddValue("fusePoints", fusePoints);
        info.AddValue("isEquiped", isEquiped);
        info.AddValue("itemGUID", itemGUID);
        info.AddValue("isNew", isNew);
    }

    [OnDeserializing]
    protected override void SetDefaults(StreamingContext ds) {
        itemType = ItemType.None;
        rareness = ItemRareness.Common;
        levelID = 0;
        fusePoints = 0;
        isEquiped = false;
        itemGUID = Guid.NewGuid().ToString();
        isNew = true;
    }

    public override string ToString() {
        return $"GD_Item[{itemType}] R:{rareness} LVL:{levelID} F:{fusePoints}";
    }

    public StatValue GetStatValue(StatType statType) {
        return info.GetStat(statType).values[levelID];
    }

    public static ItemsMergeResult TestMerge(GD_Item itemA, GD_Item itemB) {
        ItemsMergeResult result = new ItemsMergeResult();

        ItemStatProgression itemAFuseProg = itemA.info.GetStat(StatType.FusePoints);
        int itemAMaxFusePoints = itemAFuseProg.values[itemAFuseProg.maxLevel - 1].intValue;

        float rarenessCoeff = MUtils.rarenessFuseCoeff[itemB.rareness] / MUtils.rarenessFuseCoeff[itemA.rareness];

        if((int)itemB.rareness > (int)itemA.rareness) {
            rarenessCoeff *= 0.8f;
        }

        int fuseAdd = Mathf.RoundToInt(itemB.fusePoints * rarenessCoeff);

        result.newFusePoints = Mathf.Clamp(itemA.fusePoints + fuseAdd, 0, itemAMaxFusePoints);
        result.newLevel = itemAFuseProg.GetLevelByValue(result.newFusePoints);

        return result;
    }

}

public class ItemFilter {

    private ItemCategory category;
    private bool useCategoryFilter;

    public ItemFilter(ItemCategory itemCategory) {
        category = itemCategory;
        useCategoryFilter = true;
    }

    public bool Check(GD_Item item) {
        bool canPass = true;

        if(useCategoryFilter) {
            if(item.info.category != category) {
                return false;
            }
        }

        return canPass;
    }

}

public enum ItemCategory {
    Hero,
    HeroSkin,
    Weapon,
    BottomGadget,
    UpperGadget
}

public enum ItemType : byte {
    None,
    Pistol,
    WaterPistol,
    HelicopterHat,
    JumpyBoots,
    RocketBoots,
    Wheel
}

public enum ItemRareness : byte {
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary
}