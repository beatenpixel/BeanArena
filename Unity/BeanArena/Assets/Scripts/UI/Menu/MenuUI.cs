using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour {

	public static MenuUI inst;

    public MenuTopPanel topPanel;
    public ChestPanel chestPanel;

	public InventoryUI inventoryDrawer;
	public InventoryWorldUI editCharacterWorldUI;

	public UIGroupAppear idleGroup;
	public UIGroupAppear editGroup;

    public TextMeshProUGUI mmrText;

	public GameObject rootGO;

	public void Init() {
		inst = this;

		inventoryDrawer.Init();
        topPanel.Init();
        chestPanel.Init();

        DrawMMRText();
        topPanel.Draw();

        MGameLoop.Update.Register(InternalUpdate);
	}
	
	public void InternalUpdate() {
        chestPanel.InternalUpdate();
    }

	public void EditButton_Click() {
		GM_Menu.inst.SwitchMenuState(GM_Menu.MenuState.CustomizingCharacter);

        inventoryDrawer.Draw(true);

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

    public void ButtonClick_GoFight() {
        GM_Menu.inst.GoToFight();
    }

    public void DrawMMRText() {
        mmrText.text = "<sprite name=\"cup\">" + Game.data.player.mmr.ToString();
    }

	public void Show(bool show) {
		rootGO.SetActive(show);

		if (show) {
            topPanel.Draw();
            DrawMMRText();
            chestPanel.Draw();
            //inventoryDrawer.SpawnPreviewItems();
        } else {

		}
	}


}
