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
            string lineStr = GetTMProStringForStatType(itemInfo.stats[i].statType); 

            lineStr += itemInfo.stats[i].GetValue(item.levelID);
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