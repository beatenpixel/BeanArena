using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {

	public static MenuUI inst;

	public InventoryDrawer inventoryDrawer;
	public InventoryWorldUI editCharacterWorldUI;

	public UIGroupAppear idleGroup;
	public UIGroupAppear editGroup;

	public GameObject rootGO;

	public void Init() {
		inst = this;

		inventoryDrawer.Init();

		MGameLoop.Update.Register(InternalUpdate);
	}
	
	public void InternalUpdate() {
		
	}

	public void EditButton_Click() {
		GM_Menu.inst.SwitchMenuState(GM_Menu.MenuState.CustomizingCharacter);

		inventoryDrawer.Draw();

		idleGroup.Show(false);
		editGroup.Show(true);
		editCharacterWorldUI.Show(true);
	}

	public void EditReadyButton_Click() {
		GM_Menu.inst.SwitchMenuState(GM_Menu.MenuState.Idle);

		idleGroup.Show(true);
		editGroup.Show(false);
		editCharacterWorldUI.Show(false);
	}

	public void Show(bool show) {
		rootGO.SetActive(show);

		if (show) {

		} else {

		}
	}


}
