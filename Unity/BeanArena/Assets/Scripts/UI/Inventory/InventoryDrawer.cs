using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDrawer : MonoBehaviour {

	public RectTransform tabButtonsHolderRectT;
	public RectTransform inventoryGroupsHolderT;

	public InventoryWorldUI worldUI;

	public List<InventoryGroupDrawer> groupDrawers;

	private ObjectListSpawner<InventoryTabButton> tabButtons;

	private bool generatedElements;
	private int currentGroupID = 0;
	private ChangeCheck<bool> isInsideRect = new ChangeCheck<bool>(false);

	[HideInInspector] public ItemButton currentDragedButton;

	public void Init() {
		tabButtons = new ObjectListSpawner<InventoryTabButton>(Spawn_InventoryTabButton, Update_InventoryTabButton, Disable_InventoryTabButton);
		tabButtons.Spawn(groupDrawers.Count);

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
	}

    private void Update() {
		if (currentDragedButton != null) {
			worldUI.SetAreaGlowing(true);

			isInsideRect.Set(worldUI.CheckInsideDropArea());

			if (isInsideRect.CheckIsDirtyAndClear()) {
				Debug.Log("Area: " + isInsideRect.value);
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

}
