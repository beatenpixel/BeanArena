using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryGroupDrawer_Item : InventoryGroupDrawer {

    private ObjectListSpawner<ItemButton> itemButtons;

    private List<GD_Item> itemsToDraw;
    private ItemButton selectedItemButton;

    public override void Init(InventoryGroupConfig config) {
        base.Init(config);

        Debug.Log("Init InventoryGroupDrawer_Item");

        itemButtons = new ObjectListSpawner<ItemButton>(SpawnItemButton, Enable_ItemButton, Update_ItemButton, DestroyItemButton);
    }

    public override void Show(bool show) {
        base.Show(show);

        CollectItemsToDraw();
        UpdateTabNotification();

        if (show) {
            if (!config.inventoryUI.showedAnyItemInfo) {

                if (itemsToDraw.Count > 0) {
                    infoDrawer.Show(true);
                    infoDrawer.DrawItemInfo(itemsToDraw[0], itemButtons[0]);
                    config.inventoryUI.Select(itemButtons[0].iconDrawer.rarenessImage.rectTransform);

                    config.inventoryUI.showedAnyItemInfo = true;
                }
            }

            if (config.inventoryUI.showedAnyItemInfo) {
                infoDrawer.Show(true);
            }
        }        
    }

    public void UpdateTabNotification() {
        int newItemsCount = 0;
        for (int i = 0; i < itemsToDraw.Count; i++) {
            if (itemsToDraw[i].isNew)
                newItemsCount += 1;
        }

        if (newItemsCount > 0) {
            config.tabButton.notificationDot.Enable(true, newItemsCount);
        } else {
            config.tabButton.notificationDot.Enable(false);
        }
    }

    private void CollectItemsToDraw() {
        itemsToDraw = gdInventory.items.Where(x => x.info.category == config.itemCategory).OrderByDescending(x => x.rareness).ToList();
    }

    public override void Draw() {
        CollectItemsToDraw();

        itemButtons.Update(itemsToDraw.Count);       

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_Item item = itemsToDraw[i];
            ItemButton button = itemButtons[i];

            button.SetItem(item, item.info);
            button.SetArg(i);
            button.rectT.SetAsLastSibling();

            if (item.isEquiped) {
                config.inventoryUI.SetItemButtonEquiped(button);
                //button.SetState(ItemButton.ItemButtonState.InHeroSlot);
            } else {
                //button.SetState(ItemButton.ItemButtonState.InInventory);
            }
        }
    }

    private void OnItemButtonEvent(UIEventType eventType, ItemButton button, object arg) {
        switch(eventType) {
            case UIEventType.Click:
                selectedItemButton = button;

                if (selectedItemButton.currentItem.isNew) {
                    selectedItemButton.currentItem.isNew = false;

                    selectedItemButton.Redraw();
                    UpdateTabNotification();
                }
                
                config.inventoryUI.Select(button.iconDrawer.rarenessImage.rectTransform);                
                infoDrawer.DrawItemInfo(selectedItemButton.currentItem, selectedItemButton);

                break;
            case UIEventType.DragStart:
                //CAN_NOT_MERGE_ITSELF

                if (button.currentItem.isNew) {
                    button.currentItem.isNew = false;

                    button.Redraw();
                    UpdateTabNotification();
                }

                if (button == infoDrawer.lastItemButton) {
                    config.inventoryUI.ShowItemInfoBorderNotification(MLocalization.Get("CAN_NOT_MERGE_ITSELF"));
                } else {
                    config.inventoryUI.ShowItemInfoBorderNotification(MLocalization.Get("UPGRADE"));
                }

                break;
        }

        config.inventoryUI.OnItemButtonEvent(eventType, button, arg);
    }

    private ItemButton SpawnItemButton(int ind) {
        ItemButton button = MPool.Get<ItemButton>(null, itemsButtonsRootT);
        button.Init_ItemButton(this);
        button.SetState(ItemButton.ItemButtonState.InInventory);
        button.OnEvent += OnItemButtonEvent;
        return button;
    }

    private void Enable_ItemButton(ItemButton obj, int ind, bool enable) {
        obj.gameObject.SetActive(enable);
    }

    private void DestroyItemButton(ItemButton button, int ind) {
        button.Push();
        button.OnEvent -= OnItemButtonEvent;
    }

    private void Update_ItemButton(ItemButton obj, int ind) {

    }

    public override void PreviewMergeItems(ItemButton button, bool bringItemIn) {
        base.PreviewMergeItems(button, bringItemIn);

        if (bringItemIn) {

            if (infoDrawer.lastItemButton != null && button.currentItem != null && button != infoDrawer.lastItemButton) {
                GD_Item itemA = infoDrawer.lastDrawnItem;
                GD_Item itemB = button.currentItem;

                infoDrawer.itemInfoPanel.DrawInfoMerge(infoDrawer.lastItemButton,button);
            }
        } else {
            if (infoDrawer.lastDrawnItem != null) {
                infoDrawer.DrawItemInfo(infoDrawer.lastDrawnItem, infoDrawer.lastItemButton);
            }
        }
    }

    public override void MergeItems(ItemButton button) {
        base.MergeItems(button);

        if (infoDrawer.lastItemButton != null && button.currentItem != null && button != infoDrawer.lastItemButton) {
            GD_Item itemA = infoDrawer.lastDrawnItem;
            GD_Item itemB = button.currentItem;

            ItemsMergeResult mergeResult = GD_Item.TestMerge(itemA, itemB);            

            itemA.fusePoints = mergeResult.newFusePoints;
            itemA.levelID = mergeResult.newLevel;

            Game.data.inventory.items.Remove(itemB);
            itemButtons.Remove(button);

            infoDrawer.DrawItemInfo(itemA, null);
            Draw();
        }
    }
}

public struct ItemsMergeResult {
    public int newFusePoints;
    public int newLevel;
}
