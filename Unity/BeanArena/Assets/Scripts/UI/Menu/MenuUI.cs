using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {

	public static MenuUI inst;

	public InventoryDrawer inventoryDrawer;

	public UIGroupAppear idleGroup;
	public UIGroupAppear editGroup;

	public UIGroupAppear worldEditGroup;

	public GameObject rootGO;

	public void Init() {
		inst = this;

		inventoryDrawer.Init();
		inventoryDrawer.Draw();

		MGameLoop.Update.Register(InternalUpdate);
	}
	
	public void InternalUpdate() {
		
	}

	public void EditButton_Click() {
		GM_Menu.inst.SwitchMenuState(GM_Menu.MenuState.CustomizingCharacter);

		idleGroup.Show(false);
		editGroup.Show(true);
		worldEditGroup.Show(true);
	}

	public void EditReadyButton_Click() {
		GM_Menu.inst.SwitchMenuState(GM_Menu.MenuState.Idle);

		idleGroup.Show(true);
		editGroup.Show(false);
		worldEditGroup.Show(false);
	}

	public void Show(bool show) {
		rootGO.SetActive(show);

		if (show) {

		} else {

		}
	}


}
