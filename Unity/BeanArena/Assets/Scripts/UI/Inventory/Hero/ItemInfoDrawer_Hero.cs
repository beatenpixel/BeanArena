using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoDrawer_Hero : ItemInfoDrawerBase {

    public HeroInfoPanel heroInfoPanel;

    public override void Init() {
        //base.Init();
    }

    public override void DrawHeroInfo(GD_HeroItem item) {
        base.DrawHeroInfo(item);

        heroInfoPanel.DrawHero(item);
    }

}
