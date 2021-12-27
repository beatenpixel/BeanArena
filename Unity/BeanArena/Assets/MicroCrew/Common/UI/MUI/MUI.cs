using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MUI : Singleton<MUI> {

    public UICanvas uiCanvas;

    public UI_Button button;

    public static float sizeX;
    public static float sizeY;

    public static float scaleX;
    public static float scaleY;


    public override void Init() {
        /*
        this.Wait(() => {
            scaleX = uiCanvas.canvasT.localScale.x;
            scaleY = uiCanvas.canvasT.localScale.y;

            sizeX = uiCanvas.canvasT.sizeDelta.x;
            sizeY = uiCanvas.canvasT.sizeDelta.y;
            Debug.Log("size: " + sizeX + ";" + sizeY);

            var button2 = new UI_Button("btn2", null, new UI_Button.UIData_Button() {
                defaultColor = MAssets.colors["button"],
                OnClick = () => {
                    Debug.Log("ON CLICK");
                }
            });
            button2.rootRend.AnchorMin(0f, 0f).AnchorMax(1f, 0.2f).Offset(0,0);
            button2.CreateText("button", Color.white).Align(TMPro.TextAlignmentOptions.Left);

            var switchPanel = new UI_SwitchPanel("panel", MUI.inst.uiCanvas.canvasT, 3);
        }, 1);
        */
    }

    protected override void Shutdown() {
        
    }

}
