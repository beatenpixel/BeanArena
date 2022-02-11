using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryGroupDrawer_Item : InventoryGroupDrawer {

    private ObjectListSpawner<ItemButton> itemButtons;

    private List<GD_Item> itemsToDraw;

    public override void Init(InventoryGroupConfig config) {
        base.Init(config);

        Debug.Log("Init InventoryGroupDrawer_Item");

        itemButtons = new ObjectListSpawner<ItemButton>(SpawnItemButton, Enable_ItemButton, Update_ItemButton);
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

            GD_Item item = itemsToDraw[inventorySlotID];

            Debug.Log("GD_Item: " + item.itemGUID);

            infoDrawer.DrawInfo(item);
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

    private void Update_ItemButton(ItemButton obj, int ind) {

    }

}
