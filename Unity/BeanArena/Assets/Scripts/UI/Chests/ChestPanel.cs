using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPanel : MonoBehaviour {

    public ChestSlotUI[] slots;

    private Timer timeUpdateTimer;

    public void Init() {
        timeUpdateTimer = new Timer(0.5f,true,false);

        for (int i = 0; i < slots.Length; i++) {
            slots[i].Init(this);
        }
    }

    public void InternalUpdate() {
        if(timeUpdateTimer) {
            timeUpdateTimer.AddFromNow();

            for (int i = 0; i < slots.Length; i++) {
                slots[i].UpdateTime();
            }
        }
    }

    public void Click_OpenChest(ChestSlotUI chestSlot) {
        GD_Chest alreadyOpeningChest = null;
        for (int i = 0; i < slots.Length; i++) {
            if(slots[i].chest != null && slots[i].chest.isOpening) {
                alreadyOpeningChest = slots[i].chest;
                break;
            }
        }

        if (alreadyOpeningChest != null) {
            if(alreadyOpeningChest == chestSlot.chest) {
                if (chestSlot.chest.IsReadyToOpen()) {
                    ForceOpenSlot(chestSlot);
                } else {
                    UIWindowManager.CreateWindow(new UIWData_Message(MLocalization.Get("CHEST_OPEN_NOW", LocalizationGroup.Main, MFormat.GetTMProIcon(TMProIcon.Gem),chestSlot.chest.gemSkipPrice),                       
                        new UIW_ButtonConfig(MLocalization.NO, MAssets.inst.colors[MAssets.COLOR_BUTTON_GRAY], (x) => {

                        }, null),
                        new UIW_ButtonConfig(MLocalization.YES, MAssets.inst.colors[MAssets.COLOR_BUTTON_GREEN], (x) => {
                            if (Economy.inst.HasCurrency(CurrencyType.Gem, chestSlot.chest.gemSkipPrice)) {
                                Economy.inst.TakeCurrency(CurrencyType.Gem, chestSlot.chest.gemSkipPrice);
                                ForceOpenSlot(chestSlot);
                            } else {
                                UIWindowManager.CreateWindow(new UIWData_Message(MLocalization.Get("NOT_ENOUGH_GEMS"),
                                    new UIW_ButtonConfig(MLocalization.OK, MAssets.inst.colors[MAssets.COLOR_BUTTON_GRAY], (x) => {

                                    }, null)
                                ));
                            }
                        }, null)
                        ));
                }
            } else {
                UIWindowManager.CreateWindow(new UIWData_Message(MLocalization.Get("ANOTHER_CHEST_IS_OPENING"),
                        new UIW_ButtonConfig(MLocalization.OK, MAssets.inst.colors[MAssets.COLOR_BUTTON_GRAY], (x) => {

                        }, null)
                        ));
            }
        } else {
            if (chestSlot.chest.isOpening) {
                
            } else {
                chestSlot.StartOpeningChest();
                chestSlot.chest.StartOpening();
            }
        }
    }

    private void ForceOpenSlot(ChestSlotUI slot) {
        MenuUI.inst.chestOpener.ShowChestScreen(slot.chest);
        Game.data.inventory.chests.Remove(slot.chest);
        slot.SetEmpty();

        Draw();
    }

    public void Draw() {
        List<GD_Chest> chests = Game.data.inventory.chests;

        for (int i = 0; i < slots.Length; i++) {
            if(i < chests.Count) {
                slots[i].SetChest(chests[i]);
            } else {
                slots[i].SetEmpty();
            }
        }
    }

}
