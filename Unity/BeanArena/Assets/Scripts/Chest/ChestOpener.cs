using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpener : MonoBehaviour {

    public Animator anim;
    public Camera chestCam;

    public ChestOpenerState state;

    private float timeOfDelay;

    public void Init() {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (state == ChestOpenerState.WaitToClick) {
                if(Time.realtimeSinceStartup > timeOfDelay + 0.3f) {
                    Input_OpenChest();
                }
            } else if(state == ChestOpenerState.Opened) {
                if(Time.realtimeSinceStartup > timeOfDelay + 0.3f) {
                    Exit();
                }
            }
        }        
    }

    public void ShowChestScreen(ChestType chestType) {
        if(state != ChestOpenerState.None) {
            return;
        }

        state = ChestOpenerState.WaitToClick;
        timeOfDelay = Time.realtimeSinceStartup;

        MenuUI.inst.Show(false);
        chestCam.enabled = true;
        anim.Play("common_chest_open");
    }

    public void Input_OpenChest() {
        anim.Play("common_chest_open_2");
        state = ChestOpenerState.Opened;

        this.WaitRealtime(() => {
            Exit();
        }, 0.2f);

        timeOfDelay = Time.realtimeSinceStartup;
    }

    private void CloseChestOpener() {
        state = ChestOpenerState.None;
        MenuUI.inst.Show(true);
        chestCam.enabled = false;
        anim.Play("idle");
    }

    public void Exit() {
        state = ChestOpenerState.Exit;
        UIWindowManager.CreateWindow(new UIWData_ChestReward() {
            items = Game.data.inventory.items,
            fadeBlackBackground = false,
            CloseChestOpenerCallback = CloseChestOpener
        });
    }

    public enum ChestOpenerState {
        None,
        Drop,
        WaitToClick,
        Opened,
        Exit
    }

}
