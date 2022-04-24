using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class UIW_SearchingOnlineGame : UIWindow {

    public TextMeshProUGUI beansOnlineText;

    private UIWData_SearchOnlineGame data;

    private Timer tickTimer;

    protected override void Awake() {
        base.Awake();

        tickTimer = new Timer(0.2f);
    }

    public override void OnInitBeforeOpen() {
        data = (UIWData_SearchOnlineGame)genericWindowData;
    }

    public override void InternalUpdate() {
        if(tickTimer) {
            tickTimer.AddFromNow();

            Tick();
        }
    }

    private void Tick() {
        beansOnlineText.text = MLocalization.Get("BEANS_ONLINE_TEXT", LocalizationGroup.Main, BeanNetwork.inst.photonNetworkManager.GetTotalPalyersOnlineCount());
    }

    public override void OnStartOpening() {

    }

    public override void OnOpened() {

    }

    public override void OnStartClosing() {

    }

    public override void OnClosed() {

    }

    public override Type GetPoolObjectType() {
        return typeof(UIWindow);
    }

    public override UIW_Opener GenerateWindowOpener() {
        return new IGNWindowSimpleOpener();
    }

    public override UIWindowInfo GetUIWindowInfo() {
        return new UIWindowInfo() {
            windowDataType = typeof(UIWData_SearchOnlineGame),
            windowType = typeof(UIW_SearchingOnlineGame),
            layer = UIWindowLayerType.Main
        };
    }
}

public class UIWData_SearchOnlineGame : UIW_Data {    

    public UIWData_SearchOnlineGame() {

    }

    public UIWData_SearchOnlineGame(SerializationInfo info, StreamingContext sc) : base(info, sc) {

    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        base.GetObjectData(info, context);
    }

}
