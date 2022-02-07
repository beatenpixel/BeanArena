using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class UIW_RoundEnd : UIWindow {

    public UISimpleButton goToMenuButton;
    public TextMeshProUGUI logoText;

    public TextMeshProUGUI MMRCountText;
    public TextMeshProUGUI CoinsCountText;

    private UIWData_RoundEnd data;

    public override void OnInit() {
        data = (UIWData_RoundEnd)genericWindowData;        

        if (data.win) {
            logoText.text = MLocalization.Get("WIN");
            goToMenuButton.SetBackgroundColor(MAssets.colors["button_green"]);
            MMRCountText.text = "<sprite name=\"cup\">+" + data.mmrCount;
        } else {
            logoText.text = MLocalization.Get("LOSE");
            goToMenuButton.SetBackgroundColor(MAssets.colors["button_red"]);
            MMRCountText.text = "<sprite name=\"cup\">-" + data.mmrCount;
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
            layer = UIWindowLayerType.Notification
        };
    }
}

public class UIWData_RoundEnd : UIW_Data {

    public bool win;
    public int mmrCount;
    public int coinCount;

    public UIWData_RoundEnd() {
    }

    public UIWData_RoundEnd(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}
