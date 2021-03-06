using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIW_RoundEnd : UIWindow {

    public UISimpleButton goToMenuButton;
    public TextMeshProUGUI logoText;

    public TextMeshProUGUI MMRCountText;
    public TextMeshProUGUI CoinsCountText;

    public IconDrawer chestIconDrawer;

    private UIWData_RoundEnd data;

    public override void OnInitBeforeOpen() {
        data = (UIWData_RoundEnd)genericWindowData;

        if (data.earnedChest != null) {
            chestIconDrawer.Show(true);
            chestIconDrawer.DrawChest(data.earnedChest);            
        } else {
            chestIconDrawer.Show(false);
        }

        if (data.vsType == GameModeVSType.Bot) {
            if (data.win) {
                logoText.text = MLocalization.Get("WIN");
                goToMenuButton.SetBackgroundColor(MAssets.inst.colors["button_green"]);
                MMRCountText.text = MFormat.GetTMProIcon(TMProIcon.Cup)  + "+" + data.mmrCount;
            } else {
                logoText.text = MLocalization.Get("LOSE");
                goToMenuButton.SetBackgroundColor(MAssets.inst.colors["button_red"]);
                MMRCountText.text = MFormat.GetTMProIcon(TMProIcon.Cup) + "-" + data.mmrCount;
            }
        } else if(data.vsType == GameModeVSType.Local) {
            logoText.text = MLocalization.Get("WIN_LOCAL_TEXT", LocalizationGroup.Main, data.wonPlayerName);
            goToMenuButton.SetBackgroundColor(MAssets.inst.colors["button_green"]);
            MMRCountText.text = "";
        }

        CoinsCountText.text = "<sprite name=\"coin\">+" + data.coinCount;
    }

    public override void InternalUpdate() {

    }

    public override void OnStartOpening() {

    }

    public override void OnOpened() {

    }

    public override void OnStartClosing() {

    }

    public override void OnClosed() {

    }

    public void ButtonClick_GoToMenu() {
        GameMode.current.ExitGame();
        Open(false);
        AdManager.inst.TryShowInterstitial();
    }

    public override Type GetPoolObjectType() {
        return typeof(UIWindow);
    }

    public override UIW_Opener GenerateWindowOpener() {
        return new IGNWindowSimpleOpener();
    }

    public override UIWindowInfo GetUIWindowInfo() {
        return new UIWindowInfo() {
            windowDataType = typeof(UIWData_RoundEnd),
            windowType = typeof(UIW_RoundEnd),
            layer = UIWindowLayerType.Main
        };
    }
}

public class UIWData_RoundEnd : UIW_Data {

    public bool win;
    public int mmrCount;
    public int coinCount;
    public GD_Chest earnedChest;

    public GameModeVSType vsType;
    public string wonPlayerName;

    public UIWData_RoundEnd(GameModeVSType _vsType) {
        vsType = _vsType;
    }

    public UIWData_RoundEnd(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}
