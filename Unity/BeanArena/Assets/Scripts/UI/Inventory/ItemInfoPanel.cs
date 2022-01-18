using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Polyglot;

public class ItemInfoPanel : MonoBehaviour {

    public IconDrawer isevIconDrawer;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemStatsText;

    public void Init() {
        
    }

    public void DrawInfo(SO_ItemInfo itemInfo, GD_Item item) {
        isevIconDrawer.DrawItem(item, itemInfo);
        itemNameText.text = Localization.Get(itemInfo.itemName_LKey);

        string statsStr = "";
        for (int i = 0; i < itemInfo.stats.Count; i++) {
            string lineStr = "<sprite name=\"";

            switch(itemInfo.stats[i].statType) {
                case StatType.Damage:
                    lineStr += "damage\"> ";
                    break;
                case StatType.Health:
                    lineStr += "heart\"> ";
                    break;
                case StatType.Duration:
                    lineStr += "time\"> ";
                    break;
                case StatType.JumpHeight:
                    lineStr += "jump\"> ";
                    break;
                case StatType.FusePoints:
                    lineStr += "coin\"> ";
                    break;
            }

            lineStr += itemInfo.stats[i].GetValue(item.levelID);
            statsStr += lineStr + "\n";
        }

        itemStatsText.text = statsStr;
    }

}