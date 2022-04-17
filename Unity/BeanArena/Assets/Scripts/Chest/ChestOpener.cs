using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpener : MonoBehaviour {

    public Animator anim;
    public Camera chestCam;

    public Transform chestHolderT;
    public GameObject[] chestPrefabs;
    public GameObject currentChestGO;

    public ChestOpenerState state;

    private float timeOfDelay;
    private GD_Chest openingChest;

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

    public void ShowChestScreen(GD_Chest chest) {
        if(state != ChestOpenerState.None) {
            return;
        }

        if(currentChestGO != null) {
            Destroy(currentChestGO);
        }

        //anim.enabled = false;

        currentChestGO = Instantiate(chestPrefabs[(int)(chest.type - 1)], chestHolderT);
        currentChestGO.name = "chest";
        currentChestGO.transform.SetAsFirstSibling();
        anim.Rebind();

        openingChest = chest;

        state = ChestOpenerState.WaitToClick;
        timeOfDelay = Time.realtimeSinceStartup;

        MenuUI.inst.Show(false);
        chestCam.enabled = true;
        //anim.enabled = true;
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
            content = GameBalance.GenerateChestContent(openingChest),
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
