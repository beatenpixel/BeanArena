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
    private bool groupsAreSetup;

	public void Init() {
        groupsAreSetup = false;

        tabButtons = new ObjectListSpawner<InventoryTabButton>(Spawn_InventoryTabButton, Enable_InventoryTabButton, Update_InventoryTabButton);
		tabButtons.Update(groupDrawers.Count);

		for (int i = 0; i < tabButtons.activeObjectsCount; i++) {
			tabButtons[i].SetOnClick(OnTabButtonClick, i);
		}

        List<ItemCategory> groupsCategory = new List<ItemCategory>() {
            ItemCategory.Weapon,
            ItemCategory.BottomGadget,
            ItemCategory.UpperGadget,
            ItemCategory.Hero
        };

        for (int i = 0; i < 4; i++) {
            groupDrawers[i].Init(new InventoryGroupConfig() {
                groupID = i,
                itemCategory = groupsCategory[i],
                tabButton = tabButtons[i],
                drawer = this,
            });
        }       

        groupsAreSetup = true;

        tabButtons.Update(groupDrawers.Count);

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

    public void Draw(bool updateAll) {
        for (int i = 0; i < groupDrawers.Count; i++) {
			tabButtons[i].SetActive(i == currentGroupID);

            if(updateAll) {
                groupDrawers[i].Draw();

                if (i == currentGroupID) {
                    groupDrawers[i].Show(true);
                } else {
                    groupDrawers[i].Show(false);
                }
            } else {
                if (i == currentGroupID) {
                    groupDrawers[i].Draw();
                    groupDrawers[i].Show(true);
                } else {
                    groupDrawers[i].Show(false);
                }
            }			
        }
	}

	public void OnItemButtonEvent(UIEventType e, ItemButton button, object arg) {
		switch (e) {
			case UIEventType.DragEnd:

				if (button.buttonState == ItemButton.ItemButtonState.InInventory) {
					if (worldUI.CheckInsideDropArea()) {
                        SetItemButtonEquiped(button);
					}
				} else if(button.buttonState == ItemButton.ItemButtonState.InHeroSlot) {
					if(CheckInsideItemsInventoryArea()) {
						GD_Item item = button.currentItem;
						WorldItemSlot worldSlot = worldUI.GetWorldSlot(button);						

						worldSlot.ClearItemButton();
						worldSlot.ClearPreviewItem();

                        item.isEquiped = false;
                    }

                    if(currentGroupID != button.inventoryGroupDrawer.groupID) {
                        SwitchTab(button.inventoryGroupDrawer.groupID);
                    }
                }				

				currentDragedButton = null;
				break;
			case UIEventType.DragStart:
				currentDragedButton = button;
				break;
        }
    }

    public void SpawnPreviewItems() {
        for (int i = 0; i < Game.data.inventory.items.Count; i++) {
            GD_Item item = Game.data.inventory.items[i];

            if (!item.isEquiped) {
                continue;
            }

            List<EquipmentSlot> heroFreeSlots = worldUI.targetHero.heroEquipment.GetFreeSlots(item);
            if (heroFreeSlots.Count > 0) {
                EquipmentSlot equipSlot = heroFreeSlots[0];
                WorldItemSlot worldSlot = worldUI.GetWorldSlot(equipSlot);

                for (int x = 0; x < heroFreeSlots.Count; x++) {
                    if (equipSlot.HasPreviewItem()) {
                        equipSlot.ClearPreviewItem();
                        worldSlot.ClearItemButton();
                        worldSlot.ClearPreviewItem();
                    }

                    worldUI.targetHero.heroEquipment.PreviewItem(item, equipSlot);
                    break;
                }
            }
        }
    }

    public void SetItemButtonEquiped(ItemButton button) {
        GD_Item item = button.currentItem;

        List<EquipmentSlot> heroFreeSlots = worldUI.targetHero.heroEquipment.GetFreeSlots(item);
        if (heroFreeSlots.Count > 0) {

            for (int i = 0; i < heroFreeSlots.Count; i++) {
                EquipmentSlot slot = heroFreeSlots[0];
                WorldItemSlot worldSlot = worldUI.GetWorldSlot(slot);

                SetItemButtonEquiped(button, slot, worldSlot);
                item.isEquiped = true;

                break;
            }
        }
    }

    public void SetItemButtonEquiped(ItemButton button, EquipmentSlot equipSlot, WorldItemSlot worldSlot) {
        GD_Item item = button.currentItem;

        if (equipSlot.HasPreviewItem()) {            
            worldSlot.ClearItemButton();
            worldSlot.ClearPreviewItem();
            equipSlot.ClearPreviewItem();
        }

        worldUI.targetHero.heroEquipment.PreviewItem(item, equipSlot);
        worldSlot.SetItemButton(button);
    }

	public bool CheckInsideItemsInventoryArea() {
		Vector2 screenPos = Input.mousePosition;
		inventoryGroupsHolderT.GetWorldCorners(inventoryWorldCorners);
		bool isInside = MUtils.PointIsInsideCorners(screenPos, inventoryWorldCorners);
		return isInside;
	}

	private void OnTabButtonClick(InventoryTabButton tabButton, object arg) {
		int tabID = (int)arg;

        SwitchTab(tabID);		
    }

    private void SwitchTab(int tabID) {
        currentGroupID = tabID;
        Draw(false);
    }

	#region InventoryTabButton

	private InventoryTabButton Spawn_InventoryTabButton(int ind) {
		InventoryTabButton button = MPool.Get<InventoryTabButton>(null, tabButtonsHolderRectT);
		return button;
	}

	private void Enable_InventoryTabButton(InventoryTabButton obj, int ind, bool enable) {
        obj.gameObject.SetActive(enable);
    }

	private void Update_InventoryTabButton(InventoryTabButton obj, int ind) {
        obj.gameObject.SetActive(true);

        if (groupsAreSetup) {
            InventoryGroupConfig groupConfig = groupDrawers[ind].GetConfig();
            obj.SetGroupName(MLocalization.Get(MFormat.GetItemCategoryNameKey(groupConfig.itemCategory), LocalizationGroup.Items));
        }        
	}

    #endregion

	public enum InventoryState {
		None,
		DragFromInventoryToHero,
		DragFromHeroToInventory
    }

}
