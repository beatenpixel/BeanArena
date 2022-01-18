using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDrawer : MonoBehaviour {

	public RectTransform holderRectT;

	public ItemInfoPanel itemInfoPanel;

	private ObjectListSpawner<ItemButton> itemButtons;
	private GD_Inventory gdInventory;

	public void Init() {
		itemButtons = new ObjectListSpawner<ItemButton>(SpawnItemButton, UpdateItemButton, DisableItemButton);
		gdInventory = Game.data.inventory;
	}
	
	public void Draw() {
		itemButtons.Spawn(gdInventory.items.Count);

        for (int i = 0; i < itemButtons.activeObjectsCount; i++) {
			GD_Item item = gdInventory.items[i];
			SO_ItemInfo itemInfo = MAssets.itemsInfo.GetAsset(item.itemType);

			ItemButton button = itemButtons[i];

			button.iconDrawer.DrawItem(item, itemInfo);
			button.SetOnClick(OnItemButtonClick, i);
        }
    }

	private void OnItemButtonClick(ItemButton button, object clickArg) {
		int inventorySlotID = (int)clickArg;

		GD_Item item = gdInventory.items[inventorySlotID];
		SO_ItemInfo itemInfo = MAssets.itemsInfo.GetAsset(item.itemType);

		itemInfoPanel.DrawInfo(itemInfo, item);
	}

	private ItemButton SpawnItemButton(int ind) {
		ItemButton button = MPool.Get<ItemButton>(null, holderRectT);
		return button;
    }

	private void UpdateItemButton(ItemButton obj, int ind, bool isNewObj) {
		obj.gameObject.SetActive(true);
	}

	private void DisableItemButton(ItemButton obj, int ind) {
		obj.gameObject.SetActive(false);
	}

}
