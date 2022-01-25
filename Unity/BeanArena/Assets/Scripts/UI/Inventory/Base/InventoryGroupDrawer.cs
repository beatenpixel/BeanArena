using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryGroupDrawer : MonoBehaviour {

    public GameObject allObjectsRootGO;
    public RectTransform itemsButtonsRootT;

    public ItemInfoDrawerBase infoDrawer;

    private ObjectListSpawner<ItemButton> itemButtons;
    private GD_Inventory gdInventory;
    private List<GD_Item> itemsToDraw;

    private InventoryGroupConfig config;

    public void Init(InventoryGroupConfig config) {
        this.config = config;

        itemButtons = new ObjectListSpawner<ItemButton>(SpawnItemButton, UpdateItemButton, DisableItemButton);
        gdInventory = Game.data.inventory;
    }

    public void Generate() {

    }

    public void Show(bool show) {
        allObjectsRootGO.SetActive(show);
    }

    public void Draw() {
        itemsToDraw = gdInventory.items.Where(x => x.info.category == config.itemCategory).ToList();

        itemButtons.Spawn(itemsToDraw.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_Item item = itemsToDraw[i];  
            ItemButton button = itemButtons[i];

            button.iconDrawer.DrawItem(item, item.info);
            button.SetArg(i);
        }
    } 

    private void OnItemButtonEvent(UIEventType eventType, ItemButton button, object clickArg) {
        if (eventType == UIEventType.Click) {
            int inventorySlotID = (int)clickArg;

            GD_Item item = itemsToDraw[inventorySlotID];
            SO_ItemInfo itemInfo = MAssets.itemsInfo.GetAsset(item.itemType);

            infoDrawer.DrawInfo(itemInfo, item);
        } else if(eventType == UIEventType.DragStart) {
            config.drawer.currentDragedButton = button;
        } else if(eventType == UIEventType.DragEnd) {
            config.drawer.currentDragedButton = null;
        }
    }

    private ItemButton SpawnItemButton(int ind) {
        ItemButton button = MPool.Get<ItemButton>(null, itemsButtonsRootT);
        button.OnEvent += OnItemButtonEvent;
        return button;
    }

    private void UpdateItemButton(ItemButton obj, int ind, bool isNewObj) {
        obj.gameObject.SetActive(true);
    }

    private void DisableItemButton(ItemButton obj, int ind) {
        obj.gameObject.SetActive(false);
    }

}

public class InventoryGroupConfig {
    public ItemCategory itemCategory;
    public InventoryTabButton tabButton;
    public InventoryDrawer drawer;
}
