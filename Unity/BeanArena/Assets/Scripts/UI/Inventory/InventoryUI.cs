using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

	public InventoryState inventoryState;

	public RectTransform tabButtonsHolderRectT;
	public RectTransform inventoryGroupsHolderT;

	public InventoryWorldUI worldUI;
	public List<InventoryGroupDrawer> groupDrawers;

	private ObjectListSpawner<InventoryTabButton> tabButtons;
	private bool generatedElements;
	private int currentGroupID = 0;
	private ChangeCheck<bool> isInsideHeroWorldRect = new ChangeCheck<bool>(false);
	private ChangeCheck<bool> isInsideInventoryRect = new ChangeCheck<bool>(false);

	[HideInInspector] public ItemButton currentDragedButton;

	private Vector3[] inventoryWorldCorners = new Vector3[4];

	public void Init() {
		tabButtons = new ObjectListSpawner<InventoryTabButton>(Spawn_InventoryTabButton, Update_InventoryTabButton, Disable_InventoryTabButton);
		tabButtons.Update(groupDrawers.Count);

		for (int i = 0; i < tabButtons.activeObjectsCount; i++) {
			tabButtons[i].SetOnClick(OnTabButtonClick, i);
		}

		groupDrawers[0].Init(new InventoryGroupConfig() {
			itemCategory = ItemCategory.Weapon,
			tabButton = tabButtons[0],
			drawer = this,
		});

		groupDrawers[1].Init(new InventoryGroupConfig() {
			itemCategory = ItemCategory.BottomGadget,
			tabButton = tabButtons[1],
			drawer = this,
		});

		if (!generatedElements) {
			generatedElements = true;

            for (int i = 0; i < groupDrawers.Count; i++) {
				groupDrawers[i].Generate();
			}
		}

		worldUI.Init();
	}

    private void Update() {
		if (currentDragedButton != null) {
			if (currentDragedButton.buttonState == ItemButton.ItemButtonState.InInventory) {				

			} else if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InHeroSlot) {			
				
			}
		}
    }

    public void Draw() {
        for (int i = 0; i < groupDrawers.Count; i++) {
			tabButtons[i].SetActive(i == currentGroupID);

			if(i == currentGroupID) {
				groupDrawers[i].Draw();
				groupDrawers[i].Show(true);
			} else {
				groupDrawers[i].Show(false);
			}
        }
	}

	public void OnItemButtonEvent(UIEventType e, ItemButton button, object arg) {
		switch (e) {
			case UIEventType.DragEnd:

				if (button.buttonState == ItemButton.ItemButtonState.InInventory) {
					if (worldUI.CheckInsideDropArea()) {
						GD_Item item = button.currentItem;

						List<EquipmentSlot> heroFreeSlots = worldUI.targetHero.heroEquipment.GetFreeSlots(item);
						if (heroFreeSlots.Count > 0) {

                            for (int i = 0; i < heroFreeSlots.Count; i++) {
								EquipmentSlot slot = heroFreeSlots[0];
								WorldItemSlot worldSlot = worldUI.GetWorldSlot(slot);

								if (slot.HasPreviewItem()) {
									worldSlot.ClearItemButton();
									worldSlot.ClearPreviewItem();
								}

								worldUI.targetHero.heroEquipment.PreviewItem(item, slot);								
								worldSlot.SetItemButton(button);
							}							
						}
					}
				} else if(button.buttonState == ItemButton.ItemButtonState.InHeroSlot) {
					if(CheckInsideItemsInventoryArea()) {
						GD_Item item = button.currentItem;
						WorldItemSlot worldSlot = worldUI.GetWorldSlot(button);						

						worldSlot.ClearItemButton();
						worldSlot.ClearPreviewItem();						
                    }
                }				

				currentDragedButton = null;
				break;
			case UIEventType.DragStart:
				currentDragedButton = button;
				break;
        }
    }

	public bool CheckInsideItemsInventoryArea() {
		Vector2 screenPos = Input.mousePosition;
		inventoryGroupsHolderT.GetWorldCorners(inventoryWorldCorners);
		bool isInside = MUtils.PointIsInsideCorners(screenPos, inventoryWorldCorners);
		return isInside;
	}

	private void OnTabButtonClick(InventoryTabButton tabButton, object arg) {
		int tabID = (int)arg;

		currentGroupID = tabID;
		Draw();
    }

	#region InventoryTabButton

	private InventoryTabButton Spawn_InventoryTabButton(int ind) {
		InventoryTabButton button = MPool.Get<InventoryTabButton>(null, tabButtonsHolderRectT);
		return button;
	}

	private void Update_InventoryTabButton(InventoryTabButton obj, int ind, bool isNewObj) {
		obj.gameObject.SetActive(true);
	}

	private void Disable_InventoryTabButton(InventoryTabButton obj, int ind) {
		obj.gameObject.SetActive(false);
	}

    #endregion

	public enum InventoryState {
		None,
		DragFromInventoryToHero,
		DragFromHeroToInventory
    }

}
