using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Polyglot;

public class ItemInfoPanel : MonoBehaviour {

    public IconDrawer isevIconDrawer;
    public TextMeshProUGUI itemNameText;

    public void Init() {
        
    }

    public void DrawInfo(SO_ItemInfo itemInfo, GD_Item item) {
        isevIconDrawer.DrawItem(item, itemInfo);
        itemNameText.text = Localization.Get(itemInfo.itemName_LKey);
    }

}