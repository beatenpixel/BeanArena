using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoDrawer_Hero : ItemInfoDrawerBase {

    public HeroInfoPanel heroInfoPanel;

    public override void Init() {
        //base.Init();
    }

    public override void DrawHeroInfo(GD_HeroItem item, HeroInfoDrawConfig config) {
        base.DrawHeroInfo(item, config);

        heroInfoPanel.DrawHero(item, config);
    }

}
