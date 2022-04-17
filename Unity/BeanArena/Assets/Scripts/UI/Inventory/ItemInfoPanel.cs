using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInfoPanel : MonoBehaviour {

    public IconDrawer isevIconDrawer;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescrText;
    public TextMeshProUGUI itemStatsText;

    public void Init() {
        
    }

    public void DrawInfoMerge(ItemButton itemAButton, ItemButton itemBButton) {
        GD_Item itemA = itemAButton.currentItem;
        GD_Item itemB = itemBButton.currentItem;

        ItemsMergeResult mergeResult = GD_Item.TestMerge(itemA, itemB);

        bool hasFuse = itemA.info.GetFusePointsPercent(itemA, mergeResult.newFusePoints, mergeResult.newLevel, out float fuseProgress);

        isevIconDrawer.DrawItemMerged(itemA, new MergedItemInfo() {
            prevLevel = itemA.levelID,
            newLevel = mergeResult.newLevel,
            levelChanged = itemA.levelID != mergeResult.newLevel,
            progressBar = fuseProgress
        });

        itemNameText.text = MLocalization.Get(itemA.info.localizationKeyName, LocalizationGroup.Items);
        itemDescrText.text = MLocalization.Get(itemA.info.localizationKeyDescr, LocalizationGroup.Items);

        string statsStr = "";
        for (int i = 0; i < itemA.info.stats.Count; i++) {
            if (itemA.info.stats[i].statType == StatType.FusePoints)
                continue;

            string lineStr = MFormat.GetTMProIcon(itemA.info.stats[i].statType);

            lineStr += itemA.info.stats[i].GetDifferenceStr(itemA.levelID, mergeResult.newLevel, StatConfig.FromItem(itemA));

            statsStr += lineStr + "\n";
        }

        itemStatsText.text = statsStr;
    }

    public void DrawInfo(GD_Item item) {
        SO_ItemInfo itemInfo = item.info;

        isevIconDrawer.DrawItem(item, itemInfo);
        isevIconDrawer.EnableRedDot(false);

        itemNameText.text = MLocalization.Get(itemInfo.localizationKeyName, LocalizationGroup.Items);
        itemDescrText.text = MLocalization.Get(itemInfo.localizationKeyDescr, LocalizationGroup.Items);

        string statsStr = "";
        for (int i = 0; i < itemInfo.stats.Count; i++) {
            if (itemInfo.stats[i].statType == StatType.FusePoints)
                continue;

            string lineStr = MFormat.GetTMProIcon(itemInfo.stats[i].statType);

            if (itemInfo.stats[i].statType == StatType.FusePoints) {
                lineStr += item.fusePoints;
            } else {                
                lineStr += itemInfo.stats[i].GetValueStr(item.levelID, StatConfig.FromItem(item));
            }

            statsStr += lineStr + "\n";
        }

        itemStatsText.text = statsStr;
    }

}