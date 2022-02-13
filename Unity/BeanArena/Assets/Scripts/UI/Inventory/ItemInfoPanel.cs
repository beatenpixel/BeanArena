using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour {

    public IconDrawer isevIconDrawer;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemStatsText;

    public void Init() {
        
    }

    public void DrawInfoMerge(ItemButton itemAButton, ItemButton itemBButton) {
        GD_Item itemA = itemAButton.currentItem;
        GD_Item itemB = itemBButton.currentItem;

        Debug.Log("Preview 3");

        ItemStatProgression itemAFuseProg = itemA.info.GetStat(StatType.FusePoints);
        int itemAMaxFusePoints = itemAFuseProg.values[itemAFuseProg.maxLevel - 1].intValue;

        int fusePoints = Mathf.Clamp(itemA.fusePoints + itemB.fusePoints, 0, itemAMaxFusePoints);
        int newLevel = itemAFuseProg.GetLevelByValue(fusePoints);

        Debug.Log($"Fuse {fusePoints} Level: {newLevel}");

        bool hasFuse = itemA.info.GetFusePointsPercent(fusePoints, newLevel, out float fuseProgress);

        isevIconDrawer.DrawItemMerged(itemA, new MergedItemInfo() {
            prevLevel = itemA.levelID,
            newLevel = newLevel,
            levelChanged = itemA.levelID != newLevel,
            progressBar = fuseProgress
        });

        itemNameText.text = MLocalization.Get(itemA.info.itemName_LKey);

        string statsStr = "";
        for (int i = 0; i < itemA.info.stats.Count; i++) {
            if (itemA.info.stats[i].statType == StatType.FusePoints)
                continue;

            string lineStr = GetTMProStringForStatType(itemA.info.stats[i].statType);            

            lineStr += itemA.info.stats[i].GetDifferenceStr(itemA.levelID, newLevel);

            statsStr += lineStr + "\n";
        }

        itemStatsText.text = statsStr;
    }

    public void DrawInfo(GD_Item item) {
        SO_ItemInfo itemInfo = item.info;

        isevIconDrawer.DrawItem(item, itemInfo);
        itemNameText.text = MLocalization.Get(itemInfo.itemName_LKey);

        string statsStr = "";
        for (int i = 0; i < itemInfo.stats.Count; i++) {
            if (itemInfo.stats[i].statType == StatType.FusePoints)
                continue;

            string lineStr = GetTMProStringForStatType(itemInfo.stats[i].statType);

            if (itemInfo.stats[i].statType == StatType.FusePoints) {
                lineStr += item.fusePoints;
            } else {                
                lineStr += itemInfo.stats[i].GetValue(item.levelID);
            }

            statsStr += lineStr + "\n";
        }

        itemStatsText.text = statsStr;
    }

    public static string GetTMProStringForStatType(StatType statType) {
        string statIconStr = "<sprite name=\"";

        switch (statType) {
            case StatType.Damage:
                statIconStr += "damage\"> ";
                break;
            case StatType.Health:
                statIconStr += "heart\"> ";
                break;
            case StatType.Duration:
                statIconStr += "time\"> ";
                break;
            case StatType.JumpHeight:
                statIconStr += "jump\"> ";
                break;
            case StatType.FusePoints:
                statIconStr += "coin\"> ";
                break;
        }

        return statIconStr;
    }

}