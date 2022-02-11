using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoDrawer_Hero : ItemInfoDrawerBase {

    public HeroInfoPanel heroInfoPanel;

    public override void Init() {
        //base.Init();
    }

    public override void DrawInfo(object item) {
        GD_HeroItem heroItem = (GD_HeroItem)item;
        heroInfoPanel.DrawHero(heroItem);
    }

}
