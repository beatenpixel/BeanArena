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
    }

    public override void Draw() {
        itemsToDraw = gdInventory.items.Where(x => x.info.category == config.itemCategory).OrderByDescending(x => x.rareness).ToList();

        itemButtons.Update(itemsToDraw.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
            GD_Item item = itemsToDraw[i];
            ItemButton button = itemButtons[i];

            button.SetItem(item, item.info);
            button.SetArg(i);
            button.rectT.SetAsLastSibling();

            if (item.isEquiped) {
                config.drawer.SetItemButtonEquiped(button);
                //button.SetState(ItemButton.ItemButtonState.InHeroSlot);
            } else {
                //button.SetState(ItemButton.ItemButtonState.InInventory);
            }
        }
    }

    private void OnItemButtonEvent(UIEventType eventType, ItemButton button, object arg) {
        if (eventType == UIEventType.Click) {
            int inventorySlotID = (int)arg;

            config.drawer.Select(button.iconDrawer.rarenessImage.rectTransform);

            selectedItemButton = button;
            infoDrawer.DrawItemInfo(selectedItemButton.currentItem, selectedItemButton);

            Debug.Log("onClick: " + button.currentItem.itemType + button.currentItem.fusePoints);
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

            Debug.Log("ItemA: " + itemA.itemGUID);
            Debug.Log("ItemB: " + itemB.itemGUID);

            ItemStatProgression itemAFuseProg = itemA.info.GetStat(StatType.FusePoints);
            int itemAMaxFusePoints = itemAFuseProg.values[itemAFuseProg.maxLevel - 1].intValue;

            int fusePoints = Mathf.Clamp(itemA.fusePoints + itemB.fusePoints, 0, itemAMaxFusePoints);
            int newLevel = itemAFuseProg.GetLevelByValue(fusePoints);

            itemA.fusePoints = fusePoints;
            itemA.levelID = newLevel;

            Game.data.inventory.items.Remove(itemB);
            itemButtons.Remove(button);

            infoDrawer.DrawItemInfo(itemA, null);
            Draw();
        }
    }
}
