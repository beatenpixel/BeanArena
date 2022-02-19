using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

	public InventoryState inventoryState;

	public RectTransform tabButtonsHolderRectT;
	public RectTransform inventoryGroupsHolderT;
    public RectTransform itemsInfoRectT;
    [SerializeField]private RectTransform selectionRect;

    public UICompHolder[] onDragItemButtonScrollingLines;

	public InventoryWorldUI worldUI;
	public List<InventoryGroupDrawer> groupDrawers;

	private ObjectListSpawner<InventoryTabButton> tabButtons;
	private bool generatedElements;
	private int currentGroupID = 0;
	private ChangeCheck<bool> isInsideHeroWorldRect = new ChangeCheck<bool>(false);
	private ChangeCheck<bool> isInsideInfoRect = new ChangeCheck<bool>(false);

	[HideInInspector] public ItemButton currentDragedButton;

    public bool showedAnyItemInfo;

	private Vector3[] worldCornersCache = new Vector3[4];
    private bool groupsAreSetup;

	public void Init() {
        groupsAreSetup = false;
        showedAnyItemInfo = false;

        isInsideInfoRect = new ChangeCheck<bool>(false);

        tabButtons = new ObjectListSpawner<InventoryTabButton>(Spawn_InventoryTabButton, Enable_InventoryTabButton, Update_InventoryTabButton, Destroy_InventoryTabButton);
		tabButtons.Update(groupDrawers.Count);

		for (int i = 0; i < tabButtons.activeObjectsCount; i++) {
			tabButtons[i].SetOnClick(OnTabButtonClick, i);
		}

        List<ItemCategory> groupsCategory = new List<ItemCategory>() {
            ItemCategory.Weapon,
            ItemCategory.Head,
            ItemCategory.Hero
        };

        for (int i = 0; i < groupsCategory.Count; i++) {
            groupDrawers[i].Init(new InventoryGroupConfig() {
                groupID = i,
                itemCategory = groupsCategory[i],
                tabButton = tabButtons[i],
                inventoryUI = this,
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
                    //groupDrawers[i].ShowInfoPanel(true);
                } else {
                    groupDrawers[i].Show(false);
                }
            } else {
                if (i == currentGroupID) {
                    groupDrawers[i].Draw();
                    groupDrawers[i].Show(true);
                    //groupDrawers[i].ShowInfoPanel(true);
                } else {
                    groupDrawers[i].Show(false);
                }
            }			
        }
	}

    public void OnClose() {
        foreach (var item in Game.data.inventory.items) {
            item.isNew = false;
        }
    }

    public void OnHeroItemButtonEvent(UIEventType e, HeroItemButton button, object arg) {

    }

    public void OnItemButtonEvent(UIEventType e, ItemButton button, object arg) {
		switch (e) {
            case UIEventType.DragUpdate:
                if (button.buttonState == ItemButton.ItemButtonState.InInventory) {
                    isInsideInfoRect.Set(CheckInsideItemsInfoArea());

                    if (isInsideInfoRect.CheckIsDirtyAndClear()) {
                        Debug.Log("isInsideInfoRect");

                        if(isInsideInfoRect.value) {
                            PreivewMergeItems(button, true);
                        } else {
                            PreivewMergeItems(button, false);
                        }                        
                    }
                }
                break;

			case UIEventType.DragEnd:

                Debug.Log("DragEnd" + Time.time);

				if (button.buttonState == ItemButton.ItemButtonState.InInventory) {
					if (worldUI.CheckInsideDropArea()) {
                        SetItemButtonEquiped(button);
					} else if(CheckInsideItemsInfoArea()) {
                        MergeItems(button);
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
                    } else {
                        Draw(false);
                    }
                }				

				currentDragedButton = null;

                onDragItemButtonScrollingLines[0].go.SetActive(false);
                onDragItemButtonScrollingLines[1].go.SetActive(false);
                break;
			case UIEventType.DragStart:
				currentDragedButton = button;

                if (button.buttonState == ItemButton.ItemButtonState.InHeroSlot) {

                } else {
                    onDragItemButtonScrollingLines[0].go.SetActive(true);

                    onDragItemButtonScrollingLines[1].text.text = MLocalization.Get("EQUIP");
                    onDragItemButtonScrollingLines[1].go.SetActive(true);
                }
                break;
            case UIEventType.Click:
                if(currentGroupID != button.inventoryGroupDrawer.groupID) {
                    SwitchTab(button.inventoryGroupDrawer.groupID);
                }
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

    public void PreivewMergeItems(ItemButton button, bool bringItemIn) {
        groupDrawers[currentGroupID].PreviewMergeItems(button, bringItemIn);
    }

    public void MergeItems(ItemButton button) {
        groupDrawers[currentGroupID].MergeItems(button);
    }

    public bool CheckInsideItemsInventoryArea() {
		Vector2 screenPos = Input.mousePosition;
		inventoryGroupsHolderT.GetWorldCorners(worldCornersCache);
		bool isInside = MUtils.PointIsInsideCorners(screenPos, worldCornersCache);
		return isInside;
	}

    public bool CheckInsideItemsInfoArea() {
        Vector2 screenPos = Input.mousePosition;
        itemsInfoRectT.GetWorldCorners(worldCornersCache);
        bool isInside = MUtils.PointIsInsideCorners(screenPos, worldCornersCache);
        return isInside;
    }

    private void OnTabButtonClick(InventoryTabButton tabButton, object arg) {
		int tabID = (int)arg;

        SwitchTab(tabID);		
    }

    private void SwitchTab(int tabID) {
        if (currentGroupID != -1) {
            groupDrawers[currentGroupID].infoDrawer.Show(false);
        }

        currentGroupID = tabID;
        Draw(false);
    }

    public void Select(RectTransform target) {
        selectionRect.gameObject.SetActive(true);
        selectionRect.SetParent(target);
        selectionRect.localScale = Vector3.one;
        selectionRect.anchorMin = Vector2.zero;
        selectionRect.anchorMax = Vector2.one;
        selectionRect.offsetMin = new Vector2(-4, -4);
        selectionRect.offsetMax = new Vector2(4, 4);
        selectionRect.SetParent(target.parent);
        selectionRect.SetSiblingIndex(Mathf.Clamp(target.GetSiblingIndex() - 1, 0, int.MaxValue));
    }

    public void Unselect() {
        selectionRect.gameObject.SetActive(false);
        selectionRect.SetParent(null);
    }

    public void ShowItemInfoBorderNotification(string notificationText) {
        onDragItemButtonScrollingLines[0].text.text = notificationText;
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

    private void Destroy_InventoryTabButton(InventoryTabButton obj, int ind) {
        
    }

    #endregion

    public enum InventoryState {
		None,
		DragFromInventoryToHero,
		DragFromHeroToInventory
    }

}
