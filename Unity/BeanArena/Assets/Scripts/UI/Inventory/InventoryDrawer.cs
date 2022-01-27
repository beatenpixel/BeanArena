using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDrawer : MonoBehaviour {

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
	private InventoryWorldUI.TryPlaceItemResult dragButtonPlaceResult;

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
				worldUI.SetAreaGlowing(true);

				isInsideHeroWorldRect.Set(worldUI.CheckInsideDropArea());

				if (isInsideHeroWorldRect.CheckIsDirtyAndClear()) {
					if (isInsideHeroWorldRect.value) {
						dragButtonPlaceResult = worldUI.TryPlaceItemButton(currentDragedButton);
						Debug.Log(dragButtonPlaceResult.placeResult);

						if (dragButtonPlaceResult.placeResult == SlotPlaceResult.CanReplace) {

						} else if (dragButtonPlaceResult.placeResult == SlotPlaceResult.CanPlace) {
							worldUI.EquipTempItem(currentDragedButton, dragButtonPlaceResult.slot);
						}
					} else {
						worldUI.UnequipTempItem(currentDragedButton);
					}
				}
			} else if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InHeroSlot) {			

				worldUI.SetAreaGlowing(false);

				isInsideInventoryRect.Set(CheckInsideItemsInventoryArea());
			}
		} else {
			worldUI.SetAreaGlowing(false);
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
		switch(e) {
			case UIEventType.DragEnd:			
				if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InHeroSlot) {					
					if (isInsideInventoryRect.value) {
						worldUI.RemoveItemButton(button, false);
						worldUI.UnequipItem(button);
						button.SetState(ItemButton.ItemButtonState.InInventory);						
					} else {
						worldUI.ReturnItemButtonToFrame(button);
                    }
				} else if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InInventory) {
					if (isInsideHeroWorldRect.value) {
						dragButtonPlaceResult = worldUI.TryPlaceItemButton(currentDragedButton);

						if (dragButtonPlaceResult.placeResult == SlotPlaceResult.CanReplace) {
							Debug.Log("Replace");

							if (dragButtonPlaceResult.slot.itemButton != null) {
								Debug.Log("Not null");
								worldUI.RemoveItemButton(dragButtonPlaceResult.slot.itemButton, false);
								worldUI.UnequipItem(dragButtonPlaceResult.slot.itemButton);								
							}

							worldUI.EquipItem(currentDragedButton, dragButtonPlaceResult.slot);
							worldUI.PlaceItemButton(button, dragButtonPlaceResult);

							button.SetState(ItemButton.ItemButtonState.InHeroSlot);
						} else if (dragButtonPlaceResult.placeResult == SlotPlaceResult.CanPlace) {
							worldUI.UnequipTempItem(currentDragedButton);

							worldUI.EquipItem(button, dragButtonPlaceResult.slot);
							worldUI.PlaceItemButton(button, dragButtonPlaceResult);
							button.SetState(ItemButton.ItemButtonState.InHeroSlot);
						} else {

						}						
					}
				}				

				currentDragedButton = null;
				break;
			case UIEventType.DragStart:
				currentDragedButton = button;

				if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InHeroSlot) {

                } else if(currentDragedButton.buttonState == ItemButton.ItemButtonState.InInventory) {

                }

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
