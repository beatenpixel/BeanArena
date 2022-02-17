using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryGroupDrawer : MonoBehaviour {

    public RectTransform itemsButtonsRootT;
    public GameObject allObjectsRootGO;
    public ItemInfoDrawerBase infoDrawer;

    protected GD_Inventory gdInventory;

    protected InventoryGroupConfig config;
    public InventoryGroupConfig GetConfig() => config;

    public int groupID => config.groupID;

    public virtual void Init(InventoryGroupConfig config) {
        this.config = config;

        gdInventory = Game.data.inventory;
    }

    public void Generate() {

    }

    public virtual void Show(bool show) {
        allObjectsRootGO.SetActive(show);
    }

    public virtual void Draw() {
        
    } 

    public virtual void PreviewMergeItems(ItemButton button, bool bringItemIn) {

    }

    public virtual void MergeItems(ItemButton button) {

    }

}

public class InventoryGroupConfig {
    public int groupID;
    public ItemCategory itemCategory;
    public InventoryTabButton tabButton;
    public InventoryUI inventoryUI;
}
