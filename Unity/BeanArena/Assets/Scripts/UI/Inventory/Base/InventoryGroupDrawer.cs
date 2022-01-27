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

        itemButtons.Update(itemsToDraw.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_Item item = itemsToDraw[i];  
            ItemButton button = itemButtons[i];

            button.SetItem(item, item.info);
            button.SetArg(i);

            if(item.isEquiped) {
                //button.SetState(ItemButton.ItemButtonState.InHeroSlot);
            } else {
                //button.SetState(ItemButton.ItemButtonState.InInventory);
            }
        }
    } 

    private void OnItemButtonEvent(UIEventType eventType, ItemButton button, object arg) {
        if (eventType == UIEventType.Click) {
            int inventorySlotID = (int)arg;

            GD_Item item = itemsToDraw[inventorySlotID];

            infoDrawer.DrawInfo(item.info, item);
        }

        config.drawer.OnItemButtonEvent(eventType, button, arg);
    }

    private ItemButton SpawnItemButton(int ind) {
        ItemButton button = MPool.Get<ItemButton>(null, itemsButtonsRootT);
        button.Init_ItemButton(this);
        button.SetState(ItemButton.ItemButtonState.InInventory);
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
