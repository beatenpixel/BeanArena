using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfoDrawerBase : MonoBehaviour {

    public GameObject panelRootGO;
    public ItemInfoPanel itemInfoPanel;

    [HideInInspector] public GD_Item lastDrawnItem { get; private set; }
    [HideInInspector] public ItemButton lastItemButton { get; private set; }

    public virtual void Init() {
        itemInfoPanel.Init();
    }

    public void Show(bool show) {
        panelRootGO.SetActive(show);
    }

    public virtual void DrawHeroInfo(GD_HeroItem item, HeroInfoDrawConfig config) {
        
    }

    public virtual void DrawItemInfo(GD_Item item, ItemButton button) {
        if (item != null) {
            lastDrawnItem = item;
        }
        if (button != null) {
            lastItemButton = button;
        }

        itemInfoPanel.DrawInfo(item);
    }

}
